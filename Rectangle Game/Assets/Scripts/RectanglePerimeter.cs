
using System.Collections.Generic;
using UnityEngine;

/*
 * Used to track corners and bounds of rectangles and the gameSquare
 */
public class RectanglePerimeter : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer { get; private set; }
    public Dictionary<Corner, Vector2> corners;
    public Bounds Bounds { get; private set; }

    // in place of MonoBehaviour Start method so objects with this script can be created and initialized in the same place
    public void InitRectanglePerimeter()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();

        corners = new Dictionary<Corner, Vector2>
        {
            { Corner.TopLeft, Vector2.zero },
            { Corner.TopRight, Vector2.zero },
            { Corner.BottomRight, Vector2.zero },
            { Corner.BottomLeft, Vector2.zero }
        };

        SetCorners();
    }

    // called when corners need to be updated. Rotation does not affect this because the values from bounds still correspond to the intended corner
    public void SetCorners()
    {
        Bounds = SpriteRenderer.bounds;

        corners[Corner.TopLeft] = new Vector2(Bounds.min.x, Bounds.max.y);
        corners[Corner.TopRight] = new Vector2(Bounds.max.x, Bounds.max.y);
        corners[Corner.BottomRight] = new Vector2(Bounds.max.x, Bounds.min.y);
        corners[Corner.BottomLeft] = new Vector2(Bounds.min.x, Bounds.min.y);
    }



    // used during debugging
    public override string ToString()
    {
        return $"Corners: tl{corners[Corner.TopLeft]}, tr{corners[Corner.TopRight]}, br{corners[Corner.BottomRight]}, bl{corners[Corner.BottomLeft]}";
    }
}
