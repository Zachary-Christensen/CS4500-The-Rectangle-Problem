using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSquare : MonoBehaviour
{
    public DebugSprites debugSprites;
    public RectangleFactory rectangleFactory;
    public GameSquareInput gameSquareInput;
    public SpriteRenderer SpriteRenderer { get; private set; }


    public void Start()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();

        RectanglePerimeter rectanglePerimeter = GetComponent<RectanglePerimeter>();
        rectanglePerimeter.InitRectanglePerimeter();

        gameSquareInput.AddRectanglePerimeter(rectanglePerimeter);

        // switch top <-> bottom and left <-> right edges so rectangle snapper can treat the inside of the square as the outside
        Vector2 tempRectangleCorner = rectanglePerimeter.corners[Corner.TopLeft];
        rectanglePerimeter.corners[Corner.TopLeft] = rectanglePerimeter.corners[Corner.BottomRight];
        rectanglePerimeter.corners[Corner.BottomRight] = tempRectangleCorner;
        tempRectangleCorner = rectanglePerimeter.corners[Corner.TopRight];
        rectanglePerimeter.corners[Corner.TopRight] = rectanglePerimeter.corners[Corner.BottomLeft];
        rectanglePerimeter.corners[Corner.BottomLeft] = tempRectangleCorner;

        debugSprites.CreateDebugObject(rectanglePerimeter.corners[Corner.TopLeft]).SetSpriteRendererColor(Color.red);

    }

    public Rectangle AddRectangle()
    {
        Rectangle rectangle = rectangleFactory.CreateRectangle();
        rectangle.transform.SetParent(transform);
        rectangle.transform.localPosition = Vector3.zero;
        rectangle.SetScale(1, 1);
        return rectangle;
    }

}
