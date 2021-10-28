using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rectangle : MonoBehaviour
{
    public RectanglePerimeter RectanglePerimeter { get; private set; } // used to store a reference to RectanglePerimeter because GetComponent is expensive and RectanglePerimeter is used frequently during move routine
    public int scaleDivisor = 1;
    public int scaleDividend = 1;

    // in place of MonoBehaviour Start method so objects with this script can be created and initialized in the same place
    public void InitRectangle()
    {
        RectanglePerimeter = GetComponent<RectanglePerimeter>();
        RectanglePerimeter.InitRectanglePerimeter();
        RectanglePerimeter.SetCorners();
    }

    // sets scale of rectangle as ratio of (1, 1/2)
    // when (1, 1/2), a rectangle is half the gameSquare because it is a child of the gameSquare object and the (1, 1/2) refers to its local scale. Also the sprites used by both objects have the same resolution and ppi
    public void SetScale(int scaleDividend, int scaleDivisor)
    {
        if (scaleDividend == 0) scaleDividend = 1;
        if (scaleDivisor == 0) scaleDivisor = 1;

        this.scaleDividend = scaleDividend;
        this.scaleDivisor = scaleDivisor;
        transform.localScale = new Vector3(1, 1 / 2f, 0) * this.scaleDividend / this.scaleDivisor;

        RectanglePerimeter.SetCorners();
    }

}
