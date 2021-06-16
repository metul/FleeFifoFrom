using System;
using UnityEngine;

public abstract class Meeple: MonoBehaviour
{
    [SerializeField] protected Renderer _renderer;
    public DMeeple Core { get; private set; }

    protected Action OnDefault;

    private Tile _tile;
    private FieldManager _fieldManager;
    private Transform _transform;
    protected Color _defaultColor;
    protected Animator _animator;

    protected virtual void Start()
    {
        _defaultColor = _renderer.material.color;
        _animator = GetComponent<Animator>();
    }

    public virtual void Initialize(DMeeple core, FieldManager fieldManager)
    {
        Core = core;
        _fieldManager = fieldManager;
        _transform = transform;

        SetTo(Core.Position.Current);
        core.Position.OnChange += SetTo;
    }

    protected void SetColor(Color color)
    {
        _renderer.material.color = color;
    }

    public void Initialize(DMeeple core)
    {
        Initialize(core, FindObjectOfType<FieldManager>());
    }

    public void SetTo(Tile tile)
    {
        if (_tile == tile)
            return;

        if (_tile != null)
            _tile.Meeples.Remove(this);
        _tile = tile;
        _tile.Meeples.Add(this);
        _transform.SetParent(_tile.Transform);
        if (_tile.Meeples.Count > 1)
        {
            float angle = (float) _tile.Meeples.Count;
            _transform.localPosition = new Vector3
                (((float) Math.Cos(angle)) * .6f,
                0,
                -((float) Math.Sin(angle)) * .6f
            );
        }
        else
        {
            _transform.localPosition = Vector3.zero;
        }
    }

    protected virtual void SetTo(DPosition position)
    {
        SetTo(_fieldManager.TileByPosition(position));
    }
}
