
using System.Collections.Generic;
using UnityEngine;

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
    public GameSquare gameSquare; //  the sprite renderer on gameSquare is used as the bounds for mouse input for selecting and moving the selected rectangle

    public float snapDistance = 0.05f; // can be adjusted in editor

    private readonly List<RectanglePerimeter> rectanglePerimeters = new List<RectanglePerimeter>(); // used to check for win and snap rectangle being moved. Includes rectanglePerimeter from gameSquare, ie not all rectanglePerimeters are rectangles

    private readonly List<Rectangle> rectangles = new List<Rectangle>(); // used to track rectangles to be selected and assign selected rectangle
    private Rectangle selectedRectangle; // used as rectangle in transform methods

    public DebugSprites debugSprites;

    // these bool's control access to the different transform methods associated with a rectangle
    // only 1 of them is able to be set to true at a time
    public bool doMoveRectangle = false;
    public bool doRotateRectangle = false;
    public bool doScaleRectangle = false;

    private void Update()
    {
        if (GetMouseDown()) // if attempting mouseclick input
        {
            SelectRectangle(); // try to change selected rectangle
        }

        if (DoAddRectangle()) // user gave command to add rectangle to gameSquare
        {
            SetSelectedRectangle(gameSquare.AddRectangle()); // add rectangle and set as selected rectangle
        }


        // only attempt rectangle transform input if selected rectangle is assigned
        if (selectedRectangle != null)
        {
            DoRectangleTransformInput(); // sets transform mode and runs transform mode that has been set
        }

        if (DoCheckWin())
        {
            if (CheckWin())
            {
                print("You won!");
            }
            else
            {
                print("You have not won");
            }
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

    private void DoRectangleTransformInput()
    {
        if (Input.GetKeyDown(KeyCode.M) && !doRotateRectangle && !doScaleRectangle) // if start move routine
        {
            doMoveRectangle = true;
        }
        else if (Input.GetKeyDown(KeyCode.R) && !doMoveRectangle && !doScaleRectangle) // if start rotate routine. also do rotate routine because of the nature of the rotation
        {
            doRotateRectangle = true;
            selectedRectangle.transform.Rotate(new Vector3(0, 0, 90));
            selectedRectangle.RectanglePerimeter.SetCorners();
        }
        else if (Input.GetKeyDown(KeyCode.S) && !doMoveRectangle && !doRotateRectangle) // if start scale routine
        {
            doScaleRectangle = true;
        }

        // stop all transform routines
        if (Input.GetKeyDown(KeyCode.D))
        {
            doMoveRectangle = false;
            doRotateRectangle = false;
            doScaleRectangle = false;
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
            }

            MoveSelectedRectangle(); // moves selected rectangle by calculating newPosition
        }
        if (doScaleRectangle)
        {
            ScaleSelectedRectangle();
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

    private void ScaleSelectedRectangle()
    {
        // increments or decrements dividend or divisor of scale ratio according to scale direction given
        int scaleDirection = GetScaleDirection();
        if (scaleDirection != 0)
        {

            if (scaleDirection == 1)
            {
                selectedRectangle.SetScale(selectedRectangle.scaleDividend, --selectedRectangle.scaleDivisor);
            }
            else if (scaleDirection == -1)
            {
                selectedRectangle.SetScale(selectedRectangle.scaleDividend, ++selectedRectangle.scaleDivisor);
            }
            else if (scaleDirection == -2)
            {
                selectedRectangle.SetScale(--selectedRectangle.scaleDividend, selectedRectangle.scaleDivisor);
            }
            else if (scaleDirection == 2)
            {
                selectedRectangle.SetScale(++selectedRectangle.scaleDividend, selectedRectangle.scaleDivisor);
            }
        }
    }

    // check win command
    private static bool DoCheckWin()
    {
        return Input.GetKeyDown(KeyCode.W);
    }

    // definitely temporary
    /* The signals are:
     *      -1 subtracts from divisor of scale ratio
     *      1 adds to divisor of scale ratio
     *      -2 subtracts from dividend of scale ratio
     *      2 adds to dividend of scale ratio
     *      0 do nothing
     */
    private static int GetScaleDirection()
    {
        int scaleDirection = 0;
        if (Input.GetKeyDown(KeyCode.DownArrow)) scaleDirection = -1;
        else if (Input.GetKeyDown(KeyCode.UpArrow)) scaleDirection = 1;
        else if (Input.GetKeyDown(KeyCode.LeftBracket)) scaleDirection = -2;
        else if (Input.GetKeyDown(KeyCode.RightBracket)) scaleDirection = 2;

        return scaleDirection;
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

    // command for adding a rectangle. Will cause a rectangle to be created everytime it is evaluated to true.
    public bool DoAddRectangle()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    // position of mouse from screen to world coordinates. transform.position coordinates are world coordinates, ie the coordinates of gameobjects
    public Vector2 GetMousePosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
    }

}