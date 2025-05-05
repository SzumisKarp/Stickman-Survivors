using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    [Header("References")]
    public Tilemap targetTilemap;
    public TileBase[] tileVariants;

    [Header("Map Size")]
    public int width = 20;
    public int height = 10;

    // Start is called before the first frame update
    void Start()
    {
        if (targetTilemap != null)
        {
            generateMap();
        }
    }

    // Ręczne wywołanie w edytorze: prawy klik na komponencie → Generate Map
    [ContextMenu("Generate Map")]
    void generateMap()
    {
        if (tileVariants == null || tileVariants.Length == 0)
            return;

        targetTilemap.ClearAllTiles();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                TileBase chosen = tileVariants[Random.Range(0, tileVariants.Length)];
                targetTilemap.SetTile(new Vector3Int(x, y, 0), chosen);
            }
        }

        targetTilemap.RefreshAllTiles();
        Debug.Log("Map Generated");
    }
}
