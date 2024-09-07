using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MouseAction : MonoBehaviour
{
    private Vector3Int previousTilePosition = new Vector3Int(100, 100, 0);
    public Tilemap tilemap;
    public Tile hoveringTile;
    public Tile normalTile;
    // Update is called once per frame
    void Update()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = tilemap.WorldToCell(mouseWorldPos);
        cellPosition.z = 0;
        tilemap.SetTile(cellPosition, hoveringTile);
        if(cellPosition != previousTilePosition)
        {

        }
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Tile position: " + cellPosition);
        }
    }
}
