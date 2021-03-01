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
// public enum BoardState
// {
//     Connecting, // Player hasn't found a match yet
//     Coding, // Player is inputting code
//     Running // Player is running the board state
// }

    /// <summary> 
    /// The Board Manager handles the state of everything on the board, and communicates with the player objects.
    /// This facilitates the ordering of all of the motion of a turn, and controls some of the game logic.
    /// </summary>
public class BoardManager : MonoBehaviour
{
    /// References for tilemaps that handle the objects on the screen
    public Tilemap backgroundTilemap;
    public Tilemap objectTilemap;

    /// References for tiles to fill the tilemap
    public Tile bgTile; 
    public Color borderColor = new Color(0.5f,0.5f,0.5f,1.0f);

    /// Reference for camera so it can properly be scaled to fit the board
    public Camera cam;

    /// Reference for each of the players; needs to probably be switched to their base class later, but is temporarily a gameobject for testing
    public Robot player1;
    public Robot player2;

    /// Size of the grid. Can be modified in editor, but should remain constant over the course of a game
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

        // Set player positions at opposite corners
        player1.transform.position = backgroundTilemap.LocalToWorld(new Vector3(0.5f,0.5f,0));
        player2.transform.position = backgroundTilemap.LocalToWorld(new Vector3(boardWidth-0.5f,boardHeight-0.5f,0));
        // Pass this BoardManager to the players so they can access its functions
        player1.Init(this);
        player2.Init(this);

    } // End CreateBoard
        
    /// Takes in a World space Vector3 and gets the state of the tile at that position.
    public TileState GetTileState(Vector3 tilePos)
    {
        Vector3Int tilepos = objectTilemap.WorldToCell(tilePos);
        int x = tilepos.x;
        int y = tilepos.y;
        // Cells off the board are null
        if (x < 0 || x > boardWidth - 1 || y < 0 || y > boardHeight - 1)
        {
            return TileState.Null;
        }
        // Check if the cell is occupied
        if (objectTilemap.HasTile(tilepos))
        {
            return TileState.Occupied;
        }
        // If not null or occupied, it is empty.
        return TileState.Empty;
    }

    /// Takes in two command strings and runs them on the board.
    /// Alternates between P1 and P2, but runs both to completion if one is shorter than the other.
    public void RunTurn(string p1Moves, string p2Moves)
    {
        StartCoroutine(RunTurnHelper(p1Moves, p2Moves));
    }

    // Helper function to make RunTurn a coroutine.
    private IEnumerator RunTurnHelper(string p1Moves, string p2Moves)
    {
        // TODO: Shift camera position to fit UI
        
        // Players give input, run the commands
        int len = Mathf.Max(p1Moves.Length, p2Moves.Length);
        for (int i = 0; i < len; i++)
        {
            if (i < p1Moves.Length)
                yield return StartCoroutine(RunCommand(player1, p1Moves[i]));
                yield return new WaitForSeconds(Constants.Game.ACTION_PAUSE_BETWEEN);
            if (i < p2Moves.Length)
                yield return StartCoroutine(RunCommand(player2, p2Moves[i]));
                yield return new WaitForSeconds(Constants.Game.ACTION_PAUSE_BETWEEN);
        }

        // Both robots have finished running, so do some cleanup
        // TODO: Reset the camera position back to programming state
    }

    /// RunCommand takes in a robot and a command char and tells the robot to do the corresponding command.
    private IEnumerator RunCommand(Robot robot, char cmd)
    {
        switch(cmd)
        {
            case 'L': // Rotate left
                yield return StartCoroutine(robot.Rotate90(Direction.Left));
                break;
            case 'R': // Rotate right
                yield return StartCoroutine(robot.Rotate90(Direction.Right));
                break;
            case 'F': // Move forward
                yield return StartCoroutine(robot.Move(Direction.Up));
                break;
            case 'B': // Move back
                yield return StartCoroutine(robot.Move(Direction.Down));
                break;
            case 'M': // Mine

                break;
            case 'P': // Place

                break;
            default: 
                break;
        }
    }
}
