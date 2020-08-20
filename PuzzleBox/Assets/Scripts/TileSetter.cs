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
        PLAYER,
        KEY,
        FILLED,
        GRASS,
        DIRT
    }

    public TileState currentState;

    public void SetState(TileState state)
    {
        switch (state)
        {
            case TileState.PLAYER:
                X = 19;
                Y = 14;
                break;
            case TileState.EMPTY:
                X = 0;
                Y = 0;
                break;
            case TileState.KEY:
                X = 34;
                Y = 11;
                break;
            case TileState.FILLED:
                X = 0;
                Y = 21;
                break;
            case TileState.GRASS:
                X = 6;
                Y = 22;
                break;
            case TileState.DIRT:
                X = 2;
                Y = 22;
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
        SetState(currentState);
    }

    public void UpdateMaterial()
    {
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
