using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapController : MonoBehaviour
{
    private Vector3Int previousTilePosition = new Vector3Int(100, 100, 0);
    Vector3Int cellPosition = new Vector3Int(0, 0, 0);
    public Tilemap tilemap;
    public Tile hoveringTile;
    public Tile normalTile;
    private Tile previousTile;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cellPosition = tilemap.WorldToCell(mouseWorldPos);
        cellPosition.z = 0;
    }
}
