using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class BuildManager : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile[] tiles;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            TileBase currentTile = tilemap.GetTile(tilemap.WorldToCell(position));

            if(currentTile is null)
            {
                Debug.Log(tilemap.WorldToCell(position));
                Debug.Log(position);
                int randomRock = Random.Range(0, tiles.Length+1);
                tilemap.SetTile(tilemap.WorldToCell(position), tiles[randomRock]);
            }
        }
    }
    
}
