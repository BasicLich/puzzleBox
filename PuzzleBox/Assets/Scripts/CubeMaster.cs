using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMaster : MonoBehaviour
{
    public SideState Front;
    public SideState Right;
    public SideState Left;
    public SideState Back;

    private Vector3Int playerPos = new Vector3Int(1,2,2); // xy is Front and Back, yz is Left and Right

    public void TrySetPlayerPos(Vector2Int pos, SideState.Orientation orientation)
    {
        bool nextToPlayer;
        switch (orientation)
        {
            case SideState.Orientation.FRONT:
                nextToPlayer = CheckIfNextToPlayer(Front.PlayerPos, pos);
                if (nextToPlayer)
                {
                    playerPos = new Vector3Int(pos.x, pos.y, playerPos.z);
                }
                break;
            case SideState.Orientation.RIGHT:
                nextToPlayer = CheckIfNextToPlayer(Right.PlayerPos, pos);
                if (nextToPlayer)
                {
                    playerPos = new Vector3Int(playerPos.x, pos.y, pos.x);
                }
                break;
            case SideState.Orientation.BACK:
                nextToPlayer = CheckIfNextToPlayer(Back.PlayerPos, pos);
                if (nextToPlayer)
                {
                    playerPos = new Vector3Int(4 - pos.x, pos.y, playerPos.z);
                }
                break;
            case SideState.Orientation.LEFT:
                nextToPlayer = CheckIfNextToPlayer(Left.PlayerPos, pos);
                if (nextToPlayer)
                {
                    playerPos = new Vector3Int(playerPos.x, pos.y, 4-pos.x);
                }
                break;
        }
        UpdatePlayerPos();
    }

    private void Start()
    {
        UpdatePlayerPos();
    }

    private void UpdatePlayerPos()
    {
        Front.SetPlayerPos(new Vector2Int(playerPos.x, playerPos.y));
        Right.SetPlayerPos(new Vector2Int(playerPos.z, playerPos.y));
        Left.SetPlayerPos(new Vector2Int(4 - playerPos.z, playerPos.y));
        Back.SetPlayerPos(new Vector2Int(4 - playerPos.x, playerPos.y));
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
