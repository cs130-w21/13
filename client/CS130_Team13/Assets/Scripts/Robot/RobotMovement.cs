﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

Vector2 GetDirectionVector(Direction dir)
{
    switch (dir)
    {
        case Direction.Up: 
            return Vector2.up;
        case Direction.Down: 
            return Vector2.down;
        case Direction.Left: 
            return Vector2.left;
        case Direction.Right: 
            return Vector2.right;
        default: 
            return Vector2.zero;
    }
}
enum Direction {
Up, Down, Left, Right
}


public class RobotMovement : MonoBehaviour
{

    void ProcessCommands(string cmdstr)
    {
            //Take in a string and parse it for movement directions
    }

    void Move(Direction dir)
    {
        
    }

    void PlaceRock(Direction dir)
    {
        
    }



}

