using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideState : MonoBehaviour
{
    private CubeMaster cubeMaster;
    public GameObject Cover;

    public bool isActivated;

    public enum Orientation
    {
        FRONT,
        RIGHT,
        LEFT,
        BACK,
        TOP
    }

    public Orientation orientation;

    public Transform PlayerTransform;

    public Vector2Int PlayerPos;

    public TileSetter[,] tiles = new TileSetter[5,5];

    // Start is called before the first frame update
    void Awake()
    {
        isActivated = false;
        Cover.gameObject.SetActive(true);

        cubeMaster = FindObjectOfType<CubeMaster>();

        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                tiles[x, y] = gameObject.transform.Find(x.ToString() + y.ToString()).GetComponent<TileSetter>();
                tiles[x, y].UpdateMaterial();
                tiles[x, y].ParentState = this;
                tiles[x, y].location = new Vector2Int(x, y);
            }
        }
    }

    public void Activate()
    {
        isActivated = true;
        Cover.gameObject.SetActive(false);
    }

    public TileSetter GetTile(Vector2Int pos)
    {

        if (isActivated)
            return tiles[pos.x, pos.y];
        else
            return null;
    }

    private void Start()
    {
        SetPlayerPos(PlayerPos);
    }

    public void PressedCell(TileSetter tile)
    {
        if (!isActivated)
            return;

        cubeMaster.TrySetPlayerPos(tile.location, orientation);
    }

    public void SetPlayerPos(Vector2Int pos)
    {
        PlayerPos = pos;
        PlayerTransform.localPosition = new Vector3(pos.x - 2f, PlayerTransform.localPosition.y, -pos.y + 2f);
    }
}
