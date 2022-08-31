
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * GameManager:
 *  Manages and connects the execution of other objects
 */
public class GameManager : MonoBehaviour
{

    public AudioManager audioManager;

    public UserInput userInput;

    public DropDownScaleController dropDownScaleController;

    public GameUI gameUI;

    public GameSquare gameSquare; // the sprite renderer on gameSquare is used as the bounds for mouse input for selecting and moving the selected rectangle

    public TextController textController;

    public RectangleController rectangleController;

    public SequenceOfNController sequenceOfNController;

    public DebugSprites debugSprites;

    private bool scrollInputRunning = false; // controls access to change scale routine so can only be run several times a second


    private void Start()
    {
        gameUI.OpenRules();
        rectangleController.UpdateRectangleCountText();
    }

   
    private void FixedUpdate()
    {
        rectangleController.EmitParticleOnSelectedRectangleCorners();
    }

    private void Update()
    {

        if (userInput.GetMouseDown()) // if attempting mouseclick input
        {
            if (gameSquare.Contains(userInput.GetMousePosition()))
            {
                rectangleController.SelectRectangle(userInput.GetMousePosition()); // try to change selected rectangle
            }
        }

        // only attempt rectangle transform input if selected rectangle is assigned
        if (rectangleController.IsAnyRectangleSelected)
        {
            DoRectangleMoveInput();

            DoRectangleScaleInput();
            DoRectangleRotateInput();

            DoChangeRectangleInput();
        }

    }

    public void DoChangeRectangleInput()
    {

        if (userInput.DoSelectNextRectangle())
        {
            rectangleController.SelectNextRectangle();
        }
        else if (userInput.DoSelectLastRectangle())
        {
            rectangleController.SelectLastRectangle();
        }

    }

    private IEnumerator ScrollInputRoutine()
    {
        if (!scrollInputRunning)
        {
            scrollInputRunning = true;
            // if scroll to increase scale
            if (userInput.GetScrollValue() < 0)
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
        if (userInput.GetScrollValue() != 0)
        {
            StartCoroutine(ScrollInputRoutine());
        }
    }

    public void DoRectangleRotateInput()
    {
        if (userInput.DoRotate()) // right click
        {
            if (rectangleController.RotateSelectedRectangle()) audioManager.PlayRotate();
        }
    }

    private void DoRectangleMoveInput()
    {
        if (userInput.DoMove()) // if start move routine
        {
            audioManager.PlayMove();
            rectangleController.AllowMoveRectangle();
            textController.SetRectangleCommandText("press (d) to drop rectangle");
        }

        if (userInput.DoDrop())
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
        if (gameSquare.Contains(userInput.GetMousePosition())) // only move cursor if will stay inside gameSquare
        {
            rectangleController.MoveSelectedRectangle(userInput.GetMousePosition()); // moves selected rectangle by calculating newPosition
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
                audioManager.PlayWin();

                switch (sequenceOfNController.GetN())
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

    

}