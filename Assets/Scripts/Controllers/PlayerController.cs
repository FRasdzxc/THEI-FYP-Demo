using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    private TilemapController tmCTR;
    private UIController uic;
    private InformationRecorder ir;
    private EventController ec;
    void Start()
    {
        tmCTR = GameObject.FindWithTag("EventSystem").GetComponent<TilemapController>();
        uic = GameObject.FindWithTag("EventSystem").GetComponent<UIController>();
        ir = GameObject.FindWithTag("EventSystem").GetComponent<InformationRecorder>();
        ec = GameObject.FindWithTag("EventSystem").GetComponent<EventController>();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            ir.UpdateTileObjectList();
            ir.UpdateSpaceShips();
            ir.UpdateEnemies();
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cell = tmCTR.tilemap.WorldToCell(mouseWorldPos);
            cell.z = 0;
            uic.TogglePanel(UIController.Panels.info, tmCTR.GetSelected());
            foreach(SpaceShip spaceship in ir.GetSpaceShips())
            {
                if (spaceship.GetHasMoved())
                {

                }
                else
                {
                    if (cell == spaceship.GetCell())
                    {
                        tmCTR.Clicked(cell, 3, TilemapController.SelectedType.player);
                        spaceship.SetSelected(true);
                        foreach(SpaceShip s in ir.GetSpaceShips())
                        {
                            if(s != spaceship)
                            {
                                s.SetSelected(false);
                            }
                        }
                        return;
                    }
                    else if (tmCTR.GetTile(cell) == tmCTR.highlightTile && spaceship.GetSelected())
                    {
                        spaceship.transform.position = tmCTR.tilemap.CellToWorld(cell);
                        tmCTR.Clicked(cell, 3, TilemapController.SelectedType.highlight);
                        spaceship.SetCell(cell);
                        ec.CheckBattle(spaceship);
                        spaceship.SetSelected(false);
                        spaceship.SetHasMoved(true);
                        return;
                    }
                }
            }
            tmCTR.Clicked(cell, 3, TilemapController.SelectedType.tile);
            foreach(SpaceShip spaceship in ir.GetSpaceShips())
            {
                spaceship.SetSelected(false);
            }
        }
    }
}
