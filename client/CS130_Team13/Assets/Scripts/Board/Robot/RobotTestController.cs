using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotTestController : MonoBehaviour
{
    public GameObject robot;
    private RobotMovement robotMovement; 
    private bool isMoving;
    
    // Start is called before the first frame update
    void Start()
    {
        robotMovement = robot.GetComponent<RobotMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving)
        {
            if (Input.GetKeyDown(KeyCode.W))
        {
            isMoving = true;
            robotMovement.Move(Vector3.up, 1);
            isMoving = false;
        } 
        else if (Input.GetKeyDown(KeyCode.A))
        {
            isMoving = true;
            robotMovement.Move(Vector3.left, 1);
            isMoving = false;
        } 
        else if (Input.GetKeyDown(KeyCode.S))
        {
            isMoving = true;
            robotMovement.Move(Vector3.down, 1);
            isMoving = false;
        } 
        else if (Input.GetKeyDown(KeyCode.D))
        {
            isMoving = true;
            robotMovement.Move(Vector3.right, 1);
            isMoving = false;
        } 
        }

    }
}
