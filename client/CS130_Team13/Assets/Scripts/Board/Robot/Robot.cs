using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// The main controller for the player's robot.
/// Its movement is controlled via the BoardManager. 
public enum Direction
{
    Up,
    Down,
    Left,
    Right
}
/// The robot class handles all of the movement of the robot on the board.
public class Robot : MonoBehaviour
{
    public ParticleSystem outOfBatteryEffect;

    private int batteryCharge;
    private int batteryBoostTurns;
    private int moveBoostTurns;
    private int mineBoostTurns;

    private BoardManager boardManager;

    /// Allows the BoardManager to pass a reference to itself so the Robot can get data from it
    public void Init(BoardManager bm)
    {
        boardManager = bm;
        batteryBoostTurns = 0;
        moveBoostTurns = 0;
        mineBoostTurns = 0;
        Recharge();
    }

    /// Resets the battery of the robot to full charge.
    public void Recharge()
    {
        batteryCharge = Constants.Robot.BASE_BATTERY_CHARGE;
        if (batteryBoostTurns > 0)
        {
            batteryCharge += Constants.Robot.BATTERY_PACK_BONUS;
            batteryBoostTurns--;
        }
    }

    /// Plays an animation when there is no more battery
    public void OutOfBattery()
    {
        outOfBatteryEffect.Play();
    }

    /// Adds to the capacity of the battery for a few turns 
    public void PowerupBatteryBoost()
    {
        batteryBoostTurns += Constants.Robot.BATTERY_PACK_TURNS;
    }

    /// Reduces movement cost for a few turns
    public void PowerupMoveCostReduction()
    {
        moveBoostTurns += Constants.Robot.MOVE_BONUS_MOVES;
    }

    /// Reduces the cost of mining for a few turns
    public void PowerupMineBoost()
    {
        mineBoostTurns += Constants.Robot.MINE_BONUS_MOVES;
    }

    /// Rotates 90 degrees. 
    /// Direction.Left is counterclockwise, Direction.Right is clockwise.
    public IEnumerator Rotate90(Direction dir)
    {
        // Check battery cost
        if (batteryCharge < Constants.EnergyCosts.TURN)
        {
            OutOfBattery();
        }
        else
        {
            batteryCharge -= Constants.EnergyCosts.TURN;
            int dirMultiplier = 0;
            switch (dir)
            {
                case Direction.Left:
                    dirMultiplier = 1;
                    break;
                case Direction.Right:
                    dirMultiplier = -1;
                    break;
                default:
                    dirMultiplier = 0;
                    break;
            }
            Quaternion from = transform.rotation;
            Quaternion to = Quaternion.Euler(transform.eulerAngles + Vector3.forward * 90 * dirMultiplier);
            for (float t = 0f; t < 1; t += Time.deltaTime / Constants.Game.ACTION_SPEED)
            {
                transform.rotation = Quaternion.Slerp(from, to, t);
                yield return null;
            }
            transform.rotation = to;
        }
    }

    /// Move in the given direction.
    /// If the tile space is occupied or inaccessable, 
    /// play an animation and stay in the same spot.
    public IEnumerator Move(Direction dir)
    {
        // Check battery cost
        int moveCost = moveBoostTurns > 0 ? Constants.EnergyCosts.BOOSTED_MOVE : Constants.EnergyCosts.MOVE;
        if (batteryCharge < moveCost)
        {
            OutOfBattery();
        }
        else
        {
            batteryCharge -= moveCost;
            // Move the robot
            Vector3 start = transform.position;
            Vector3 dest = start;
            int dirMultiplier = 0;
            switch (dir)
            {
                case Direction.Up:
                    dest += transform.up.normalized;
                    dirMultiplier = 1;
                    break;
                case Direction.Down:
                    dest -= transform.up.normalized;
                    dirMultiplier = -1;
                    break;
                default:
                    break;
            }
            float timer = 0;
            // Move forward, lerped over a duration
            TileState moveTileState = boardManager.GetTileState(dest);
            if (moveTileState == TileState.Empty || moveTileState == TileState.Powerup)
            {
                while (timer <= Constants.Game.ACTION_SPEED)
                {
                    transform.position = Vector3.Lerp(start, dest, timer / Constants.Game.ACTION_SPEED);
                    timer += Time.deltaTime;
                    yield return null;
                }
                transform.position = dest;
            }
            // Cannot move forward
            else
            {
                // Some fancy math to animate it wobbling but staying in place
                Vector3 facingDir = transform.up;
                Quaternion startRot = transform.rotation;
                for (float t = 0f; t < 1; t += Time.deltaTime / Constants.Game.ACTION_SPEED)
                {
                    float angle = Mathf.Sin(t * 6.28f) * 10 + startRot.eulerAngles.z; // Wobbles twice
                    float dist = Mathf.Sin(t * 3.14f); // Moves forward and back a short dist
                    transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                    transform.position = Vector3.Lerp(start, start + facingDir * 0.2f * dirMultiplier, dist);
                    yield return null;
                }
                transform.rotation = startRot;
                transform.position = start;
            }
        }
    }

    /// Mines the tile in front of the robot
    public IEnumerator Mine()
    {
        int mineCost = moveBoostTurns > 0 ? Constants.EnergyCosts.BOOSTED_MINE : Constants.EnergyCosts.MINE;
        if (batteryCharge < mineCost)
        {
            OutOfBattery();
        }
        else
        {
            batteryCharge -= mineCost;
            // Play an animation that moves the robot forward and back
            Vector3 start = transform.position;
            Vector3 facingDir = transform.up;
            float t = 0f;
            // Move forward
            for (; t < 0.5; t += Time.deltaTime / Constants.Game.ACTION_SPEED)
            {
                float dist = Mathf.Sin(t * 3.14f); // Moves forward and back a short dist
                transform.position = Vector3.Lerp(start, start + facingDir * 0.2f, dist);
                yield return null;
            }

            // Try to mine the block in front
            boardManager.MineTile(start + facingDir, this);

            // Move backward
            for (; t < 1; t += Time.deltaTime / Constants.Game.ACTION_SPEED)
            {
                float dist = Mathf.Sin(t * 3.14f); // Moves forward and back a short dist
                transform.position = Vector3.Lerp(start, start + facingDir * 0.2f, dist);
                yield return null;
            }
            transform.position = start;
        }
    }

    /// Places a rock in front of the robot
    public IEnumerator Place()
    {
        if (batteryCharge < Constants.EnergyCosts.PLACE)
        {
            OutOfBattery();
        }
        else
        {
            batteryCharge -= Constants.EnergyCosts.PLACE;
            // Play an animation that moves the robot forward and back
            Vector3 start = transform.position;
            Vector3 facingDir = transform.up;
            float t = 0f;
            // Move forward
            for (; t < 0.5; t += Time.deltaTime / Constants.Game.ACTION_SPEED)
            {
                float dist = Mathf.Sin(t * 3.14f); // Moves forward and back a short dist
                transform.position = Vector3.Lerp(start, start + facingDir * 0.2f, dist);
                yield return null;
            }

            // Try to mine the block in front
            boardManager.PlaceTile(start + facingDir);

            // Move backward
            for (; t < 1; t += Time.deltaTime / Constants.Game.ACTION_SPEED)
            {
                float dist = Mathf.Sin(t * 3.14f); // Moves forward and back a short dist
                transform.position = Vector3.Lerp(start, start + facingDir * 0.2f, dist);
                yield return null;
            }
            transform.position = start;
        }
    }
}
