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
    
    private Tile[][] _field;
    private Tile[][] _battleField;

    #region Placeholder

    // store tile references for multi step actions
    private Tile _storeTile;
    private List<Tile> _storeRiotPath;

    #endregion

    #region Setup
    
    private void Awake()
    {
        _field = GetField(_rows);
        _battleField = GetField(_squads);
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
    

    public Tile? TileByPosition(DPosition position)
    {
        return _field[position.Row - 1][position.Col - 1];
    } 
    private void PopulateFieldRandomly()
    {
        var meeple = GameState.Instance.MatchingMeepleOnBoard(m => true);
        foreach (var meep in meeple)
        {
            var tile = TileByPosition(meep.Position);
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

                if (prefab != null)
                {
                    Meeple newguy = Instantiate(prefab);
                    newguy.Core = meep;
                    tile.SetMeeple(newguy);
                }
            }
        }
    }

    private void PopulateBattlefield()
    {
        foreach (var tiles in _battleField)
        {
            foreach (var tile in tiles)
            {
                tile.SetMeeple(Instantiate(_knightPrefab));
            }
        }
    }
    
    #endregion

    #region Field Actions

    // TODO: All issuer IDs are 0 by default, use actual client ID after network integration
    
    public void Authorize(Tile tile)
    {
        CommandProcessor.Instance.ExecuteCommand(new AuthorizeCommand(0, tile));
    }

    public void Swap(Tile tile1, Tile tile2)
    {
        CommandProcessor.Instance.ExecuteCommand(new SwapCommand(0, tile1, tile2));
    }

    public void Riot(List<Tile> path)
    {
        CommandProcessor.Instance.ExecuteCommand(new RiotCommand(0, path));
    }

    public void Revive(Tile tile)
    {
        CommandProcessor.Instance.ExecuteCommand(new ReviveCommand(0, tile));
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
                Authorize(tile);
                StateManager.GameState = StateManager.State.Default;
                break;
            case StateManager.State.Swap1:
                _storeTile = tile;
                StateManager.GameState = StateManager.State.Swap2;
                break;
            case StateManager.State.Swap2:
                Swap(tile, _storeTile);
                _storeTile = null;
                StateManager.GameState = StateManager.State.Default;
                break;
            case StateManager.State.Riot:
                _storeRiotPath = new List<Tile>();
                _storeRiotPath.Add(tile);
                Riot(_storeRiotPath);
                StateManager.GameState = StateManager.State.Default;
                break;
            case StateManager.State.RiotChooseKnight:
                _storeRiotPath = new List<Tile>();
                _storeRiotPath.Add(tile);
                StateManager.GameState = StateManager.State.RiotChoosePath;
                break;
            case StateManager.State.RiotChoosePath:
                _storeRiotPath.Add(tile);
                if (tile.ID == Vector2.zero)
                {
                    Riot(_storeRiotPath);
                    StateManager.GameState = StateManager.State.Default;
                }
                else
                    StateManager.GameState = StateManager.State.RiotChoosePath;
                break;
            case StateManager.State.Revive:
                Revive(tile);
                StateManager.GameState = StateManager.State.Default;
                break;
            case StateManager.State.Reprioritize:
                Reprioritize(tile);
                StateManager.GameState = StateManager.State.Default;
                break;
            case StateManager.State.RetreatChooseTile:
                _storeTile = tile;
                StateManager.GameState = StateManager.State.RetreatChooseKnight;
                break;
            case StateManager.State.RetreatChooseKnight:
                Retreat(tile, _storeTile);
                _storeTile = null;
                StateManager.GameState = StateManager.State.Default;
                break;
            case StateManager.State.Villager:
                Villager(Instantiate(GetRandomVillagerPrefab()), tile);
                StateManager.GameState = StateManager.State.Default;
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
                _storeTile.Interactable = false;
                break;
            case StateManager.State.Riot:
                EnableInjuryBased(false);
                break;
            case StateManager.State.RiotChooseKnight:
                EnableKnights();
                break;
            case StateManager.State.RiotChoosePath:
                EnableRiotPath(_storeRiotPath);
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
                tile.Interactable = (tile.Meeple == null);
            }
        }
    }

    private void EnableInjuryBased(bool enableInjured)
    {
        GameState.Instance.TraverseBoard(p => {
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
        foreach (var tiles in _field)
        {
            foreach (var tile in tiles)
            {
                tile.Interactable = (tile.Meeple.GetType() == typeof(Knight));
            }
        }
    }

    private void EnableAuthorizable()
    {
        // TODO enable interaction for authorize
        // TODO: switch this to read game state
        List<int> lastEmpty = new List<int>(); 
        List<int> newEmpty = new List<int>();
        for (var i = 0; i < _field.Length; i++)
        {
            var fields = _field[i];
            for (var j = 0; j < fields.Length; j++)
            {
                var tile = fields[j];
                        
                // not empty
                if (tile.Meeple != null)
                {
                    // injured
                    // if (tile.Meeple.CurrentState == Meeple.State.Injured)
                    //     tile.Interactable = false;
                            
                    // top tile
                    if (i == 0)
                        tile.Interactable = true;
                            
                    // previous tile is empty
                    else if (lastEmpty.Contains(j) || lastEmpty.Contains(j - 1))
                        tile.Interactable = true;
                }
                // empty
                else
                {
                    tile.Interactable = false;
                    newEmpty.Add(j);
                }
            }

            // row is fully occupied, abort
            if(newEmpty.Count == 0)
                return;

            lastEmpty = new List<int>(newEmpty);
            newEmpty.Clear();
        }
    }

    private void EnableRiotPath(List<Tile> path)
    {
        var currentID = path[path.Count - 1].ID;
        foreach (var tiles in _battleField)
        {
            foreach (var tile in tiles)
            {
                tile.Interactable = tile.ID.x < currentID.x &&
                                    (tile.ID.y == currentID.y || tile.ID.y == currentID.y - 1);
            }
        }
    }

    private void EnableBattlefield()
    {
        DisableAllTiles();
        foreach (var tiles in _battleField)
        {
            foreach (var tile in tiles)
            {
                tile.Interactable = (tile.Meeple != null);
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