// Retrieves all Tiles from an area on the Tilemap and prints out the Tiles to console
using UnityEngine;
using UnityEngine.Tilemaps;

public class Example : MonoBehaviour
{
    public BoundsInt area;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Tilemap tilemap = GetComponent<Tilemap>();
            TileBase[] tileArray = tilemap.GetTilesBlock(area);
            Debug.Log(tileArray.Length);
            Debug.Log(area);
            Debug.Log(tilemap.cellBounds);

            for (int x = 0; x < area.size.x; x++)
            {
                for (int y = 0; y < area.size.y; y++)
                {
                    TileBase tile = tileArray[x + y * area.size.x];
                    if (tile != null)
                    {
                        Debug.Log("x:" + x + " y:" + y + " tile:" + tile.name);
                    }
                    else
                    {
                        Debug.Log("x:" + x + " y:" + y + " tile: (null)");
                    }
                }
            }
        }
    }
}