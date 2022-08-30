using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * inverts its rectanglePerimeter and adds itself to the rectanglePerimeters in GameSquareInput
 * Uses RectangleFactory to create rectangles and assign itself as the parent so that the scaling of the rectangle is relative
 */
public class GameSquare : MonoBehaviour
{
    public DebugSprites debugSprites;

    public RectangleController rectangleController;
    private SpriteRenderer SpriteRenderer { get; set; }

    
    public void Start()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();

        RectanglePerimeter rectanglePerimeter = GetComponent<RectanglePerimeter>();
        rectanglePerimeter.InitRectanglePerimeter();

        rectangleController.AddRectanglePerimeter(rectanglePerimeter);

        // switch top <-> bottom and left <-> right edges so rectangle snapper and check win routines can treat the inside of the square as the outside
        Vector2 tempRectangleCorner = rectanglePerimeter.corners[Corner.TopLeft];
        rectanglePerimeter.corners[Corner.TopLeft] = rectanglePerimeter.corners[Corner.BottomRight];
        rectanglePerimeter.corners[Corner.BottomRight] = tempRectangleCorner;
        tempRectangleCorner = rectanglePerimeter.corners[Corner.TopRight];
        rectanglePerimeter.corners[Corner.TopRight] = rectanglePerimeter.corners[Corner.BottomLeft];
        rectanglePerimeter.corners[Corner.BottomLeft] = tempRectangleCorner;

        //debugSprites.CreateDebugObject(rectanglePerimeter.corners[Corner.TopLeft]).SetSpriteRendererColor(Color.red);

    }

    public bool Contains(Vector3 point)
    {
        return SpriteRenderer.bounds.Contains(point);
    }

}
