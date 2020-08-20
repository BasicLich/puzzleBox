using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[ExecuteInEditMode]
[RequireComponent(typeof(Renderer))]
public class TileSetter : MonoBehaviour
{
    public enum TileState
    {
        EMPTY,
        KEY,
        FILLED
    }

    public TileState currentState;

    public void SetState(TileState state)
    {
        switch (state)
        {
            case TileState.EMPTY:
                X = 0;
                Y = 0;
                break;
            case TileState.KEY:
                X = 10;
                Y = 10;
                break;
            case TileState.FILLED:
                X = 12;
                Y = 12;
                break;
        }
        UpdateMaterial();
    }

    public SideState ParentState;

    public Color HighlightColor {
        set
        {
            _highlightColor = value;
            UpdateMaterial();
        }
    }
    public Color _highlightColor;

    public Vector2Int location;

    public int X;
    public int Y;

    private void Awake()
    {
        ParentState = transform.parent.GetComponent<SideState>();
    }

    private void OnEnable()
    {
        UpdateMaterial();
    }

    public void UpdateMaterial()
    {
        SetState(currentState);

        MaterialPropertyBlock props = new MaterialPropertyBlock();
        props.SetInt("_TileX", X);
        props.SetInt("_TileY", Y);
        props.SetColor("_HighlightColor", _highlightColor);

        Renderer renderer = GetComponent<Renderer>();
        renderer.SetPropertyBlock(props);
    }

    public void Press()
    {
        Color tempCol = _highlightColor;
        tempCol.a = 0.2f;
        DOTween.To(() => _highlightColor, x => HighlightColor = x, tempCol, 0.2f);
        tempCol.a = 0;
        DOTween.To(() => _highlightColor, x => HighlightColor = x, tempCol, 0.2f).SetDelay(0.2f);

        ParentState.PressedCell(this);
    }
}
