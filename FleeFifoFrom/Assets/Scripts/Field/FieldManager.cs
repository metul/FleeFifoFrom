using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;
using Random = UnityEngine.Random;

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
    private List<Tile> _storeRiotPath = new List<Tile>();
    private List<DVillager> _drawnVillagersThisTurn = new List<DVillager>();

    #endregion

    #region Setup

    private void Awake()
    {
        _field = GetField(_rows);
        _battleField = GetField(_squads);
        _tempStorageTile = _tempStorage.GetComponent<Tile>();
    }

    private void Start()
    {
        PopulateField();
        StateManager.OnStateUpdate += UpdateInteractability;
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
                if (meep.GetType() == typeof(DCommoner))
                {
                    prefab = _villagerPrefabs[0];
                }
                else if (meep.GetType() == typeof(DChild))
                {
                    prefab = _villagerPrefabs[1];
                }
                else if (meep.GetType() == typeof(DElder))
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

    public void Reprioritize(Tile tile)
    {
        CommandProcessor.Instance.ExecuteCommand(new ReprioritizeCommand(0, tile));
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

    #endregion

    #region Interaction (state-depending)

    public void ProcessClickedTile(Tile tile)
    {
        switch (StateManager.GameState)
        {
            case StateManager.State.Authorize:
                _storeTile = tile;
                StateManager.GameState = StateManager.State.PayForAction;
                break;
            case StateManager.State.Swap1:
                _storeSecondTile = tile;
                StateManager.GameState = StateManager.State.Swap2;
                break;
            case StateManager.State.Swap2:
                _storeTile = tile;
                StateManager.GameState = StateManager.State.PayForAction;
                break;
            case StateManager.State.RiotChooseKnight:
                _storeTile = tile;
                StateManager.GameState = StateManager.State.PayForAction;
                break;
            case StateManager.State.RiotChoosePath:
                _storeSecondTile = _storeTile;
                _storeTile = tile;
                RiotStep(_storeSecondTile, _storeTile);

                if (tile.Position.IsFinal)
                    StateManager.GameState = StateManager.State.Default;
                else
                    StateManager.GameState = StateManager.State.RiotChoosePath;

                break;
            case StateManager.State.Revive:
                _storeTile = tile;
                StateManager.GameState = StateManager.State.PayForAction;
                break;
            case StateManager.State.Reprioritize:
                Reprioritize(tile);
                StateManager.GameState = StateManager.State.Default;
                break;
            case StateManager.State.RetreatChooseTile:
                Retreat(_storeTile, tile);
                _storeSecondTile = null;
                StateManager.GameState = StateManager.State.Default;
                break;
            case StateManager.State.RetreatChooseKnight:
                _storeTile = tile;
                StateManager.GameState = StateManager.State.RetreatChooseTile;
                break;
            case StateManager.State.Villager:
                Villager(tile);
                StateManager.GameState = StateManager.State.Default;
                break;
        }
    }

    public void InvokeAction(StateManager.State action, DWorker worker)
    {
        switch (action)
        {
            case StateManager.State.Authorize:
                Authorize(_storeTile, worker);
                _storeTile = null;
                StateManager.GameState = StateManager.State.Default;
                break;
            case StateManager.State.Swap2:
                Swap(_storeTile, _storeSecondTile, worker);
                _storeTile = _storeSecondTile = null;
                StateManager.GameState = StateManager.State.Default;
                break;
            case StateManager.State.Revive:
                Revive(_storeTile, worker);
                _storeTile = null;
                StateManager.GameState = StateManager.State.Default;
                break;
            case StateManager.State.RiotChooseKnight:
                StartRiot(_storeTile, worker);
                StateManager.GameState = StateManager.State.RiotChoosePath;
                break;
        }
    }


    public void UpdateInteractability()
    {
        switch (StateManager.GameState)
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
                EnableKnights();
                break;
            case StateManager.State.RiotChoosePath:
                EnableRiotPath();
                break;
            case StateManager.State.Revive:
                EnableInjuryBased(true);
                break;
            case StateManager.State.Reprioritize:
                EnableInjuryBased(false);
                break;
            case StateManager.State.RetreatChooseTile:
                EnableEmptyTiles();
                break;
            case StateManager.State.RetreatChooseKnight:
                EnableBattlefield();
                break;
            case StateManager.State.Villager:
                EnableEmptyTiles();
                break;
            case StateManager.State.Default:
                _storeTile = null;
                _storeSecondTile = null;
                _storeRiotPath.Clear();
                DisableAllTiles();
                DisableBattlefield();
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
            (!overwriteInjured || GameState.Instance.HealthyMeepleAtPosition(p));
            Debug.Log($"Tile {tile.Position}: {tile.Interactable}");
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

    private void EnableAuthorizable()
    {
        bool nonInjuredInPreviousRow = false;
        bool nonInjuredInCurrentRow = false;

        GameState.Instance.TraverseBoard(p =>
        {
            // TODO replace with pathfinding / inFrontOf helper function
            var tile = TileByPosition(p);

            if (nonInjuredInPreviousRow)
                tile.Interactable = false;
            else
            {
                // check at first pos if there is something in the previous row
                if (p.Col == 1 && p.Row != 1)
                {
                    nonInjuredInPreviousRow = nonInjuredInCurrentRow;
                    nonInjuredInCurrentRow = false;

                    if (nonInjuredInPreviousRow)
                    {
                        tile.Interactable = false;
                        return;
                    }
                }

                // check meeple
                DMeeple meeple = GameState.Instance.AtPosition(p);
                if (meeple != null)
                {
                    var healthy = GameState.Instance.HealthyMeepleAtPosition(p);
                    tile.Interactable = healthy;
                    nonInjuredInCurrentRow |= healthy;
                }
                else
                {
                    tile.Interactable = false;
                }
            }
        });
    }

    private void EnableRiotPath()
    {
        var last = _storeTile.Position;

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
        _storeRiotPath.Clear();
        _drawnVillagersThisTurn.Clear();
    }
    
    #endregion
}