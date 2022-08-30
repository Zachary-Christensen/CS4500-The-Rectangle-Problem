using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInput : MonoBehaviour
{
    private Camera mainCamera;


    private void Awake()
    {
        mainCamera = Camera.main;
    }

    public bool DoSelectNextRectangle()
    {
        return Input.GetKeyDown(KeyCode.RightArrow);
    }
    public bool DoSelectLastRectangle()
    {
        return Input.GetKeyDown(KeyCode.LeftArrow);
    }


    public bool DoMove()
    {
        return Input.GetKeyDown(KeyCode.M);
    }

    public bool DoDrop()
    {
        return Input.GetKeyDown(KeyCode.D);
    }

    public bool DoRotate()
    {
        return Input.GetMouseButtonDown(1);
    }

    public float GetScrollValue()
    {
        return Input.mouseScrollDelta.y;
    }

    public bool GetMouseDown()
    {
        return Input.GetMouseButtonDown(0);
    }

    // position of mouse from screen to world coordinates. transform.position coordinates are world coordinates, ie the coordinates of gameobjects
    public Vector3 GetMousePosition()
    {
        Vector2 pos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        return new Vector3(pos.x, pos.y, 0);
    }
}
