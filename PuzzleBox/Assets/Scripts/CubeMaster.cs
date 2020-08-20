using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CubeMaster : MonoBehaviour
{
    public SideState Front;
    public SideState Right;
    public SideState Left;
    public SideState Back;
    public SideState Top;

    private enum MAIN_STATE
    {
        OPENING,
        FRONT,
        RIGHT,
        BACK,
        LEFT,
        TOP,
        ENDING
    }
    private MAIN_STATE mainState = MAIN_STATE.OPENING;

    private Vector3Int playerPos = new Vector3Int(1, 2, 2); // xy is Front and Back, yz is Left and Right

    private void Start()
    {
        UpdatePlayerPos();
        GoToNextState(); // to front
    }

    public void TrySetPlayerPos(Vector2Int pos, SideState.Orientation orientation)
    {
        bool nextToPlayer = false;
        Vector3Int newPlayerPos = Vector3Int.zero;
        switch (orientation)
        {
            case SideState.Orientation.FRONT:
                nextToPlayer = CheckIfNextToPlayer(Front.PlayerPos, pos);
                if (nextToPlayer)
                {
                    newPlayerPos = new Vector3Int(pos.x, pos.y, playerPos.z);
                }
                break;
            case SideState.Orientation.RIGHT:
                nextToPlayer = CheckIfNextToPlayer(Right.PlayerPos, pos);
                if (nextToPlayer)
                {
                    newPlayerPos = new Vector3Int(playerPos.x, pos.y, pos.x);
                }
                break;
            case SideState.Orientation.BACK:
                nextToPlayer = CheckIfNextToPlayer(Back.PlayerPos, pos);
                if (nextToPlayer)
                {
                    newPlayerPos = new Vector3Int(4 - pos.x, pos.y, playerPos.z);
                }
                break;
            case SideState.Orientation.LEFT:
                nextToPlayer = CheckIfNextToPlayer(Left.PlayerPos, pos);
                if (nextToPlayer)
                {
                    newPlayerPos = new Vector3Int(playerPos.x, pos.y, 4 - pos.x);
                }
                break;
            case SideState.Orientation.TOP:
                nextToPlayer = CheckIfNextToPlayer(Top.PlayerPos, pos);
                if (nextToPlayer)
                {
                    newPlayerPos = new Vector3Int(pos.x, playerPos.y, 4 - pos.y);
                }
                break;
        }

        if (nextToPlayer && !oneOfTilesIs(getTiles(newPlayerPos), TileSetter.TileState.FILLED))
        {
            playerPos = newPlayerPos;
            if(oneOfTilesIs(getTiles(newPlayerPos), TileSetter.TileState.KEY))
            {
                CollectedKey();
                foreach(var tile in getTiles(newPlayerPos))
                {
                    if(tile.currentState == TileSetter.TileState.KEY)
                    {
                        tile.currentState = TileSetter.TileState.EMPTY;
                        tile.SetState(tile.currentState);
                    }
                }
            }
            UpdatePlayerPos();
        }

    }

    private void CollectedKey()
    {
        GoToNextState();
    }

    private void GoToNextState()
    {
        switch (mainState) // previous state
        {
            case MAIN_STATE.OPENING:
                Front.Activate();
                mainState = MAIN_STATE.FRONT;
                break;
            case MAIN_STATE.FRONT:
                Right.Activate();
                mainState = MAIN_STATE.RIGHT;
                break;
            case MAIN_STATE.RIGHT:
                Back.Activate();
                mainState = MAIN_STATE.BACK;
                break;
            case MAIN_STATE.BACK:
                Left.Activate();
                mainState = MAIN_STATE.LEFT;
                break;
            case MAIN_STATE.LEFT:
                Top.Activate();
                mainState = MAIN_STATE.TOP;
                break;
            case MAIN_STATE.TOP:
                FindObjectOfType<Flower>().Bloom();
                Camera.main.DOFieldOfView(100, 6f).SetEase(Ease.InOutSine);
                break;
            case MAIN_STATE.ENDING:
                break;
        }
        Debug.Log("#### Update to " + mainState);
    }

    private TileSetter[] getTiles(Vector3Int pos)
    {
        var result = new List<TileSetter>();

        var frontTile = Front.GetTile(new Vector2Int(pos.x, pos.y));
        if (frontTile != null)
            result.Add(frontTile);
        var rightTile = Right.GetTile(new Vector2Int(pos.z, pos.y));
        if (rightTile != null)
            result.Add(rightTile);
        var backTile = Back.GetTile(new Vector2Int(4 - pos.x, pos.y));
        if (backTile != null)
            result.Add(backTile);
        var leftTile = Left.GetTile(new Vector2Int(4 - pos.z, pos.y));
        if (leftTile != null)
            result.Add(leftTile);
        var topTile = Top.GetTile(new Vector2Int(pos.x, pos.z));
        if (topTile != null)
            result.Add(topTile);

        return result.ToArray();
    }

    private bool oneOfTilesIs(TileSetter[] tiles, TileSetter.TileState askedState)
    {
        foreach(var tile in tiles)
        {
            if(tile.currentState == askedState)
            {
                return true;
            }
        }
        return false;
    }

    private void UpdatePlayerPos()
    {
        Front.SetPlayerPos(new Vector2Int(playerPos.x, playerPos.y));
        Right.SetPlayerPos(new Vector2Int(playerPos.z, playerPos.y));
        Left.SetPlayerPos(new Vector2Int(4 - playerPos.z, playerPos.y));
        Back.SetPlayerPos(new Vector2Int(4 - playerPos.x, playerPos.y));
        Top.SetPlayerPos(new Vector2Int(playerPos.x, 4-playerPos.z));
    }

    private bool CheckIfNextToPlayer(Vector2Int currentPos, Vector2Int newPos)
    {
        Vector2Int delta = newPos - currentPos;

        bool isMovingOneOnX = delta.x == 1 || delta.x == -1;
        bool isMovingOneOnY = delta.y == 1 || delta.y == -1;

        bool isMovingOnX = delta.x != 0;
        bool isMovingOnY = delta.y != 0;

        return (isMovingOneOnX && !isMovingOnY) ^ (isMovingOneOnY && !isMovingOnX);
    }
}
