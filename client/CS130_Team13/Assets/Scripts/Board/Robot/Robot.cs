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
public class Robot : MonoBehaviour
{
    private int batteryCharge;
    private int batteryBoostTurns;

    private BoardManager boardManager;
    

    /// Allows the BoardManager to pass a reference to itself so the Robot can get data from it
    public void Init(BoardManager bm)
    {
        boardManager = bm;
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

    /// Rotates 90 degrees. 
    /// Direction.Left is counterclockwise, Direction.Right is clockwise.
    public IEnumerator Rotate90(Direction dir) 
    {    
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
        for(float t = 0f; t < 1; t += Time.deltaTime/Constants.Game.ACTION_SPEED) 
        {
            transform.rotation = Quaternion.Slerp(from, to, t);
            yield return null;
        }
        transform.rotation = to;
    }

    /// Move in the given direction.
    /// If the tile space is occupied or inaccessable, 
    /// play an animation and stay in the same spot.
    public IEnumerator Move(Direction dir) 
    {
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
        if (boardManager.GetTileState(dest) == TileState.Empty)
        {
            while (timer <= Constants.Game.ACTION_SPEED) 
            {
                transform.position = Vector3.Lerp(start, dest, timer/Constants.Game.ACTION_SPEED);
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
            for(float t = 0f; t < 1; t += Time.deltaTime/Constants.Game.ACTION_SPEED) 
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
