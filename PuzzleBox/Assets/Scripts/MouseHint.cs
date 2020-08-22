using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHint : MonoBehaviour
{
    public SpriteState ShowingState;

    private Renderer _renderer;

    public enum SpriteState
    {
        NOCLICK,
        LEFTCLICK,
        RIGHTCLICK
    }

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        ShowingState = SpriteState.NOCLICK;
    }

    private void Update()
    {
        switch (ShowingState)
        {
            case SpriteState.NOCLICK:
                _renderer.enabled = false;
                break;
            case SpriteState.LEFTCLICK:
                UpdateMaterial(Time.time % 1f > 0.5f ? SpriteState.NOCLICK : SpriteState.LEFTCLICK);
                Vector3 tempPos = transform.localPosition;
                tempPos.x = 0;
                transform.localPosition = tempPos;
                _renderer.enabled = true;
                break;
            case SpriteState.RIGHTCLICK:
                tempPos = transform.localPosition;
                tempPos.x = 1f * (0.5f - (Time.time + 0.2f) % 1f);
                transform.localPosition = tempPos;
                UpdateMaterial(Time.time % 1f > 0.5f ? SpriteState.NOCLICK : SpriteState.RIGHTCLICK);
                _renderer.enabled = true;
                break;
        }
    }

    private void UpdateMaterial(SpriteState state)
    {
        MaterialPropertyBlock props = new MaterialPropertyBlock();
        switch (state)
        {
            case SpriteState.NOCLICK:
                props.SetInt("_TileX", 29);
                props.SetInt("_TileY", 7);
                break;
            case SpriteState.LEFTCLICK:
                props.SetInt("_TileX", 30);
                props.SetInt("_TileY", 7);
                break;
            case SpriteState.RIGHTCLICK:
                props.SetInt("_TileX", 31);
                props.SetInt("_TileY", 7);
                break;
        }

        Renderer renderer = GetComponent<Renderer>();
        renderer.SetPropertyBlock(props);
    }
}
