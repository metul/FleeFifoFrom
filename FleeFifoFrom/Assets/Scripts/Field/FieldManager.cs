using MLAPI;
using MLAPI.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class FieldManager : MonoBehaviour
{
    [SerializeField] private Meeple[] _villagerPrefabs;
    [SerializeField] private Knight _knightPrefab;

    [SerializeField] private Transform[] _rows;
    [SerializeField] private Transform[] _squads;
    [SerializeField] private Transform _tempStorage;

    private Tile[][] _field;
    private Tile[][] _battleField;
    private Tile _tempStorageTile;

    #region Placeholder

    // store tile references for multi step actions
    private Tile _storeTile;
    private Tile _storeSecondTile;
    private List<DVillager> _drawnVillagersThisTurn = new List<DVillager>(); // TODO (metul): Make networked variable?
    // Use properties for network variable update
    public Tile StoreTile
    {
        get => _storeTile;
        set
        {
            if (_storeTile != value) // TODO (metul): Override Equals()?
            {
                _storeTile = value;
                // MARK: Update tile if connected, supports local debugging
                if ((NetworkManager.Singleton?.IsConnectedClient).GetValueOrDefault())
                {
                    NetworkLog.LogInfoServer($"Store Tile set to {_storeTile}.");
                    NetworkFieldManager.Instance.NetworkStoreTile.Value = value;
                }
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
                {
                    NetworkLog.LogInfoServer($"Second Store Tile set to {_storeSecondTile}.");
                    NetworkFieldManager.Instance.NetworkStoreSecondTile.Value = value;
                }
            }
        }
    }

    #endregion

    #region Setup

    private void Awake()
    {
        _field = GetField(_rows);
        _battleField = GetField(_squads); // TODO (metul): Do not register battlefield tiles (or register in a separate container)
        _tempStorageTile = _tempStorage.GetComponent<Tile>();
    }

    private void Start()
    {
        PopulateField();
        StateManager.OnStateUpdate += state => NetworkedUpdateInteractability();
    }

    private Tile[][] GetField(Transform[] sourceArray)
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
                tiles[j].ID = new Vector2(i, j);
                ObjectManager.Instance.Register(tiles[j].ID, tiles[j]);
            }
        }

        return field;
    }

    public Tile TileByPosition(DPosition position)
    {
        return (position == null) ? _tempStorageTile : _field[position.Row - 1][position.Col - 1];
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

    public void Authorize(Tile tile, DWorker worker)
    {
        CommandProcessor.Instance.ExecuteCommand(
            new AuthorizeCommand(0, GameState.Instance.TurnPlayer().Id, worker, tile.Meeples[0].Core)
        );
    }

    public void Swap(Tile tile1, Tile tile2, DWorker worker)
    {
        CommandProcessor.Instance.ExecuteCommand(
            new SwapCommand(
                0, // --> TODO: should this be zero?
                GameState.Instance.TurnPlayer().Id,
                worker,
                GameState.Instance.AtPosition(tile1.Position),
                GameState.Instance.AtPosition(tile2.Position)
            )
        );
    }

    public void RiotStep(Tile from, Tile to)
    {
        var knight = (DKnight) GameState.Instance.AllAtPosition(from.Position, m => m.GetType() == typeof(DKnight))[0];
        CommandProcessor.Instance.ExecuteCommand(
            new RiotStepCommand(
                0,
                GameState.Instance.TurnPlayer().Id,
                knight,
                to.Position
            )
        );
    }

    public void StartRiot(Tile tile, DWorker worker)
    {
        var meeple = GameState.Instance.AtPosition(tile.Position);
        CommandProcessor.Instance.ExecuteCommand(new StartRiotCommand(
            0,
            GameState.Instance.TurnPlayer().Id,
            worker,
            meeple
        ));
    }

    public void Revive(Tile tile, DWorker worker)
    {
        //var worker = new DWorker(GameState.Instance.TurnPlayer().Id);
        CommandProcessor.Instance.ExecuteCommand(new ReviveCommand(0, GameState.Instance.TurnPlayer().Id, worker,
            tile.Meeples[0].Core));
    }

    public void Retreat(Tile battlefrontTile, Tile tile)
    {
        CommandProcessor.Instance.ExecuteCommand(new RetreatCommand(
            0,
            battlefrontTile.Meeples[0].Core as DKnight,
            tile.Position
        ));
    }

    public void Villager(Tile tile)
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
    }

    public void MoveMeeple(Tile from, Tile to)
    {
        CommandProcessor.Instance.ExecuteCommand(new MoveVillagerCommand(0, from.Meeples[0].Core, to.Position));
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
                StoreTile = tile;
                StateManager.CurrentState = StateManager.State.PayForAction;
                break;
            //TODO: Can we add a case for RiotChooseFollowerType
            case StateManager.State.RiotChoosePath:
                StoreSecondTile = StoreTile;
                StoreTile = tile;
                RiotStep(StoreSecondTile, StoreTile);

                if (tile.Position.IsFinal)
                    StateManager.CurrentState = StateManager.State.Default;
                else
                    StateManager.CurrentState = StateManager.State.RiotChoosePath;

                break;
            case StateManager.State.Revive:
                StoreTile = tile;
                StateManager.CurrentState = StateManager.State.PayForAction;
                break;
            case StateManager.State.RetreatChooseTile:
                Retreat(StoreTile, tile);
                StoreSecondTile = null;
                StateManager.CurrentState = StateManager.State.Default;
                break;
            case StateManager.State.RetreatChooseKnight:
                StoreTile = tile;
                StateManager.CurrentState = StateManager.State.RetreatChooseTile;
                break;
            case StateManager.State.Villager:
                Villager(tile);
                StateManager.CurrentState = StateManager.State.Default;
                break;
            case StateManager.State.MoveMeeple:
                MoveMeeple(StoreTile, tile);
                StateManager.CurrentState = StateManager.State.Default;
                StoreTile = null;
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
        switch (action)
        {
            case StateManager.State.Authorize:
                Authorize(StoreTile, worker);
                StoreTile = null;
                StateManager.CurrentState = StateManager.State.Default;
                break;
            case StateManager.State.Swap2:
                Swap(StoreTile, StoreSecondTile, worker);
                StoreTile = StoreSecondTile = null;
                StateManager.CurrentState = StateManager.State.Default;
                break;
            case StateManager.State.Revive:
                Revive(StoreTile, worker);
                StoreTile = null;
                StateManager.CurrentState = StateManager.State.Default;
                break;
            case StateManager.State.RiotChooseKnight:
                StartRiot(StoreTile, worker);
                StateManager.CurrentState = StateManager.State.RiotChoosePath;
                break;
        }
    }

    public void NetworkedUpdateInteractability()
    {
        if ((NetworkManager.Singleton?.IsConnectedClient).GetValueOrDefault() && (!NetworkManager.Singleton?.IsServer).GetValueOrDefault())
        {
            //NetworkLog.LogInfoServer($"NetworkedUpdateInteractability: {NetworkManager.Singleton.LocalClientId}");
            if (PlayerManager.Instance.NetworkPlayerIDs[NetworkManager.Singleton.LocalClientId] == GameState.Instance.TurnPlayer().Id)
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
            case StateManager.State.Riot:
                EnableInjuryBased(false);
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
            tile.Interactable = StoreSecondTile.Position.Neighbors(tile.Position) && // TODO: StoreSecondTile is probably null
            (!overwriteInjured || GameState.Instance.HealthyMeepleAtPosition(p));
            Debug.Log($"Tile {tile.Position}: {tile.Interactable}");
        });
    }

    private void EnableRetreatable()
    {
        GameState.Instance.TraverseBoard(p => 
        {
            var tile = TileByPosition(p);
            tile.Interactable = 
                GameState.Instance.IsEmpty(p) &&
                GameState.Instance.PathExists(
                    DPosition.LastRow(),
                    p,
                    p => GameState.Instance.IsEmpty(p)
                );
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
                        (endguy == null || (endguy.IsHealthy() && endguy.GetType() != typeof(DKnight))) &&
                        GameState.Instance.PathExists(
                            p,
                            endpoint,
                            _p => {
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
                meep != null && meep.IsHealthy() &&  // --> there is a healthy meeple at this position
                (
                    p.Equals(endpoint) || (          // --> this position is endpoint itself, or ...
                    endguy == null &&                // --> endpoint is empty
                    GameState.Instance.PathExists(   // --> and there is a path
                        p,                           // --> from this point
                        endpoint,                    // --> to the endpoint
                        _p => p.Equals(_p)
                            || GameState.Instance.IsEmpty(_p) // --> where the only non-empty step is the first one
                    )
                    )
                );
        });
    }

    private void EnableRiotPath()
    {
        var last = StoreTile.Position;

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
        StoreTile = null;
        StoreSecondTile = null;
        _drawnVillagersThisTurn.Clear();

        NetworkedUpdateInteractability();
    }
    
    #endregion
}