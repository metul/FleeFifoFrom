using System;
using System.Collections;
using UnityEngine;

public abstract class Meeple : MonoBehaviour
{
    private static readonly int ANIM_RIOTING = Animator.StringToHash("rioting");
    protected const float CLOSE_ENOUGH_SNAP_VALUE = 0.01f;
    protected const float TOO_FAR_SNAP_VALUE = 500f;
    
    [SerializeField] protected Renderer _renderer;
    [SerializeField] protected Animator _animator;
    [SerializeField] protected float _movementSpeed = 1f;
    
    public DMeeple Core { get; private set; }
    
    protected Coroutine CurrentMovement;

    protected Tile _tile;
    protected FieldManager _fieldManager;
    protected Transform _transform;
    protected Color _defaultColor;

    protected virtual void Start()
    {
        _defaultColor = _renderer.material.color;
    }

    public virtual void Initialize(DMeeple core, FieldManager fieldManager)
    {
        Core = core;
        _fieldManager = fieldManager;
        _transform = transform;

        SetTo(Core.Position.Current);
        core.Position.OnChange += pos => SetTo(pos, false);
        core.IsRioting.OnChange += ToggleRiotAnimation;
    }

    protected void SetColor(Color color)
    {
        _renderer.material.color = color;
    }

    public void Initialize(DMeeple core)
    {
        Initialize(core, FindObjectOfType<FieldManager>());
    }

    public void SetTo(Tile tile, bool instantly = true)
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
            // _transform.localPosition = new Vector3
            var offset = new Vector3
            (((float) Math.Cos(angle)) * .6f,
                0,
                -((float) Math.Sin(angle)) * .6f
            );

            if (instantly)
                _transform.localPosition = offset;
            else
            {
                if (CurrentMovement != null)
                    StopCoroutine(CurrentMovement);
                CurrentMovement = StartCoroutine(MoveTo(tile.Transform.position + offset));
            }
        }
        else
        {
            if (instantly)
                _transform.localPosition = Vector3.zero;
            else
            {
                if (CurrentMovement != null)
                    StopCoroutine(CurrentMovement);
                CurrentMovement = StartCoroutine(MoveTo(tile.Transform.position));
            }
        }
    }

    protected virtual void SetTo(DPosition position, bool instantly = true)
    {
        SetTo(_fieldManager.TileByStateAndPosition(Core.State, position), instantly);
    }

    protected IEnumerator MoveTo(Vector3 position)
    {
        while ((_transform.position - position).magnitude > CLOSE_ENOUGH_SNAP_VALUE
               && (_transform.position - position).magnitude < TOO_FAR_SNAP_VALUE)
        {
            _transform.position = Vector3.Lerp(_transform.position, position, _movementSpeed * Time.deltaTime);
            yield return null;
        }

        _transform.position = position;
    }
    
    protected void ToggleRiotAnimation(bool isRioting)
    {
        _animator.SetBool(ANIM_RIOTING, isRioting);
    }
}