using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class BoardManager : MonoBehaviour
{
    // References for tilemaps that handle the objects on the screen
    public Tilemap backgroundTilemap;
    public Tilemap objectTilemap;

    // References for tiles to fill the tilemap
    public Tile bgTile;

    // Size of the grid. Can be modified in editor, but should remain constant over the course of a game
    [SerializeField]
    private int boardWidth = 16;
    [SerializeField]
    private int boardHeight = 10;

    public void CreateBoard(int seed)
    {
        // Set the random seed based on the input
        Random.InitState(seed); 

        // Generate the background board from the basic tiles
        for (int y = 0; y < boardHeight; y++)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                Vector3Int tilepos = new Vector3Int(x,y,0);
                backgroundTilemap.SetTile(tilepos, bgTile);
            }
        }
        
        // Offset the grids so that they are centered on the camera
        Vector3 tileAnchorOffset = new Vector3(-boardWidth/2 + 0.5f, -boardHeight/2 + 0.5f, 0.0f);
        backgroundTilemap.tileAnchor = tileAnchorOffset;
        objectTilemap.tileAnchor = tileAnchorOffset;

        

    }
        

}
