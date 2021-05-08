using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoBehaviour
{
    [SerializeField] private Transform[] _rows;
    [SerializeField] private Meeple[] _meeplePrefabs;
    private Tile[][] _field;

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

    public void OnTileClicked(Vector2 id, Meeple meeple)
    {
        Debug.Log($"Tile [{id}] has been klicked");

        if (meeple != null)
        {
            meeple.CurrentState = (Meeple.State)(((int) meeple.CurrentState + 1) % 3);
        }
    }
}