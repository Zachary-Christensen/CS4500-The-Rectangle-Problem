
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * GameSquareInput:
 *  Add rectangles to gameSquare on input
 *  Change selected rectangle
 *  Move, rotate, scale selected rectangle
 *  Move includes snap routine which snaps a corner of the selected rectangle to the side or corner of another rectangle within snap distance
 *  Check for win
 */
public class GameSquareInput : MonoBehaviour
{
    public Camera mainCamera;

    public AudioManager audioManager;


    public DropDownScaleController dropDownScaleController;

    public GameUI gameUI;


    public GameSquare gameSquare; //  the sprite renderer on gameSquare is used as the bounds for mouse input for selecting and moving the selected rectangle


    public TextController textController;

    public RectangleController rectangleController;

    public DebugSprites debugSprites;

    // when true during update will try to move rectangle, when false rectangle is 'dropped'
    //public bool doMoveRectangle = false;

    private bool scrollInputRunning = false; // controls access to change scale routine so can only be run several times a second

    private void Start()
    {
        mainCamera = Camera.main;
        gameUI.OpenRules();
        rectangleController.UpdateRectangleCountText();
    }

   
    private void FixedUpdate()
    {
        if (rectangleController.doMoveRectangle)
        {
            rectangleController.EmitParticleOnSelectedRectangleCorners();
        }
    }

    private void Update()
    {

        if (GetMouseDown()) // if attempting mouseclick input
        {
            if (gameSquare.SpriteRenderer.bounds.Contains(GetMousePosition()))
            {
                rectangleController.SelectRectangle(GetMousePosition()); // try to change selected rectangle
            }
        }



        // only attempt rectangle transform input if selected rectangle is assigned
        if (rectangleController.IsAnyRectangleSelected)
        {
            DoRectangleMoveInput();

            DoRectangleScaleInput();
            DoRectangleRotateInput();

            rectangleController.DoChangeRectangleInput();
        }

    }


    private IEnumerator ScrollInputRoutine()
    {
        if (!scrollInputRunning)
        {
            scrollInputRunning = true;
            // if scroll to increase scale
            if (Input.mouseScrollDelta.y < 0)
            {
                // if not on last option, then increment index
                if (dropDownScaleController.Increment()) audioManager.PlayScaleDown();

            }
            else
            {
                // if not on first option, then decrement index
                if (dropDownScaleController.Decrement()) audioManager.PlayScaleUp();
            }

            yield return new WaitForSeconds(0.3f);
            scrollInputRunning = false;
        }

        yield return null;
    }

    private void DoRectangleScaleInput()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            StartCoroutine(ScrollInputRoutine());
        }
    }

    public void DoRectangleRotateInput()
    {
        if (Input.GetMouseButtonDown(1)) // right click
        {
            if (rectangleController.RotateSelectedRectangle()) audioManager.PlayRotate();
        }
    }

    private void DoRectangleMoveInput()
    {
        if (Input.GetKeyDown(KeyCode.M)) // if start move routine
        {
            audioManager.PlayMove();
            rectangleController.doMoveRectangle = true;
            textController.SetRectangleCommandText("press (d) to drop rectangle");
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            audioManager.PlayDrop();
            rectangleController.DropRectangle();
        }

        /*
         * Move routine works by:
         *      getting the mouse position
         *      checking that the mouse position is within the gameSquare
         *      if it is, then this gameobject is moved to that position
         *      
         *      the position of this object acts as a cursor for the position of the input
         *      the purpose of this objects position acting as a cursor is so that when a rectangle is moved, 
         *          its new position is determined as if the rectangle was at the position of this object, not its actual position. 
         *          If this does not happen, then a rectangle would snap and get stuck
         */
        if (rectangleController.doMoveRectangle)
        {
            Vector2 newPosition = GetMousePosition();
            if (gameSquare.SpriteRenderer.bounds.Contains(newPosition)) // only move cursor if will stay inside gameSquare
            {
                rectangleController.MoveSelectedRectangle(newPosition); // moves selected rectangle by calculating newPosition
            }
        }
    }

    // assigned to check win button click event in editor
    public void CheckWinRoutine()
    {
        StartCoroutine(WinRoutine());
    }

    private IEnumerator WinRoutine()
    {
        string originalText = "Check\nSolution";// winText.text;
        if (rectangleController.CheckWin())
        {
            if (rectangleController.DoesRectangleCountEqualGoalCount)
            {
                //winText.text = "You won!";
                audioManager.PlayWin();

                switch (rectangleController.GetN)
                {
                    case 5:
                        gameUI.EnableNPlus3();
                        gameUI.OpenNPlus3();
                        break;
                    case 6:
                        gameUI.EnableNPlus4();
                        gameUI.OpenNPlus4();
                        break;
                    case 9:
                        gameUI.EnableAllNumbers();
                        gameUI.OpenAllNumbers();
                        break;
                    case 7:
                        gameUI.OpenEndGamePanel();
                        break;
                    default:
                        gameUI.OpenWin();
                        break;
                }

                rectangleController.AdvanceN();
            }
            else
            {
                audioManager.PlayWrongSolution();
                textController.SetWinText("Wrong\nSolution");
            }
        }
        else
        {
            audioManager.PlayLose();
            textController.SetWinText("Wrong");
        }
        yield return new WaitForSeconds(2f);
        textController.SetWinText(originalText);
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