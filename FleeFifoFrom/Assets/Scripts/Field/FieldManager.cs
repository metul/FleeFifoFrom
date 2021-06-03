using System;
using System.Collections;
using System.Collections.Generic;
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
    private List<Tile> _storeRiotPath;

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
        PopulateFieldRandomly();
        PopulateBattlefield();
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

    private void PopulateFieldRandomly()
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

    private void PopulateBattlefield()
    {
        var vanguard = GameState.Instance.Vanguard();
        ushort cursor = 0;

        foreach (var tiles in _battleField)
        {
            foreach (var tile in tiles)
            {
                if (cursor >= vanguard.Length) {
                    return;
                }

                var knight = Instantiate(_knightPrefab);
                knight.Initialize(vanguard[cursor], this);
                knight.SetTo(tile);
                cursor++;
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

    public void Swap(Tile tile1, Tile tile2)
    {
        var worker = new DWorker(GameState.Instance.TurnPlayer().Id);
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
        CommandProcessor.Instance.ExecuteCommand(new ReviveCommand(0,GameState.Instance.TurnPlayer().Id, worker, tile.Meeples[0].Core));
    }

    public void Reprioritize(Tile tile)
    {
        CommandProcessor.Instance.ExecuteCommand(new ReprioritizeCommand(0, tile));
    }

    public void Retreat(Tile battlefrontTile, Tile tile)
    {
        CommandProcessor.Instance.ExecuteCommand(new RetreatCommand(0, battlefrontTile, tile));
    }

    public void Villager(Meeple villager, Tile tile)
    {
        CommandProcessor.Instance.ExecuteCommand(new DrawVillagerCommand(0, villager, tile));
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
            case StateManager.State.Riot:
                // _storeRiotPath = new List<Tile>();
                // _storeRiotPath.Add(tile);
                // Riot(_storeRiotPath);
                // StateManager.GameState = StateManager.State.Default;
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
                {
                    StateManager.GameState = StateManager.State.Default;
                } else {
                    StateManager.GameState = StateManager.State.RiotChoosePath;
                }
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
                _storeSecondTile = tile;
                StateManager.GameState = StateManager.State.RetreatChooseKnight;
                break;
            case StateManager.State.RetreatChooseKnight:
                Retreat(tile, _storeSecondTile);
                _storeSecondTile = null;
                StateManager.GameState = StateManager.State.Default;
                break;
            case StateManager.State.Villager:
                Villager(Instantiate(GetRandomVillagerPrefab()), tile);
                StateManager.GameState = StateManager.State.Default;
                break;
        }
    }

    public void InvokeAction(StateManager.State action, DWorker worker)
    {
        //Debug.Log(action);
        switch (action)
        {
            case StateManager.State.Authorize:
                Authorize(_storeTile, worker);
                _storeTile = null;
                StateManager.GameState = StateManager.State.Default;
                break;
            case StateManager.State.Swap2:
                Swap(_storeTile, _storeSecondTile);
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
                EnableInjuryBased(false);
                // TODO replace with EnableNeighbours
                _storeSecondTile.Interactable = false;
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
                _storeRiotPath = null;
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

    #endregion

    // TODO move villager generation to game manager?
    // TODO will eventually need to seed villagers with a specific probability
    // TODO can use random values for now
    private Meeple GetRandomVillagerPrefab()
    {
        return _villagerPrefabs[Random.Range(0, _villagerPrefabs.Length)];
    }
}