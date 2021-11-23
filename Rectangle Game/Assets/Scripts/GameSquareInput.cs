
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

    public ParticleSystem snapParticleSystem;

    public int idxN = 0;
    private int[] sequenceOfN = new int[] { 2, 5, 8, 6, 10, 9, 7 };

    public List<Material> skyboxes;
    

    public GameUI gameUI;

    public Solutions solutions;

    public GameSquare gameSquare; //  the sprite renderer on gameSquare is used as the bounds for mouse input for selecting and moving the selected rectangle
    public RectangleFactory rectangleFactory;

    public float snapDistance = 0.05f; // can be adjusted in editor

    private readonly List<RectanglePerimeter> rectanglePerimeters = new List<RectanglePerimeter>(); // used to check for win and snap rectangle being moved. Includes rectanglePerimeter from gameSquare, ie not all rectanglePerimeters are rectangles

    private readonly List<Rectangle> rectangles = new List<Rectangle>(); // used to track rectangles to be selected and assign selected rectangle
    private Rectangle selectedRectangle; // used as rectangle in transform methods

    public Dropdown dropdownScale; // UI object from canvas
    public Text winText; // currently the set as the same object as rectangleCommandText
    public Text rectangleCount;
    public Text rectangleCommandText; // tells keyboard key to move or drop rectangle
    public Text topText; // writes the N being solved for and the current number of rectangles on the square


    public DebugSprites debugSprites;

    // when true during update will try to move rectangle, when false rectangle is 'dropped'
    public bool doMoveRectangle = false;

    private bool randomizeColorsRunning = false; // controls access to randomize color routine so can only be run several times a second
    private bool changeSkyboxesRunning = false; // controls access to change skyboxes routine so can only be run several times a second
    private bool scrollInputRunning = false; // controls access to change scale routine so can only be run several times a second

    private IEnumerator ScrollInputRoutine()
    {
        if (!scrollInputRunning)
        {
            scrollInputRunning = true;
            // if scroll to increase scale
            if (Input.mouseScrollDelta.y > 0)
            {
                // if not on last option, then increment index
                if (dropdownScale.value != dropdownScale.options.Count - 1) dropdownScale.value++;
            }
            else
            {
                // if not on first option, then decrement index
                if (dropdownScale.value != 0) dropdownScale.value--;
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

    public void RandomizeRectangleColors()
    {
        StartCoroutine(RandomizeRectangleColorsRoutine());
    }

    // only allow colors to be changed 0.5f seconds after being changed so button can not be used to create flashing colors
    private IEnumerator RandomizeRectangleColorsRoutine()
    {
        if (!randomizeColorsRunning)
        {
            randomizeColorsRunning = true;
            rectangles.ForEach(x => x.gameObject.SetSpriteRendererColor(UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f)));
            yield return new WaitForSeconds(0.5f);
            randomizeColorsRunning = false;
        }
    }

    public void DeleteSelectedRectangle()
    {
        DropRectangle();
        rectanglePerimeters.Remove(selectedRectangle.RectanglePerimeter);
        rectangles.Remove(selectedRectangle);
        Destroy(selectedRectangle.gameObject);
        UpdateRectangleCountText();
        // if there are other rectangles, set to last rectangle created
        if (rectangles.Count > 0)
        {
            SetSelectedRectangle(rectangles[rectangles.Count - 1]);
        }
    }

    public void ResetSquare()
    {
        selectedRectangle = null;
        RemoveRectanglesFromRectanglePerimeters();
        foreach (var rectangle in rectangles)
        {
            Destroy(rectangle.gameObject);
        }
        rectangles.RemoveRange(0, rectangles.Count);

        UpdateRectangleCountText();
        rectangleCommandText.text = "Welcome to the Rectangle Game";
    }

    public void ScaleSelectedRectangle(Dropdown change)
    {
        int dividend = 1;
        int divisor = 1;
        if (change.value != 0)
        {
            dividend = Convert.ToInt32(change.captionText.text.Split('/')[0]);
            divisor = Convert.ToInt32(change.captionText.text.Split('/')[1]);
        }

        if (selectedRectangle != null)
        {
            selectedRectangle.scaleDropdownValue = change.value;
            selectedRectangle.SetScale(dividend, divisor);
            
            // scale selected rectangle cursor
            transform.localScale = Vector2.one * 0.18f * (selectedRectangle.transform.localScale.y > selectedRectangle.transform.localScale.x ? selectedRectangle.transform.localScale.y : selectedRectangle.transform.localScale.x);
        }
    }

    private void Start()
    {
        //mainCamera = Camera.main;
        gameUI.OpenRules();
        UpdateRectangleCountText();

        RenderSettings.skybox = skyboxes[0];
    }

    private void AdvanceN()
    {
        solutions.AddSolution(sequenceOfN[idxN], rectangles);
        rectangles.RemoveRange(0, rectangles.Count);
        RemoveRectanglesFromRectanglePerimeters();

        if (idxN < sequenceOfN.Length - 1) idxN++; // do not go past last solution in sequence
        ResetSquare();
    }

    private void RemoveRectanglesFromRectanglePerimeters()
    {
        rectanglePerimeters.RemoveAll(x => x.name.ToUpper().Contains("RECTANGLE"));
    }

    private void FixedUpdate()
    {
        if (doMoveRectangle)
        {
            foreach (var key in selectedRectangle.RectanglePerimeter.corners.Keys)
            {
                bool cornerFound = false;
                foreach (var rectanglePerimeter in rectanglePerimeters)
                {
                    if (rectanglePerimeter.name == selectedRectangle.name || cornerFound) continue;
                    foreach (var otherKey in rectanglePerimeter.corners.Keys)
                    {
                        if (key == otherKey || cornerFound) continue;
                        if (selectedRectangle.RectanglePerimeter.corners[key] == rectanglePerimeter.corners[otherKey])
                        {
                            cornerFound = true;
                            snapParticleSystem.transform.position = rectanglePerimeter.corners[otherKey];
                            snapParticleSystem.Emit(1);
                        }
                    }
                }
            }
        }
    }

    private void Update()
    {

        if (GetMouseDown()) // if attempting mouseclick input
        {
            SelectRectangle(); // try to change selected rectangle
        }



        // only attempt rectangle transform input if selected rectangle is assigned
        if (selectedRectangle != null)
        {
            DoRectangleMoveInput(); // sets transform mode and runs transform mode that has been set
            DoRectangleScaleInput();
            DoRectangleRotateInput();

            DoChangeRectangleInput();

            
        }


        // used for debugging
        if (Input.GetKeyDown(KeyCode.P))
        {
            foreach (var rectangle in rectanglePerimeters)
            {
                print($"{rectangle.name} {rectangle}");
            }
        }

    }


    private void DoChangeRectangleInput()
    {
        int rectangleIndex = rectangles.IndexOf(selectedRectangle);
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (rectangleIndex == rectangles.Count - 1) rectangleIndex = 0;
            else rectangleIndex++;
            SetSelectedRectangle(rectangles[rectangleIndex]);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (rectangleIndex == 0) rectangleIndex = rectangles.Count - 1;
            else rectangleIndex--;
            SetSelectedRectangle(rectangles[rectangleIndex]);
        }
        
    }

    private void DoRectangleRotateInput()
    {
        if (Input.GetMouseButtonDown(1)) // right click
        {
            selectedRectangle.transform.Rotate(new Vector3(0, 0, 90));
            selectedRectangle.RectanglePerimeter.SetCorners();
        }
    }

    public void AddRectangle()
    {
        SetSelectedRectangle(rectangleFactory.CreateRectangle());
    }

    public void UpdateRectangleCountText()
    {
        rectangleCount.text = $"N = {rectangles.Count}. Solve N = {sequenceOfN[idxN]}";
    }

    public void AddCloneRectangle()
    {
        if (selectedRectangle != null)
        {
            SetSelectedRectangle(rectangleFactory.CreateClone(selectedRectangle));
        }
    }

    // assigned to check win button click event in editor
    public void CheckWinRoutine()
    {
        StartCoroutine(WinRoutine());
    }

    private IEnumerator WinRoutine()
    {
        string originalText = "Check\nWin";// winText.text;
        if (CheckWin())
        {
            if (rectangles.Count == sequenceOfN[idxN])
            {
                //winText.text = "You won!";

                switch (sequenceOfN[idxN])
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

                AdvanceN();
            }
            else
            {
                winText.text = "Wrong\nN";
            }
        }
        else
        {
            winText.text = "No win";
        }
        yield return new WaitForSeconds(2f);
        winText.text = originalText;
    }

    private void SelectRectangle()
    {
        Vector2 mousePosition = GetMousePosition(); 
        if (gameSquare.SpriteRenderer.bounds.Contains(mousePosition)) // only test if clicked inside the gameSquare
        {
            // find the first rectangle in the list whose sprite renderer contains the position of the mouse and set it as selected rectangle
            foreach (var rectangle in rectangles)
            {
                if (rectangle.GetComponent<SpriteRenderer>().bounds.Contains(mousePosition)) // here I use GetComponent because this method is not called frequently
                {
                    SetSelectedRectangle(rectangle); // assigns selected rectangle and moved input position to the new selected rectangle
                    break;
                }
            }
        }
    }

    private void DoRectangleMoveInput()
    {
        if (Input.GetKeyDown(KeyCode.M) && !doMoveRectangle) // if start move routine
        {
            doMoveRectangle = true;
            rectangleCommandText.text = "press (d) to drop rectangle";
        }


        if (Input.GetKeyDown(KeyCode.D))
        {
            DropRectangle();
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
        if (doMoveRectangle)
        {
            Vector2 newPosition = GetMousePosition();
            if (gameSquare.SpriteRenderer.bounds.Contains(newPosition)) // only move cursor if will stay inside gameSquare
            {
                transform.position = newPosition;
                MoveSelectedRectangle(); // moves selected rectangle by calculating newPosition
            }
        }
    }

    private void DropRectangle()
    {
        doMoveRectangle = false;
        rectangleCommandText.text = "press (m) to move rectangle";
    }

    public void RotateSelectedRectangle()
    {
        if (true)//!doMoveRectangle)
        {
            selectedRectangle.transform.Rotate(new Vector3(0, 0, 90));
            selectedRectangle.RectanglePerimeter.SetCorners(); 
        }
    }

    // returns true if valid solution, false if not
    private bool CheckWin()
    {
        List<RectanglePerimeter> tempRectanglePerimeters = rectanglePerimeters;
        // 2 foreach loops
        // 1 to check that every rectangle perimeter has 4 corners touching other corners
        // 1 to check all other rectangle perimeters against rectangle perimeter being checked from the outer foreach loop
        foreach (var rectanglePerimeter in rectanglePerimeters)
        {
            int numberOfCornersMatched = 0; // there are situations where a rectangle can have 4 corner matches with one corner having no match. The only way to cause this though I believe would make another rectangle fail when it is then checked
            foreach (var otherRectanglePerimeter in tempRectanglePerimeters)
            {
                if (rectanglePerimeter.gameObject.name == otherRectanglePerimeter.gameObject.name) continue; // do not compare a rectangle perimeter to itself

                // check every corner against every other corner except corners that are the same. ie do not check top left corner against another top left corner. This would mean the rectangles are overlapped
                foreach (var cornerKey in rectanglePerimeter.corners.Keys)
                {
                    foreach (var otherCornerKey in otherRectanglePerimeter.corners.Keys)
                    {
                        if (rectanglePerimeter.corners[cornerKey] == otherRectanglePerimeter.corners[otherCornerKey] && cornerKey != otherCornerKey)
                        {
                            numberOfCornersMatched++;
                        }
                        else if (rectanglePerimeter.corners[cornerKey] == otherRectanglePerimeter.corners[cornerKey]) // if the same corners have the same value then the rectangles are overlapped
                        {
                            return false;
                        }
                    }
                }
            }
            if (numberOfCornersMatched < 4) // rectangle failed. Solution is not valid
            {
                return false;
            }
        }

        // every corner had a match. return true
        return true;
    }


    public void MoveSelectedRectangle()
    {
        Vector2 newPosition = transform.position; // transform.position has been updated with the new mouse position before this method was called
        bool closeToOtherCorner = false; // used so that when a corner is found close enough to snap to the routine will stop checking for other snaps to ensure corners are preferred to edges. Also used because I could not break out of the outer loop

        // search through all rectangle perimeters for snapping points. This includes the rectangle perimeter from gameSquare
        foreach (var rectanglePerimeter in rectanglePerimeters)
        {
            if (rectanglePerimeter.gameObject.name == selectedRectangle.gameObject.name || closeToOtherCorner) // if it is the same rectangle or a corner has been found, then don't search for snap
            {
                continue;
            }

            float minDistance = 1000; // used to be able to snap to closest point
            Corner closestCorner = Corner.Null; // the corner of the selected rectangle that will snap if a snap occurs
            Vector2 closestPoint = Vector2.zero; // the point found on other rectangle that is closest to a corner of the selected rectangle

            // iterate through the corners of the selected rectangle
            for (int i = 0; i < 4; i++)
            {
                Corner corner = (Corner)i; // corner being evaluated on selected rectangle

                // get position of corner as if selected rectangle's position is at origin
                Vector2 cornerOffset = selectedRectangle.RectanglePerimeter.Bounds.extents; // extents gives position of top right corner
                // change offset to correct corner by changing corresponding signs
                switch (corner)
                {
                    case Corner.TopLeft:
                        cornerOffset.x *= -1;
                        break;
                    case Corner.TopRight:
                        break;
                    case Corner.BottomRight:
                        cornerOffset.y *= -1;
                        break;
                    case Corner.BottomLeft:
                        cornerOffset.x *= -1;
                        cornerOffset.y *= -1;
                        break;
                }

                
                Vector2 rectangleCornerFromInputPosition = transform.position + (Vector3)cornerOffset; // simulated corner position. Used instead of actual corner position so rectangle can snap and unsnap
                Vector2 closestPointOnRectanglePerimeter = rectanglePerimeter.SpriteRenderer.bounds.ClosestPoint(rectangleCornerFromInputPosition); // gives closest point on bounds. Will give points from inside bounds. This is handled below by checking if the closest point was inside, and if so, then not allowing snapping to it

                float distance = Vector2.Distance(closestPointOnRectanglePerimeter, rectangleCornerFromInputPosition); // distance of possible snap

                // if the snap position is not inside the rectangle to snap to and is within snap distand and is closer than any other snaps found
                if (!rectanglePerimeter.Bounds.Contains(rectangleCornerFromInputPosition) && distance < snapDistance && distance < minDistance)
                {
                    // update snap variables
                    minDistance = distance;
                    closestCorner = corner;
                    closestPoint = closestPointOnRectanglePerimeter;

                    // check the corner of the selected rectangle against the corners of the rectangle being snapped to. If any two corners are within snap distance, update the snap variables respectively and 'break' out of the outer loop by setting closeToOtherCorner to true
                    foreach (var cornerKey in selectedRectangle.RectanglePerimeter.corners.Keys)
                    {
                        foreach (var otherCornerKey in selectedRectangle.RectanglePerimeter.corners.Keys)
                        {
                            if (cornerKey == otherCornerKey) continue; // do not snap shared corners. This is not a problem with gameSquare rectanglePerimeter, because the corners on gameSquare are inversed
                            if (Vector2.Distance(selectedRectangle.RectanglePerimeter.corners[cornerKey], rectanglePerimeter.corners[otherCornerKey]) < snapDistance * 1f)
                            {
                                closeToOtherCorner = true;
                                closestCorner = cornerKey;
                                closestPoint = rectanglePerimeter.corners[otherCornerKey];
                            } 
                        }
                    }
                } 
            }

            // get new center from closestCorner at point closestPoint
            if (closestCorner != Corner.Null)
            {
                switch (closestCorner)
                {
                    case Corner.TopLeft:
                        newPosition = closestPoint + new Vector2(selectedRectangle.RectanglePerimeter.Bounds.extents.x, -selectedRectangle.RectanglePerimeter.Bounds.extents.y);
                        break;
                    case Corner.TopRight:
                        newPosition = closestPoint + new Vector2(-selectedRectangle.RectanglePerimeter.Bounds.extents.x, -selectedRectangle.RectanglePerimeter.Bounds.extents.y);
                        break;
                    case Corner.BottomRight:
                        newPosition = closestPoint + new Vector2(-selectedRectangle.RectanglePerimeter.Bounds.extents.x, selectedRectangle.RectanglePerimeter.Bounds.extents.y);
                        break;
                    case Corner.BottomLeft:
                        newPosition = closestPoint + new Vector2(selectedRectangle.RectanglePerimeter.Bounds.extents.x, selectedRectangle.RectanglePerimeter.Bounds.extents.y);
                        break;
                }
            }
        }

        // so a rectangle does not get stuck on a corner
        // if the input cursor is further from the center of the rectangle than the rectangles smallest radius, then unsnap the rectangle and set to the position of the input cursor
        if (Vector2.Distance(newPosition, transform.position) > 
            (selectedRectangle.RectanglePerimeter.Bounds.extents.x < selectedRectangle.RectanglePerimeter.Bounds.extents.y ? selectedRectangle.RectanglePerimeter.Bounds.extents.x : selectedRectangle.RectanglePerimeter.Bounds.extents.y)
            )
        {
            selectedRectangle.transform.position = transform.position;
        }
        // otherwise set to the new calculated position
        else
        {
            selectedRectangle.transform.position = newPosition;

        }
        selectedRectangle.RectanglePerimeter.SetCorners();
    }

    // position of mouse from screen to world coordinates. transform.position coordinates are world coordinates, ie the coordinates of gameobjects
    public Vector2 GetMousePosition()
    {
        return mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    public bool GetMouseDown()
    {
        return Input.GetMouseButtonDown(0);
    }

    // used by rectangle factory so that anytime a rectangle is created, it will be added to the list of rectangles here in GameSquareInput
    public void AddRectangle(Rectangle rectangle)
    {
        rectangles.Add(rectangle);
        rectanglePerimeters.Add(rectangle.RectanglePerimeter);
    }

    // used by gameSquare to add its rectanglePerimeter so it is accounted for during the check win and snap routines
    public void AddRectanglePerimeter(RectanglePerimeter rectanglePerimeter)
    {
        rectanglePerimeters.Add(rectanglePerimeter);
    }

    // does not drop rectangle, so whatever mode was on the selected rectangle before calling this method will still be active
    private void SetSelectedRectangle(Rectangle rectangle)
    {
        transform.position = rectangle.transform.position;
        selectedRectangle = rectangle;

        dropdownScale.value = selectedRectangle.scaleDropdownValue;

        DropRectangle();
    }

}