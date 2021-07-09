using MLAPI;
using MLAPI.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;
using RSG;

public class FieldManager : MonoBehaviour
{
    private const float COMMAND_WAIT_TIME = 0.3f;
    private readonly WaitForSeconds WAIT_FOR_COMMAND = new WaitForSeconds(COMMAND_WAIT_TIME);

    [Header("Prefabs")]
    [SerializeField] private Meeple[] _villagerPrefabs;
    [SerializeField] private Knight _knightPrefab;

    [Header("Field")]
    [SerializeField] private Transform[] _rows;
    [SerializeField] private Transform[] _squads;
    [SerializeField] private Transform _tempStorage;
    [SerializeField] private Transform _authorizeStorage;
    
    [Header("Other")]
    [SerializeField] private AttackerEffects _attackerEffects;

    private Tile[][] _field;
    private Tile[][] _battleField;
    private Tile _tempStorageTile;
    private Tile _authorizeStorageTile;

    // store tile references for multi step actions
    private Tile _storeTile;
    private Tile _storeSecondTile;
    private Stack<Tile> _riotPathStack = new Stack<Tile>();
    private List<DVillager> _drawnVillagersThisTurn = new List<DVillager>();

    // Use properties for network variable update
    public Tile StoreTile
    {
        get => _storeTile;
        set
        {
            if (_storeTile != value)
            {
                _storeTile = value;
                // MARK: Update tile if connected, supports local debugging
                if ((NetworkManager.Singleton?.IsConnectedClient).GetValueOrDefault())
                    NetworkFieldManager.Instance.NetworkStoreTile.Value = value;
            }
        }
    }

    public Tile StoreSecondTile
    {
        get => _storeSecondTile;
        set
        {
            if (_storeSecondTile != value)
            {
                _storeSecondTile = value;
                // MARK: Update tile if connected, supports local debugging
                if ((NetworkManager.Singleton?.IsConnectedClient).GetValueOrDefault())
                    NetworkFieldManager.Instance.NetworkStoreSecondTile.Value = value;
            }
        }
    }

    #region Setup

    private void Awake()
    {
        _field = GetField(_rows, false);
        _battleField = GetField(_squads, true);
        _tempStorageTile = _tempStorage.GetComponent<Tile>();
        _authorizeStorageTile = _authorizeStorage.GetComponent<Tile>();
    }

    private void Start()
    {
        PopulateField();
        StateManager.OnStateUpdate += state => NetworkedUpdateInteractability();
    }

    private Tile[][] GetField(Transform[] sourceArray, bool isBattleField)
    {
        // init field reference
        var field = new Tile[sourceArray.Length][];
        for (var i = 0; i < sourceArray.Length; i++)
        {
            var tiles = sourceArray[i].GetComponentsInChildren<Tile>();
            field[i] = tiles;

            // set id's
            for (var j = 0; j < tiles.Length; j++)
            {
                tiles[j].ID = new Vector3(i, j, isBattleField ? -1 : 1);
                RegistryManager.Instance.Register(tiles[j].ID, tiles[j]);
            }
        }

        return field;
    }

    public Tile TileByPosition(DPosition position)
    {
        return (position == null) ? _tempStorageTile : _field[position.Row - 1][position.Col - 1];
    }

    public Tile TileByStateAndPosition(DMeeple.MeepleState state, DPosition position)
    {
        return (state == DMeeple.MeepleState.Authorized) ? _authorizeStorageTile : TileByPosition(position);
    }

    public Tile VacantBattlefieldTile(DPlayer.ID playerID)
    {
        foreach (var row in _battleField)
        {
            foreach (var tile in row)
            {
                if (tile.Meeples.Count == 0 && tile.NominalOwner == playerID)
                {
                    return tile;
                }
            }
        }

        return null;
    }

    private void PopulateField()
    {
        var meeple = GameState.Instance.Meeple; //MatchingMeepleOnBoard(m => true);
        foreach (var meep in meeple)
        {
            var tile = TileByPosition(meep.Position.Current);
            if (tile != null)
            {
                Meeple? prefab = null;
                if (meep.GetType() == typeof(DFarmer))
                {
                    prefab = _villagerPrefabs[0];
                }
                else if (meep.GetType() == typeof(DMerchant))
                {
                    prefab = _villagerPrefabs[1];
                }
                else if (meep.GetType() == typeof(DScholar))
                {
                    prefab = _villagerPrefabs[2];
                }
                else if (meep.GetType() == typeof(DKnight))
                {
                    prefab = _knightPrefab;
                }

                if (prefab != null)
                {
                    Meeple newguy = Instantiate(prefab);
                    newguy.Initialize(meep, this);
                }
            }
        }
    }

    #endregion

    #region Field Actions

    // TODO: All issuer IDs are 0 by default, use actual client ID after network integration

    public IEnumerator Authorize(DWorker worker)
    {
        CommandProcessor.Instance.ExecuteCommand(
            new AuthorizeCommand(
                0,
                GameState.Instance.TurnPlayer(),
                worker,
                StoreTile.Meeples[0].Core)
        );

        yield return WAIT_FOR_COMMAND;

        StoreTile = null;
        StateManager.CurrentState = StateManager.State.Default;
    }

    public IEnumerator Swap(DWorker worker)
    {
        CommandProcessor.Instance.ExecuteCommand(
            new SwapCommand(
                0, // --> TODO: should this be zero?
                GameState.Instance.TurnPlayer(),
                worker,
                GameState.Instance.AtPosition(StoreTile.Position),
                GameState.Instance.AtPosition(StoreSecondTile.Position)
            )
        );

        yield return WAIT_FOR_COMMAND;

        StoreTile = StoreSecondTile = null;
        StateManager.CurrentState = StateManager.State.Default;
    }

    public IEnumerator RiotStep(Tile tile)
    {
        var previousTile = _riotPathStack.Peek();
        _riotPathStack.Push(tile);
        var knight =
            (DKnight) GameState.Instance.AllAtPosition(previousTile.Position, m => m.GetType() == typeof(DKnight))[0];
        CommandProcessor.Instance.ExecuteCommand(
            new RiotStepCommand(
                0,
                GameState.Instance.TurnPlayer(),
                knight,
                tile.Position
            )
        );

        yield return WAIT_FOR_COMMAND;

        StateManager.CurrentState =
            tile.Position.IsFinal ? StateManager.State.RiotAuthorize : StateManager.State.RiotChoosePath;
    }

    private IEnumerator StartRiot(DWorker worker)
    {
        var meeple = GameState.Instance.AtPosition(_riotPathStack.Peek().Position);
        CommandProcessor.Instance.ExecuteCommand(new StartRiotCommand(
            0,
            GameState.Instance.TurnPlayer(),
            worker,
            meeple
        ));

        yield return WAIT_FOR_COMMAND;

        StateManager.CurrentState = StateManager.State.RiotChoosePath;
    }

    public void UndoRiotStep()
    {
        if (_riotPathStack.Count > 0)
        {
            _riotPathStack.Pop();
            UpdateInteractability();
        }
    }

    public void AuthorizeRiot()
    {
        var tile = _riotPathStack.Peek();
        var knight = (DKnight) GameState.Instance.AllAtPosition(
            tile.Position, m => m.GetType() == typeof(DKnight))[0];
        CommandProcessor.Instance.ExecuteCommand(
            new RiotAuthorizeCommand(
                0,
                GameState.Instance.TurnPlayer(),
                knight,
                tile.Position
            )
        );
    }

    public IEnumerator Revive(DWorker worker)
    {
        //var worker = new DWorker(GameState.Instance.TurnPlayer().Id);
        CommandProcessor.Instance.ExecuteCommand(new ReviveCommand(
                0,
                GameState.Instance.TurnPlayer(),
                worker,
                StoreTile.Meeples[0].Core
            )
        );

        yield return WAIT_FOR_COMMAND;

        StoreTile = null;
        StateManager.CurrentState = StateManager.State.Default;
    }

    private IEnumerator Retreat(Tile tile)
    {
        CommandProcessor.Instance.ExecuteCommand(new RetreatCommand(
                0,
                StoreTile.Meeples[0].Core as DKnight,
                tile.Position
            )
        );
        
        _attackerEffects.Play();

        yield return WAIT_FOR_COMMAND;
        
        StoreSecondTile = null;
        StateManager.CurrentState = StateManager.State.Default;
    }

    public IEnumerator Villager(Tile tile)
    {
        // if there is an villager in this list, that is not on the board
        // it must be due to an undo of the draw villager command
        // draw this villager again, to have consistent undo/redo
        DVillager villager = _drawnVillagersThisTurn.FirstOrDefault(v => v.State == DMeeple.MeepleState.OutOfBoard);

        // no undrawn villager in list, draw new one
        if (villager == null)
        {
            villager = GameState.Instance.DrawVillager();
            _drawnVillagersThisTurn.Add(villager);
        }

        CommandProcessor.Instance.ExecuteCommand(new DrawVillagerCommand(0, villager, tile.Position));

        yield return WAIT_FOR_COMMAND;
        
        StateManager.CurrentState = StateManager.State.Default;
    }

    public IEnumerator MoveMeeple(Tile tile)
    {
        CommandProcessor.Instance.ExecuteCommand(new MoveVillagerCommand(
            0, 
            GameState.Instance.TurnPlayer(),
            StoreTile.Meeples[0].Core, 
            tile.Position));

        yield return WAIT_FOR_COMMAND;
        
        StoreTile = null;
        StateManager.CurrentState = StateManager.State.Default;
    }

    #endregion

    #region Interaction (state-depending)

    public void ProcessClickedTile(Tile tile)
    {
        switch (StateManager.CurrentState)
        {
            case StateManager.State.Authorize:
                StoreTile = tile;
                StateManager.CurrentState = StateManager.State.PayForAction;
                break;
            case StateManager.State.Swap1:
                StoreSecondTile = tile;
                StateManager.CurrentState = StateManager.State.Swap2;
                break;
            case StateManager.State.Swap2:
                StoreTile = tile;
                StateManager.CurrentState = StateManager.State.PayForAction;
                break;
            case StateManager.State.RiotChooseKnight:
                // _storeTile = tile;
                _riotPathStack.Clear();
                _riotPathStack.Push(tile);
                StateManager.CurrentState = StateManager.State.PayForAction;
                break;
            //TODO: Can we add a case for RiotChooseFollowerType
            case StateManager.State.RiotChoosePath:
                StartCoroutine(RiotStep(tile));
                break;
            case StateManager.State.Revive:
                StoreTile = tile;
                StateManager.CurrentState = StateManager.State.PayForAction;
                break;
            // TODO (metul): Wait until command executed before changing state (latency on RPCs)
            case StateManager.State.RetreatChooseTile:
                StartCoroutine(Retreat(tile));
                break;
            case StateManager.State.RetreatChooseKnight:
                StoreTile = tile;
                StateManager.CurrentState = StateManager.State.RetreatChooseTile;
                break;
            case StateManager.State.Villager:
                StartCoroutine(Villager(tile));
                break;
            case StateManager.State.MoveMeeple:
                StartCoroutine(MoveMeeple(tile));
                break;
            case StateManager.State.Default:
                if (GameState.Instance.TurnType == GameState.TurnTypes.ResetTurn)
                {
                    StoreTile = tile;
                    StateManager.CurrentState = StateManager.State.MoveMeeple;
                }

                break;
        }
    }

    public void InvokeAction(StateManager.State action, DWorker worker)
    {
        // TODO (metul): Wait until command executed before changing state (latency on RPCs)
        switch (action)
        {
            case StateManager.State.Authorize:
                StartCoroutine(Authorize(worker));
                break;
            case StateManager.State.Swap2:
                StartCoroutine(Swap(worker));
                break;
            case StateManager.State.Revive:
                StartCoroutine(Revive(worker));
                break;
            case StateManager.State.RiotChooseKnight:
                StartCoroutine(StartRiot(worker));
                break;
        }
    }

    public void NetworkedUpdateInteractability()
    {
        if ((NetworkManager.Singleton?.IsConnectedClient).GetValueOrDefault() &&
            (!NetworkManager.Singleton?.IsServer).GetValueOrDefault())
        {
            //NetworkLog.LogInfoServer($"NetworkedUpdateInteractability: {NetworkManager.Singleton.LocalClientId}");
            if (PlayerManager.Instance.NetworkPlayerIDs[NetworkManager.Singleton.LocalClientId] ==
                GameState.Instance.TurnPlayer().Id)
                UpdateInteractability();
            else
            {
                DisableAllTiles();
                DisableBattlefield();
            }
        }
        else // MARK: Allow local debugging (also updates interactability on server)
            UpdateInteractability();
    }

    private void UpdateInteractability()
    {
        switch (StateManager.CurrentState)
        {
            case StateManager.State.Authorize:
                EnableAuthorizable();
                break;
            case StateManager.State.Swap1:
                EnableInjuryBased(false);
                break;
            case StateManager.State.Swap2:
                EnableNeighbours();
                break;
            case StateManager.State.RiotChooseKnight:
                EnableRiotableKnights();
                break;
            case StateManager.State.RiotChoosePath:
                EnableRiotPath();
                break;
            case StateManager.State.Revive:
                EnableInjuryBased(true);
                break;
            case StateManager.State.RetreatChooseTile:
                EnableRetreatable();
                break;
            case StateManager.State.RetreatChooseKnight:
                EnableBattlefield();
                break;
            case StateManager.State.Villager:
                EnableRetreatable();
                break;
            case StateManager.State.MoveMeeple:
                EnableMovingMeeple();
                break;
            case StateManager.State.Default:
                StoreTile = null;
                StoreSecondTile = null;
                DisableAllTiles();
                DisableBattlefield();

                if (GameState.Instance.TurnType == GameState.TurnTypes.ResetTurn)
                {
                    EnableInjuryBased(false);
                }

                break;
            default:
                DisableAllTiles();
                DisableBattlefield();
                break;
        }
    }

    private void DisableAllTiles()
    {
        foreach (var tiles in _field)
        {
            foreach (var tile in tiles)
            {
                tile.Interactable = false;
            }
        }
    }

    private void EnableEmptyTiles()
    {
        foreach (var tiles in _field)
        {
            foreach (var tile in tiles)
            {
                tile.Interactable = (tile.Meeples.Count == 0);
            }
        }
    }

    private void EnableInjuryBased(bool enableInjured)
    {
        GameState.Instance.TraverseBoard(p =>
        {
            var tile = TileByPosition(p);
            if (enableInjured)
            {
                tile.Interactable = GameState.Instance.InjuredVillagerAtPosition(p);
            }
            else
            {
                tile.Interactable = GameState.Instance.HealthyMeepleAtPosition(p);
            }
        });
    }

    private void EnableNeighbours(bool overwriteInjured = false)
    {
        GameState.Instance.TraverseBoard(p =>
        {
            var tile = TileByPosition(p);
            tile.Interactable = _storeSecondTile.Position.Neighbors(tile.Position) &&
                                (overwriteInjured || GameState.Instance.HealthyMeepleAtPosition(p));
        });
    }

    private void EnableRetreatable()
    {
        GameState.Instance.TraverseBoard(p =>
        {
            var tile = TileByPosition(p);
            tile.Interactable =
                GameState.Instance.IsEmpty(p)
                && p.Row == Rules.ROWS;

            /*
            GameState.Instance.PathExists(
                DPosition.LastRow(),
                p,
                p => GameState.Instance.IsEmpty(p)
            );*/
        });
    }

    private void EnableKnights()
    {
        GameState.Instance.TraverseBoard(p =>
        {
            var tile = TileByPosition(p);
            var meeple = GameState.Instance.AtPosition(p);

            tile.Interactable = meeple != null && meeple.GetType() == typeof(DKnight);
        });
    }

    private void EnableRiotableKnights()
    {
        var endpoint = new DPosition(1, 1);
        GameState.Instance.TraverseBoard(p =>
        {
            var tile = TileByPosition(p);
            var meeple = GameState.Instance.AtPosition(p);
            var endguy = GameState.Instance.AtPosition(endpoint);

            tile.Interactable =
                meeple != null && meeple.GetType() == typeof(DKnight)
                               && (
                                   p.Equals(endpoint) ||
                                   (
                                       (endguy == null ||
                                        (endguy.IsHealthy() && endguy.GetType() != typeof(DKnight))) &&
                                       GameState.Instance.PathExists(
                                           p,
                                           endpoint,
                                           _p =>
                                           {
                                               if (_p.Equals(p))
                                                   return true;
                                               var onTheWay = GameState.Instance.AtPosition(_p);
                                               return onTheWay == null || (
                                                   onTheWay.IsHealthy() &&
                                                   onTheWay.GetType() != typeof(DKnight)
                                               );
                                           }
                                       )
                                   )
                               );
        });
    }

    private void EnableAuthorizable()
    {
        var endpoint = new DPosition(1, 1);
        var endguy = GameState.Instance.AtPosition(endpoint);

        GameState.Instance.TraverseBoard(p =>
        {
            var tile = TileByPosition(p);
            var meep = GameState.Instance.AtPosition(p);
            tile.Interactable =
                meep != null && meep.IsHealthy() && // --> there is a healthy meeple at this position
                (
                    p.Equals(endpoint) || ( // --> this position is endpoint itself, or ...
                        endguy == null && // --> endpoint is empty
                        GameState.Instance.PathExists( // --> and there is a path
                            p, // --> from this point
                            endpoint, // --> to the endpoint
                            _p => p.Equals(_p)
                                  || GameState.Instance
                                      .IsEmpty(_p) // --> where the only non-empty step is the first one
                        )
                    )
                );
        });
    }

    private void EnableRiotPath()
    {
        if (_riotPathStack.Count == 0)
            return;

        var last = _riotPathStack.Peek().Position;

        GameState.Instance.TraverseBoard(p =>
        {
            var tile = TileByPosition(p);
            var meeple = GameState.Instance.AtPosition(p);
            tile.Interactable = (
                last.CanMoveTo(p) &&
                (
                    meeple == null ||
                    (
                        meeple.IsHealthy() &&
                        meeple.GetType() != typeof(DKnight)
                    )
                )
            );
        });
    }

    private void EnableMovingMeeple()
    {
        var start = StoreTile.Position;

        GameState.Instance.TraverseBoard(p =>
        {
            var tile = TileByPosition(p);
            tile.Interactable = start.CanMoveTo(p) && GameState.Instance.IsEmpty(p);
        });
    }

    private void EnableBattlefield()
    {
        DisableAllTiles();
        foreach (var tiles in _battleField)
        {
            foreach (var tile in tiles)
            {
                tile.Interactable = (tile.Meeples.Count != 0);
            }
        }
    }

    private void DisableBattlefield()
    {
        foreach (var tiles in _battleField)
        {
            foreach (var tile in tiles)
            {
                tile.Interactable = false;
            }
        }
    }

    public void EndTurnReset()
    {
        _storeTile = null;
        _storeSecondTile = null;
        _riotPathStack.Clear();
        _drawnVillagersThisTurn.Clear();

        NetworkedUpdateInteractability();
    }

    #endregion
}