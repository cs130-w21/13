using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// abstract public class RobotCommand
// {
//     public abstract void RunCommand();

// }
public class RobotMovement : MonoBehaviour
{
    public float actionSpeed = 0.4f;

    // Takes in two Vector3 positions and moves the object to that position
    IEnumerator MoveToCoroutine(Vector3 dest) 
    {
         Vector3 start = transform.position;
         float timer = 0;
         while (timer <= actionSpeed) 
         {
             timer += Time.deltaTime;
             transform.position = Vector3.Lerp(start, dest, timer/actionSpeed);
             yield return null;
         }
         transform.position = dest;
 }

    public void ProcessCommands(string cmdstr)
    {
            //Take in a string and parse it for movement directions
    }

    public void Move(Vector3 dir, float dist)
    {
        /*if (CheckMoveCollision() == true)
        {
            return;
        }*/
        print("Starting Move");
        StartCoroutine(MoveToCoroutine(transform.position + dir.normalized * dist));
    }

    public void PlaceRock(Vector3 dir)
    {
        
    }
}

