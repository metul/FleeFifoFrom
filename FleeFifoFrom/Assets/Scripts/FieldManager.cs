using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoBehaviour
{
    [SerializeField] private Transform[] _rows;
    [SerializeField] private Meeple[] _meeplePrefabs;
    private Tile[][] _field;

    public enum DebugGameState
    {
        Default, Authorize, Swap, Riot, Revive, Reprioritize, Retreat, Villager
    }

    public DebugGameState CurrentGameState;

    private void Awake()
    {
        InitField();
        PopulateFieldRandomly();
    }

    private void InitField()
    {
        // init field reference
        _field = new Tile[_rows.Length][];
        for (var i = 0; i < _rows.Length; i++)
        {
            var tiles = _rows[i].GetComponentsInChildren<Tile>();
            _field[i] = tiles;

            for (var j = 0; j < tiles.Length; j++)
            {
                tiles[j].ID = new Vector2(i, j);

                // DEBUG set tiles to interactable
                tiles[j].Interactable = true;
            }
        }
    }
    
    private void PopulateFieldRandomly()
    {
        if (_field == null)
            InitField();

        foreach (var tiles in _field)
        {
            foreach (var tile in tiles)
            {
                var meeple = Instantiate(_meeplePrefabs[Random.Range(0, _meeplePrefabs.Length - 1)]);
                tile.SetMeeple(meeple);
            }
        }
    }

    public void OnTileClicked(Tile tile)
    {
        switch (CurrentGameState)
        {
            case DebugGameState.Default:
                Debug.Log($"Tile {tile.ID} has been klicked");
                break;
            case DebugGameState.Authorize:
                Authorize(tile);
                break;
            case DebugGameState.Swap:
                Swap(tile, _field[0][0]);
                break;
            case DebugGameState.Riot:
                Riot(new []{tile});
                break;
            case DebugGameState.Revive:
                Revive(tile);
                break;
            case DebugGameState.Reprioritize:
                Reprioritize(tile);
                break;
            case DebugGameState.Retreat:
                foreach (var meeplePrefab in _meeplePrefabs)
                {
                    if (meeplePrefab.GetType() == typeof(Knight))
                    {
                        var knight = (Knight) Instantiate(meeplePrefab);
                        Retreat(knight, tile);
                    }
                }
                break;
            case DebugGameState.Villager:
                var commoner = Instantiate(_meeplePrefabs[1]);
                Villager(commoner, tile);
                break;
        }
    }

    public void Authorize(Tile tile)
    {
        var meeple = tile.RemoveMeeple();
        
        // Debug
        Debug.Log($"Authorize piece: {meeple}");
        Destroy(meeple.gameObject);
    }

    public void Swap(Tile tile1, Tile tile2)
    {
        var meeple1 = tile1.RemoveMeeple();
        var meeple2 = tile2.RemoveMeeple();
        tile1.SetMeeple(meeple2);
        tile2.SetMeeple(meeple1);
    }

    public void Riot(Tile[] path)
    {
        // Debug
        for (var i = 0; i < path.Length; i++)
        {
            var tile = path[i];
            tile.Meeple.CurrentState = Meeple.State.Injured;
        }
    }

    public void Revive(Tile tile)
    {
        // TODO which state after revival?
        tile.Meeple.CurrentState = Meeple.State.Default;
    }

    public void Reprioritize(Tile tile)
    {
        tile.Meeple.CurrentState = tile.Meeple.CurrentState == Meeple.State.Default
            ? Meeple.State.Tapped
            : Meeple.State.Default;
    }

    public void Retreat(Knight knight, Tile tile)
    {
        tile.SetMeeple(knight);
    }

    public void Villager(Meeple villager, Tile tile)
    {
        tile.SetMeeple(villager);
    }
}