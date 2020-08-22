using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityStandardAssets.Cameras;

public class CubeMaster : MonoBehaviour
{
    public SideState Front;
    public SideState Right;
    public SideState Left;
    public SideState Back;
    public SideState Top;

    public AudioSource Step;
    public AudioSource Blocked;
    public AudioSource KeyCollected;
    public AudioSource FlowerBloom;
    public AudioSource Soundtrack;

    public BottomCube Bottom;

    public ParticleSystem plantParticles;

    private bool hasClickedSomewhere;

    private FreeLookCam _cam;

    private MouseHint _mouseHint;

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
        _cam = FindObjectOfType<FreeLookCam>();
        UpdatePlayerPos();
        GoToNextState(); // to front
        _mouseHint = FindObjectOfType<MouseHint>();
        Invoke("CheckIfClickPromptNecessary", 2f);
    }

    private void CheckIfClickPromptNecessary()
    {
        if (!hasClickedSomewhere)
        {
            _mouseHint.ShowingState = MouseHint.SpriteState.LEFTCLICK;
        }
    }

    public void TrySetPlayerPos(Vector2Int pos, SideState.Orientation orientation)
    {
        if (!hasClickedSomewhere)
        {
            _mouseHint.ShowingState = MouseHint.SpriteState.NOCLICK;
        }
        hasClickedSomewhere = true;


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


        if (nextToPlayer)
        {
            if (!oneOfTilesIs(getTiles(newPlayerPos), TileSetter.TileState.FILLED))
            {
                playerPos = newPlayerPos;
                if (oneOfTilesIs(getTiles(newPlayerPos), TileSetter.TileState.KEY))
                {
                    CollectedKey();
                    foreach (var tile in getTiles(newPlayerPos))
                    {
                        if (tile.currentState == TileSetter.TileState.KEY)
                        {
                            tile.currentState = TileSetter.TileState.EMPTY;
                            tile.SetState(tile.currentState);
                        }
                    }
                }
                Step.pitch = Random.Range(0.8f, 1.2f);
                Step.Play();
                UpdatePlayerPos();
            }
            else
            {
                Blocked.Play();
            }
        }
        else
        {
           // nothing
        }

    }

    private void CollectedKey()
    {
        if(mainState != MAIN_STATE.TOP)
        {
            KeyCollected.Play();
        }
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
                Invoke("CheckIfDragPromptNecessary", 2f);
                Soundtrack.PlayDelayed(1f);
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

                Soundtrack.DOFade(0f, 1f).OnComplete(() =>
                {
                    Soundtrack.Stop();
                });

                FindObjectOfType<Flower>().Bloom();
                Camera.main.DOFieldOfView(100, 6f).SetEase(Ease.InOutSine);
                FindObjectOfType<FreeLookCam>().transform.DOMoveY(4f, 6f).SetEase(Ease.InOutSine);
                plantParticles.Play();
                FlowerBloom.Play();

                DOVirtual.DelayedCall(6f, () =>
                {
                    Bottom.Activate();
                    KeyCollected.Play();
                });                break;
            case MAIN_STATE.ENDING:
                break;
        }
        Debug.Log("#### Update to " + mainState);
    }

    private void CheckIfDragPromptNecessary()
    {
        if (!_cam.hasDrageAtLeastOnce)
        {
            _mouseHint.ShowingState = MouseHint.SpriteState.RIGHTCLICK;
        }
    }

    private void Update()
    {
        if(mainState != MAIN_STATE.OPENING && mainState != MAIN_STATE.FRONT)
        {
            if (_cam.hasDrageAtLeastOnce)
            {
                _mouseHint.ShowingState = MouseHint.SpriteState.NOCLICK;
            }
        }
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
