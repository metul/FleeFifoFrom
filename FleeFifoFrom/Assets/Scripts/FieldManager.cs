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
    
    public enum DebugGameState
    {
        Default,
        Authorize,
        Swap1,
        Swap2,
        Riot, // <- current debug state
        RiotChooseKnight,
        RiotChoosePath,
        Revive,
        Reprioritize,
        RetreatChooseTile,
        RetreatChooseKnight,
        Villager,
        ResetTurnSelect,
        ResetTurnMove
    }

    private DebugGameState _gameState;
    public DebugGameState CurrentGameState
    {
        get => _gameState;
        set
        {
            _gameState = value;
            UpdateInteractability();
        }
    }

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
    
    private void PopulateFieldRandomly()
    {
        // TODO consider fixed knight positions
        foreach (var tiles in _field)
        {
            foreach (var tile in tiles)
            {
                tile.SetMeeple(Instantiate(GetRandomVillagerPrefab()));
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
        switch (CurrentGameState)
        {
            case DebugGameState.Default:
                Debug.Log($"Tile {tile.ID} has been klicked");
                break;
            case DebugGameState.Authorize:
                Authorize(tile);
                CurrentGameState = DebugGameState.Default;
                break;
            case DebugGameState.Swap1:
                _storeTile = tile;
                CurrentGameState = DebugGameState.Swap2;
                break;
            case DebugGameState.Swap2:
                Swap(tile, _storeTile);
                _storeTile = null;
                CurrentGameState = DebugGameState.Default;
                break;
            case DebugGameState.Riot:
                _storeRiotPath = new List<Tile>();
                _storeRiotPath.Add(tile);
                Riot(_storeRiotPath);
                CurrentGameState = DebugGameState.Default;
                break;
            case DebugGameState.RiotChooseKnight:
                _storeRiotPath = new List<Tile>();
                _storeRiotPath.Add(tile);
                CurrentGameState = DebugGameState.RiotChoosePath;
                break;
            case DebugGameState.RiotChoosePath:
                _storeRiotPath.Add(tile);
                if (tile.ID == Vector2.zero)
                {
                    Riot(_storeRiotPath);
                    CurrentGameState = DebugGameState.Default;
                }
                else
                    CurrentGameState = DebugGameState.RiotChoosePath;
                break;
            case DebugGameState.Revive:
                Revive(tile);
                CurrentGameState = DebugGameState.Default;
                break;
            case DebugGameState.Reprioritize:
                Reprioritize(tile);
                CurrentGameState = DebugGameState.Default;
                break;
            case DebugGameState.RetreatChooseTile:
                _storeTile = tile;
                CurrentGameState = DebugGameState.RetreatChooseKnight;
                break;
            case DebugGameState.RetreatChooseKnight:
                Retreat(tile, _storeTile);
                _storeTile = null;
                CurrentGameState = DebugGameState.Default;
                break;
            case DebugGameState.Villager:
                Villager(Instantiate(GetRandomVillagerPrefab()), tile);
                CurrentGameState = DebugGameState.Default;
                break;
        }
    }

    public void UpdateInteractability()
    {
        switch (CurrentGameState)
        {
            case DebugGameState.Default:
                DisableAllTiles();
                DisableBattlefield();
                break;
            case DebugGameState.Authorize:
                EnableAuthorizable();
                break;
            case DebugGameState.Swap1:
                EnableInjuryBased(false);
                break;
            case DebugGameState.Swap2:
                EnableInjuryBased(false);
                _storeTile.Interactable = false;
                break;
            case DebugGameState.Riot:
                EnableInjuryBased(false);
                break;
            case DebugGameState.RiotChooseKnight:
                EnableKnights();
                break;
            case DebugGameState.RiotChoosePath:
                EnableRiotPath(_storeRiotPath);
                break;
            case DebugGameState.Revive:
                EnableInjuryBased(true);
                break;
            case DebugGameState.Reprioritize:
                EnableInjuryBased(false);
                break;
            case DebugGameState.RetreatChooseTile:
                EnableEmptyTiles();
                break;
            case DebugGameState.RetreatChooseKnight:
                EnableBattlefield();
                break;
            case DebugGameState.Villager:
                EnableEmptyTiles();
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
        foreach (var tiles in _field)
        {
            foreach (var tile in tiles)
            {
                if (tile.Meeple != null)
                {
                    if(enableInjured)
                        tile.Interactable = tile.Meeple.CurrentState == Meeple.State.Injured;
                    else
                        tile.Interactable = tile.Meeple.CurrentState != Meeple.State.Injured;
                }
                else
                    tile.Interactable = false;
            }
        }
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
                    if (tile.Meeple.CurrentState == Meeple.State.Injured)
                        tile.Interactable = false;
                            
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
    private Meeple GetRandomVillagerPrefab()
    {
        return _villagerPrefabs[Random.Range(0, _villagerPrefabs.Length)];
    }
}