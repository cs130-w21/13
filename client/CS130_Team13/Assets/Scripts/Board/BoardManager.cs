using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileState
{
    Null, // Not a valid space
    Empty, // Nothing in a valid space
    Occupied // Something in a valid space


}
public class BoardManager : MonoBehaviour
{
    // References for tilemaps that handle the objects on the screen
    public Tilemap backgroundTilemap;
    public Tilemap objectTilemap;

    // References for tiles to fill the tilemap
    public Tile bgTile; 

    // Reference for camera so it can properly be scaled to fit the board
    public Camera cam;

    // Reference for each of the players; needs to probably be switched to their base class later, but is temporarily a gameobject for testing
    public GameObject player1;
    public GameObject player2;

    // Size of the grid. Can be modified in editor, but should remain constant over the course of a game
    [SerializeField]
    private int boardWidth = 5;
    [SerializeField]
    private int boardHeight = 5;


    /// <summary> 
    /// Creates a board based on the board width and height
    /// The board is centered on the screen, and the bottom square left is (0,0)
    /// </summary>
    public void CreateBoard(int seed)
    {
        // Set the random seed based on the input
        Random.InitState(seed); 

        // This chunk is maybe not necessary? Move the camera to the center instead
        // // Offset the grids so that they are centered on the camera
        // Vector3 tileAnchorOffset = new Vector3(-boardWidth/2, -boardHeight/2, 0.0f);
        // backgroundTilemap.tileAnchor = tileAnchorOffset;
        // objectTilemap.tileAnchor = tileAnchorOffset;
        
        // Move the camera to the center and scale camera to fit the whole board
        cam.transform.position = new Vector3(boardWidth / 2, boardHeight / 2, cam.transform.position.z);
        cam.orthographicSize = boardHeight / 2 + 3;

        // Generate the background board from the basic tiles
        for (int y = 0; y < boardHeight; y++)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                backgroundTilemap.SetTile(new Vector3Int(x,y,0), bgTile);
            }
        }
        


        // Fill border with tiles
        // This portion is purely aesthetic and can be removed if the screen looks too cluttered
        // Also this is messy code, could probably improve it by making a List<Vector3Int> of all the coords first
        Color borderColor = new Color(0.5f,0.5f,0.5f,1.0f);
        for (int x = -1; x < boardWidth+1; x++)
        {
            Vector3Int vec1 = new Vector3Int(x,-1,0);
            backgroundTilemap.SetTile(vec1, bgTile);
            backgroundTilemap.SetTileFlags(vec1, TileFlags.None);
            backgroundTilemap.SetColor(vec1, borderColor);
            Vector3Int vec2 = new Vector3Int(x,boardHeight,0);
            backgroundTilemap.SetTile(vec2, bgTile);
            backgroundTilemap.SetTileFlags(vec2, TileFlags.None);
            backgroundTilemap.SetColor(vec2, borderColor);
        }
        for (int y = 0; y < boardHeight; y++)
        {
            Vector3Int vec1 = new Vector3Int(-1,y,0);
            backgroundTilemap.SetTile(vec1, bgTile);
            backgroundTilemap.SetTileFlags(vec1, TileFlags.None);
            backgroundTilemap.SetColor(vec1, borderColor);
            Vector3Int vec2 = new Vector3Int(boardWidth,y,0);
            backgroundTilemap.SetTile(vec2, bgTile);
            backgroundTilemap.SetTileFlags(vec2, TileFlags.None);
            backgroundTilemap.SetColor(vec2, borderColor);
        }
    }
        
    public TileState GetTileState(int x, int y)
    {
        if (x < 0 || x > boardWidth - 1 || y < 0 || y > boardHeight - 1)
        {
            return TileState.Null;
        }
            
        if (objectTilemap.HasTile(new Vector3Int(x,y,0)))
        {
            return TileState.Occupied;
        }
        
        return TileState.Empty;
    }

}
