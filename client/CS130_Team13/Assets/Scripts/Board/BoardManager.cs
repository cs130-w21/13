using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileState
{
    Null, // Not a valid space
    Empty, // Nothing in a valid space
    Occupied, // Something in a valid space
    Powerup // Space contains a powerup
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
    //#############################################
    // Class Variables
    // ############################################

    /// References for tilemaps that handle the objects on the screen
    public Tilemap backgroundTilemap;
    public Tilemap objectTilemap;

    /// References for tiles to fill the tilemap
    public TileBase bgTile;
    public TileBase placedRock;

    /// Can contain any number of tiles that will randomly fill the board
    public List<TileBase> rockTiles;

    /// Filled with 3 tiles in editor: small, medium, large, in that order
    public List<TileBase> gemTiles;

    /// Reference for the powerup tile icon. Types of powerups are randomized.
    public TileBase powerupTile;
    private Dictionary<Vector3Int, int> powerupLocations;
    public Color borderColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);

    /// Reference for camera so it can properly be scaled to fit the board
    public Camera cam;

    /// Reference for each of the players; needs to probably be switched to their base class later, but is temporarily a gameobject for testing
    public Robot player1;
    public Robot player2;

    /// Size of the grid.
    private const int boardWidth = Constants.Board.BOARD_WIDTH;
    private const int boardHeight = Constants.Board.BOARD_HEIGHT;

    private GameManager gameManager;


    //#############################################
    // Class Functions
    // ############################################

    /// <summary> 
    /// Creates a board based on the board width and height
    /// The board is centered on the screen, and the bottom square left is (0,0)
    /// </summary>
    public void CreateBoard(GameManager gm, int seed)
    {
        ////////////////////////////////
        // Camera + background setup
        ////////////////////////////////

        // Set the random seed based on the input
        gameManager = gm;
        Random.InitState(seed);
        powerupLocations = new Dictionary<Vector3Int, int>();

        // Move the camera to the center and scale camera to fit the whole board
        cam.transform.position = new Vector3(boardWidth / 2, boardHeight / 2, cam.transform.position.z);
        cam.orthographicSize = boardHeight / 2 + 3;

        List<Vector3Int> unoccupiedTiles = new List<Vector3Int>();

        // Generate the background board from the basic tiles
        for (int y = 0; y < boardHeight; y++)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                Vector3Int tilepos = new Vector3Int(x, y, 0);
                backgroundTilemap.SetTile(tilepos, bgTile);
                // Push each tile onto unoccupiedSpaces
                unoccupiedTiles.Add(tilepos);
            }
        }

        // Fill border with tiles
        // This portion is purely aesthetic and can be removed if the screen looks too cluttered
        // Also this is messy code, could probably improve it by making a List<Vector3Int> of all the coords first
        for (int x = -1; x < boardWidth + 1; x++)
        {
            Vector3Int vec1 = new Vector3Int(x, -1, 0);
            backgroundTilemap.SetTile(vec1, bgTile);
            backgroundTilemap.SetTileFlags(vec1, TileFlags.None);
            backgroundTilemap.SetColor(vec1, borderColor);
            Vector3Int vec2 = new Vector3Int(x, boardHeight, 0);
            backgroundTilemap.SetTile(vec2, bgTile);
            backgroundTilemap.SetTileFlags(vec2, TileFlags.None);
            backgroundTilemap.SetColor(vec2, borderColor);
        }
        for (int y = 0; y < boardHeight; y++)
        {
            Vector3Int vec1 = new Vector3Int(-1, y, 0);
            backgroundTilemap.SetTile(vec1, bgTile);
            backgroundTilemap.SetTileFlags(vec1, TileFlags.None);
            backgroundTilemap.SetColor(vec1, borderColor);
            Vector3Int vec2 = new Vector3Int(boardWidth, y, 0);
            backgroundTilemap.SetTile(vec2, bgTile);
            backgroundTilemap.SetTileFlags(vec2, TileFlags.None);
            backgroundTilemap.SetColor(vec2, borderColor);
        }

        ////////////////////////////////
        // Player setup
        ////////////////////////////////

        // Set player positions at opposite corners
        player1.transform.position = backgroundTilemap.LocalToWorld(new Vector3(0.5f, 0.5f, 0));
        player2.transform.position = backgroundTilemap.LocalToWorld(new Vector3(boardWidth - 0.5f, boardHeight - 0.5f, 0));
        unoccupiedTiles.Remove(new Vector3Int(0, 0, 0));
        unoccupiedTiles.Remove(new Vector3Int(boardHeight - 1, boardWidth - 1, 0));

        // Pass this BoardManager to the players so they can access its functions
        player1.Init(this);
        player2.Init(this);

        ////////////////////////////////
        // Board setup
        ////////////////////////////////

        // Fill the board with points
        for (int i = 0; i < Constants.Game.GEM_COUNT; i++)
        {
            PlaceRandomCollectable(gemTiles[0], unoccupiedTiles);
            PlaceRandomCollectable(gemTiles[1], unoccupiedTiles);
            PlaceRandomCollectable(gemTiles[2], unoccupiedTiles);
        }

        // Fill the board with powerups
        for (int i = 0; i < Constants.Game.POWERUP_COUNT; i++)
        {
            Vector3Int pos = PlaceRandomCollectable(powerupTile, unoccupiedTiles);
            powerupLocations.Add(pos, Random.Range(0, 3));
        }

        // Fill the board with rocks
        for (int y = 0; y < boardHeight; y++)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (GetTileState(pos) == TileState.Empty)
                {
                    int rockType = Random.Range(0, rockTiles.Count);
                    objectTilemap.SetTile(pos, rockTiles[rockType]);
                }
            }
        }
    } // End CreateBoard

    /// Takes in a World space Vector3 and gets the state of the tile at that position.
    public TileState GetTileState(Vector3 tilePos)
    {
        Vector3Int intTilePos = objectTilemap.WorldToCell(tilePos);
        int x = intTilePos.x;
        int y = intTilePos.y;
        // Cells off the board are null
        if (x < 0 || x > boardWidth - 1 || y < 0 || y > boardHeight - 1)
        {
            return TileState.Null;
        }
        // Check for powerup
        if (objectTilemap.GetTile(intTilePos) == powerupTile)
        {
            return TileState.Powerup;
        }
        // Check if the cell is occupied
        if (objectTilemap.HasTile(intTilePos)
            || objectTilemap.WorldToCell(player1.transform.position) == intTilePos
            || objectTilemap.WorldToCell(player2.transform.position) == intTilePos
            )
        {
            return TileState.Occupied;
        }
        // If not null or occupied, it is empty.
        return TileState.Empty;
    }

    /// Removes a tile for a robot. Adds points if the tile is a gem.
    public void MineTile(Vector3 tilePos, Robot robot)
    {
        Vector3Int intTilePos = objectTilemap.WorldToCell(tilePos);
        TileBase tile = objectTilemap.GetTile(intTilePos);

        if (gemTiles.Contains(tile))
        {
            // Figure out point value to assign
            int points = 0;
            if (tile == gemTiles[0])
                points = Constants.Points.SMALL;
            else if (tile == gemTiles[1])
                points = Constants.Points.MEDIUM;
            else if (tile == gemTiles[2])
                points = Constants.Points.LARGE;
            // Assign point value to player
            if (robot == player1)
                gameManager.UpdateScore(1, points);
            else if (robot == player2)
                gameManager.UpdateScore(2, points);

            objectTilemap.SetTile(intTilePos, null);
        }
        else if (rockTiles.Contains(tile) || gemTiles.Contains(tile) || tile == placedRock)
        {
            objectTilemap.SetTile(intTilePos, null);
        }
    }

    /// Places a tile for a robot
    public void PlaceTile(Vector3 tilePos)
    {
        Vector3Int intTilePos = objectTilemap.WorldToCell(tilePos);
        if (GetTileState(intTilePos) == TileState.Empty)
        {
            objectTilemap.SetTile(intTilePos, placedRock);
        }
    }

    /// Checks if the robot is on a tile that gives points
    public void CheckForCollectable(Robot robot)
    {
        Vector3Int intTilePos = objectTilemap.WorldToCell(robot.transform.position);
        TileBase robotTile = objectTilemap.GetTile(intTilePos);
        // Check for a gem and give a player points
        if (robotTile == powerupTile)
        {
            int powerupType = powerupLocations[intTilePos];
            switch (powerupType)
            {
                case 0:
                    robot.PowerupBatteryBoost();
                    break;
                case 1:
                    robot.PowerupMineBoost();
                    break;
                case 2:
                    robot.PowerupMoveCostReduction();
                    break;
                default:
                    break;
            }

            objectTilemap.SetTile(intTilePos, null);
            return;
        } //else if () 
    }

    /// Takes in two command strings and runs them on the board.
    /// Alternates between P1 and P2, but runs both to completion if one is shorter than the other.
    public void RunTurn(string p1Moves, string p2Moves)
    {
        StartCoroutine(RunTurnHelper(p1Moves, p2Moves));
        player1.Recharge();
        player2.Recharge();
    }

    // Helper function to make RunTurn a coroutine.
    private IEnumerator RunTurnHelper(string p1Moves, string p2Moves)
    {
        // TODO: Shift camera position to fit UI

        bool gameOver = false;
        // Players give input, run the commands
        int len = Mathf.Max(p1Moves.Length, p2Moves.Length);
        for (int i = 0; i < len; i++)
        {
            // Run P1's move
            if (i < p1Moves.Length)
                yield return StartCoroutine(RunCommand(player1, p1Moves[i]));
            yield return new WaitForSeconds(Constants.Game.ACTION_PAUSE_BETWEEN);
            if (gameManager.GetScore(1) >= Constants.Game.TARGET_SCORE || gameManager.GetScore(2) >= Constants.Game.TARGET_SCORE)
            {
                gameOver = true;
                break;
            }
            // Run P2's move
            if (i < p2Moves.Length)
                yield return StartCoroutine(RunCommand(player2, p2Moves[i]));
            yield return new WaitForSeconds(Constants.Game.ACTION_PAUSE_BETWEEN);
            if (gameManager.GetScore(1) >= Constants.Game.TARGET_SCORE || gameManager.GetScore(2) >= Constants.Game.TARGET_SCORE)
            {
                gameOver = true;
                break;
            }
        }
        if (gameOver)
        {
            gameManager.EndGame();
        }
        else
        {
            // Pause before continuing to the next turn
            yield return new WaitForSeconds(Constants.Game.END_TURN_PAUSE);

            // Both robots have finished running, so do some cleanup
            // TODO: Reset the camera position back to programming state
        }

    }

    /// RunCommand takes in a robot and a command char and tells the robot to do the corresponding command.
    private IEnumerator RunCommand(Robot robot, char cmd)
    {
        switch (cmd)
        {
            case 'L': // Rotate left
                yield return StartCoroutine(robot.Rotate90(Direction.Left));
                break;
            case 'R': // Rotate right
                yield return StartCoroutine(robot.Rotate90(Direction.Right));
                break;
            case 'F': // Move forward
                yield return StartCoroutine(robot.Move(Direction.Up));
                CheckForCollectable(robot);
                break;
            case 'B': // Move back
                yield return StartCoroutine(robot.Move(Direction.Down));
                CheckForCollectable(robot);
                break;
            case 'M': // Mine
                yield return StartCoroutine(robot.Mine());
                break;
            case 'P': // Place
                yield return StartCoroutine(robot.Place());
                break;
            default:
                break;
        }
    }

    /// Takes a tile and a list of unoccupied spaces, then places the tile in a random one of those spaces
    private Vector3Int PlaceRandomCollectable(TileBase tile, List<Vector3Int> spaces)
    {
        if (spaces.Count <= 0)
            return new Vector3Int(-1, -1, 0);
        Vector3Int pos = spaces[Random.Range(0, spaces.Count)];
        objectTilemap.SetTile(pos, tile);
        spaces.Remove(pos);
        return pos;
    }
}