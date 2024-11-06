using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    private TilemapController tmCTR;
    private InformationRecorder ir;
    void Start()
    {
        tmCTR = GameObject.FindWithTag("EventSystem").GetComponent<TilemapController>();
        ir = GameObject.FindWithTag("EventSystem").GetComponent<InformationRecorder>();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            ir.UpdateSpaceShips();
            ir.UpdateEnemies();
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cell = tmCTR.tilemap.WorldToCell(mouseWorldPos);
            cell.z = 0;
            foreach(SpaceShip spaceship in ir.GetSpaceShips())
            {
                if (cell == spaceship.GetCell())
                {
                    tmCTR.Clicked(cell, 3, TilemapController.SelectedType.player);
                    spaceship.SetSelected(true);
                    Debug.Log("Spaceship");
                    return;
                }else if(tmCTR.GetTile(cell) == tmCTR.highlightTile && spaceship.GetSelected())
                {
                    spaceship.transform.position = tmCTR.tilemap.CellToWorld(cell);
                    tmCTR.Clicked(cell, 3, TilemapController.SelectedType.highlight);
                    spaceship.SetSelected(false);
                    Debug.Log("Highlight");
                    return;
                }
            }
            tmCTR.Clicked(cell, 3, TilemapController.SelectedType.tile);
            foreach(SpaceShip spaceship in ir.GetSpaceShips())
            {
                spaceship.SetSelected(false);
            }
            Debug.Log("Normal");
        }
    }
}
