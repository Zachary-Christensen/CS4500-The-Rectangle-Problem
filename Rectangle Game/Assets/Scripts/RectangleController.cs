using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RectangleController : MonoBehaviour
{
    private readonly List<RectanglePerimeter> rectanglePerimeters = new List<RectanglePerimeter>(); // used to check for win and snap rectangle being moved. Includes rectanglePerimeter from gameSquare, ie not all rectanglePerimeters are rectangles

    private readonly List<Rectangle> rectangles = new List<Rectangle>(); // used to track rectangles to be selected and assign selected rectangle
    public bool DoesRectangleCountEqualGoalCount => rectangles.Count == NController.GetN();
    private Rectangle selectedRectangle; // used as rectangle in transform methods
    public SelectionCursor selectionCursor;
    public bool IsAnyRectangleSelected => selectedRectangle != null;

    private bool randomizeColorsRunning = false;

    public RectangleFactory rectangleFactory;

    public DropDownScaleController dropDownScaleController;
    
    public bool doMoveRectangle = false;


    public float snapDistance = 0.05f; // can be adjusted in editor

    public ParticleSystemController particleSystemController;

    public Solutions solutions;

    SequenceOfNController NController = new SequenceOfNController();
    public TextController textController;
    public int IdxN => NController.IdxN;
    public int GetN => NController.GetN();

    private Corner[] Corners => selectedRectangle.RectanglePerimeter.corners.Keys.ToArray();


    public List<Vector2> GetSelectedRectangleOverlappingCorners()
    {
        List<Vector2> positions = new List<Vector2>();

        foreach (var corner in Corners)
        {
            bool cornerFound = false;
            foreach (var rectanglePerimeter in rectanglePerimeters)
            {
                if (rectanglePerimeter.name == selectedRectangle.name || cornerFound) continue;
                foreach (var otherKey in rectanglePerimeter.corners.Keys)
                {
                    if (corner == otherKey || cornerFound) continue;
                    if (selectedRectangle.RectanglePerimeter.corners[corner] == rectanglePerimeter.corners[otherKey])
                    {
                        cornerFound = true;
                        positions.Add(rectanglePerimeter.corners[otherKey]);
                    }
                }
            }
        }

        return positions;
    }

    public void EmitParticleOnSelectedRectangleCorners()
    {
        particleSystemController.EmitAt(GetSelectedRectangleOverlappingCorners());
    }

    public void DropRectangle()
    {
        doMoveRectangle = false;
        textController.SetRectangleCommandText("press (m) to move rectangle");
    }



    public void AddRectangle()
    {
        SetSelectedRectangle(rectangleFactory.CreateRectangle());
    }

    public void UpdateRectangleCountText()
    {
        textController.SetRectangleCountText($"N = {rectangles.Count}. Solve N = {NController.GetN()}");
    }

    public void AddCloneRectangle()
    {
        if (IsAnyRectangleSelected)
        {
            SetSelectedRectangle(rectangleFactory.CreateClone(selectedRectangle));
        }
    }

    

    public void SelectRectangle(Vector2 mousePosition)
    {
        List<Rectangle> rectanglesOverlappedByMousePosition = new List<Rectangle>();
        // find the first rectangle in the list whose sprite renderer contains the position of the mouse and set it as selected rectangle
        foreach (var rectangle in rectangles)
        {
            if (rectangle.GetComponent<SpriteRenderer>().bounds.Contains(mousePosition)) // here I use GetComponent because this method is not called frequently
            {
                rectanglesOverlappedByMousePosition.Add(rectangle);
            }
        }

        if (rectanglesOverlappedByMousePosition.Count > 0)
        {
            int indexOfSelectedRectangle = rectangles.IndexOf(selectedRectangle);
            int indexOfNewSelectedRectangle = -1;
            int maxIndex = -1;
            foreach (var rectangle in rectanglesOverlappedByMousePosition)
            {
                int rectIndex = rectangles.IndexOf(rectangle);

                if (rectIndex < indexOfSelectedRectangle && (rectIndex > indexOfNewSelectedRectangle || indexOfNewSelectedRectangle == -1))
                {
                    indexOfNewSelectedRectangle = rectIndex;
                }

                if (rectIndex > maxIndex) maxIndex = rectIndex;
            }
            if (indexOfNewSelectedRectangle == -1) indexOfNewSelectedRectangle = maxIndex;
            SetSelectedRectangle(rectangles[indexOfNewSelectedRectangle]);
        }
        
    }


    public void DoRotateSelectedRectangle()
    {
        RotateSelectedRectangle();
    }
    public bool RotateSelectedRectangle()
    {
        if (IsAnyRectangleSelected)
        {
            selectedRectangle.transform.Rotate(new Vector3(0, 0, 90));
            selectedRectangle.RectanglePerimeter.SetCorners();
            return true;
        }
        return false;
    }

    // returns true if valid solution, false if not
    public bool CheckWin()
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


    public void MoveSelectedRectangle(Vector2 newPosition)
    {
        selectionCursor.SetPosition(newPosition);

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


                Vector2 rectangleCornerFromInputPosition = selectionCursor.GetPosition() + cornerOffset; // simulated corner position. Used instead of actual corner position so rectangle can snap and unsnap
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
        if (Vector2.Distance(newPosition, selectionCursor.GetPosition()) >
            (selectedRectangle.RectanglePerimeter.Bounds.extents.x < selectedRectangle.RectanglePerimeter.Bounds.extents.y ? selectedRectangle.RectanglePerimeter.Bounds.extents.x : selectedRectangle.RectanglePerimeter.Bounds.extents.y)
            )
        {
            selectedRectangle.transform.position = selectionCursor.GetPosition();
        }
        // otherwise set to the new calculated position
        else
        {
            selectedRectangle.transform.position = newPosition;

        }
        selectedRectangle.RectanglePerimeter.SetCorners();
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

    private void SetSelectedRectangle(Rectangle rectangle)
    {
        // Set sorting order of rectangles to newest on top. Then set selected rectangle above all rectangles
        for (int i = 0; i < rectangles.Count; i++)
        {
            rectangles[i].RectanglePerimeter.SpriteRenderer.sortingOrder = i;
        }
        rectangle.RectanglePerimeter.SpriteRenderer.sortingOrder = rectangles.Count;

        transform.position = rectangle.transform.position;
        selectedRectangle = rectangle;

        dropDownScaleController.SetValue(selectedRectangle.scaleDropdownValue);

        DropRectangle();
    }

    public void DoChangeRectangleInput()
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

    private void RemoveRectanglesFromRectanglePerimeters()
    {
        rectanglePerimeters.RemoveAll(x => x.name.ToUpper().Contains("RECTANGLE"));
    }


    public void AdvanceN()
    {
        solutions.AddSolution(NController.GetN(), rectangles);
        rectangles.RemoveRange(0, rectangles.Count);
        RemoveRectanglesFromRectanglePerimeters();

        NController.AdvanceN();
        ResetSquare();
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
            snapDistance = 0.2f * Mathf.Sqrt((float)dividend / divisor);

            particleSystemController.SetScale((float)dividend / divisor);

            selectedRectangle.scaleDropdownValue = change.value;
            selectedRectangle.SetScale(dividend, divisor);

            // scale selected rectangle cursor
            selectionCursor.transform.localScale = Vector2.one * 0.18f * (selectedRectangle.transform.localScale.y > selectedRectangle.transform.localScale.x ? selectedRectangle.transform.localScale.y : selectedRectangle.transform.localScale.x);
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
        textController.ResetRectangleCommand();
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

}
