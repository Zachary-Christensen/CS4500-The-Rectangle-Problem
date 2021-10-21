using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rectangle : MonoBehaviour
{
    public RectanglePerimeter RectanglePerimeter { get; private set; }
    public int scaleDivisor = 1;
    public int scaleDividend = 1;


    public void InitRectangle()
    {
        RectanglePerimeter = GetComponent<RectanglePerimeter>();
        RectanglePerimeter.InitRectanglePerimeter();
        RectanglePerimeter.SetCorners();
    }

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
