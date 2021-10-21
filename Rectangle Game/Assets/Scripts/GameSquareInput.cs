using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GameSquareInput : MonoBehaviour
{
    public GameSquare gameSquare;
    public float snapDistance = 0.05f;
    private readonly List<Rectangle> rectangles = new List<Rectangle>();
    private readonly List<RectanglePerimeter> rectanglePerimeters = new List<RectanglePerimeter>();
    private Rectangle selectedRectangle;

    public DebugSprites debugSprites;

    public bool doMoveRectangle = false;
    public bool doRotateRectangle = false;
    public bool doScaleRectangle = false;

    private void Update()
    {
        if (GetMouseDown())
        {
            SelectRectangle();
        }

        if (DoAddRectangle())
        {
            SetSelectedRectangle(gameSquare.AddRectangle());
        }


        if (selectedRectangle != null)
        {
            DoRectangleTransformInput();
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
        if (gameSquare.SpriteRenderer.bounds.Contains(mousePosition))
        {
            foreach (var rectangle in rectangles)
            {
                if (rectangle.GetComponent<SpriteRenderer>().bounds.Contains(mousePosition))
                {
                    SetSelectedRectangle(rectangle);
                    break;
                }
            }
        }
    }

    private void DoRectangleTransformInput()
    {
        if (Input.GetKeyDown(KeyCode.M) && !doRotateRectangle && !doScaleRectangle)
        {
            doMoveRectangle = true;
        }
        else if (Input.GetKeyDown(KeyCode.R) && !doMoveRectangle && !doScaleRectangle)
        {
            doRotateRectangle = true;
            selectedRectangle.transform.Rotate(new Vector3(0, 0, 90));
            selectedRectangle.RectanglePerimeter.SetCorners();
        }
        else if (Input.GetKeyDown(KeyCode.S) && !doMoveRectangle && !doRotateRectangle)
        {
            doScaleRectangle = true;
        }


        if (Input.GetKeyDown(KeyCode.D))
        {
            doMoveRectangle = false;
            doRotateRectangle = false;
            doScaleRectangle = false;
        }

        if (doMoveRectangle)
        {
            Vector2 newPosition = GetMousePosition();
            if (gameSquare.SpriteRenderer.bounds.Contains(newPosition)) // only move cursor if will stay inside gameSquare
            {
                transform.position = newPosition;
            }

            MoveSelectedRectangle();
        }
        if (doScaleRectangle)
        {
            ScaleSelectedRectangle();
        }
    }

    private bool CheckWin()
    {
        List<RectanglePerimeter> tempRectanglePerimeters = rectanglePerimeters;
        foreach (var rectanglePerimeter in rectanglePerimeters)
        {
            int numberOfCornersMatched = 0;
            foreach (var otherRectanglePerimeter in tempRectanglePerimeters)
            {
                if (rectanglePerimeter.gameObject.name == otherRectanglePerimeter.gameObject.name) continue;

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
            if (numberOfCornersMatched < 4)
            {
                return false;
            }
        }
        return true;
    }

    private void ScaleSelectedRectangle()
    {
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

    private static bool DoCheckWin()
    {
        return Input.GetKeyDown(KeyCode.W);
    }

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
        Vector2 newPosition = transform.position;
        bool closeToOtherCorner = false;

        foreach (var rectanglePerimeter in rectanglePerimeters)
        {
            if (rectanglePerimeter.gameObject.name == selectedRectangle.gameObject.name || closeToOtherCorner)
            {
                continue;
            }

            float minDistance = 1000;
            Corner closestCorner = Corner.Null;
            Vector2 closestPoint = Vector2.zero;
            Vector2 closestPointGameSquare = Vector2.zero;
            for (int i = 0; i < 4; i++)
            {
                Corner corner = (Corner)i;
                Vector2 cornerOffset = selectedRectangle.RectanglePerimeter.Bounds.extents;
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

                Vector2 rectangleCornerFromInputPosition = transform.position + (Vector3)cornerOffset;
                Vector2 closestPointOnRectanglePerimeter = rectanglePerimeter.SpriteRenderer.bounds.ClosestPoint(rectangleCornerFromInputPosition);

                float distance = Vector2.Distance(closestPointOnRectanglePerimeter, rectangleCornerFromInputPosition);

                if (!rectanglePerimeter.Bounds.Contains(rectangleCornerFromInputPosition) && distance < snapDistance && distance < minDistance)
                {
                    minDistance = distance;
                    closestCorner = corner;
                    closestPoint = closestPointOnRectanglePerimeter;

                    foreach (var cornerKey in selectedRectangle.RectanglePerimeter.corners.Keys)
                    {
                        foreach (var otherCornerKey in selectedRectangle.RectanglePerimeter.corners.Keys)
                        {
                            if (cornerKey == otherCornerKey) continue;
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
        if (Vector2.Distance(newPosition, transform.position) > 
            (selectedRectangle.RectanglePerimeter.Bounds.extents.x < selectedRectangle.RectanglePerimeter.Bounds.extents.y ? selectedRectangle.RectanglePerimeter.Bounds.extents.x : selectedRectangle.RectanglePerimeter.Bounds.extents.y)
            )
        {
            selectedRectangle.transform.position = transform.position;
        }
        else
        {
            selectedRectangle.transform.position = newPosition;

        }
        selectedRectangle.RectanglePerimeter.SetCorners();
    }

    public bool DoAddRectangle()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    public Vector2 GetMousePosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public bool GetMouseDown()
    {
        return Input.GetMouseButtonDown(0);
    }

    public void AddRectangle(Rectangle rectangle)
    {
        rectangles.Add(rectangle);
        rectanglePerimeters.Add(rectangle.RectanglePerimeter);
    }

    public void AddRectanglePerimeter(RectanglePerimeter rectanglePerimeter)
    {
        rectanglePerimeters.Add(rectanglePerimeter);
    }

    private void SetSelectedRectangle(Rectangle rectangle)
    {
        transform.position = rectangle.transform.position;
        selectedRectangle = rectangle;
    }

}