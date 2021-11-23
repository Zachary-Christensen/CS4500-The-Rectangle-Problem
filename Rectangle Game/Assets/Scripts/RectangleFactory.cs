using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangleFactory : MonoBehaviour
{
    public GameObject rectanglePrefab; // assigned in editor from prefab folder
    public GameSquareInput gameSquareInput; // everytime a rectangle is created, it must be added to gameSquareInput so it is accounted for during snap and check win routines
    public Transform rectangleParent; // gameSquare
    private int orderInLayerCounter = 0;

    public Rectangle CreateRectangle()
    {
        Rectangle rectangle = Instantiate(rectanglePrefab).GetComponent<Rectangle>();
        rectangle.gameObject.name = $"Rectangle {Guid.NewGuid()}"; // Guid.NewGuid() so that every name is unique so that names can be compared to know when doing a correlation searched against all rectangle perimeters can know when trying to compare a rectangle to itself
        rectangle.InitRectangle();

        rectangle.gameObject.SetSpriteRendererColor(UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
        rectangle.gameObject.GetComponent<SpriteRenderer>().sortingOrder = orderInLayerCounter++;

        rectangle.transform.SetParent(rectangleParent); // set gameSquare as parent of rectangle
        rectangle.transform.localPosition = Vector3.zero; // move to center
        rectangle.SetScale(1, 1); // setScale after setting transform parent because setting transform parent changes the scale

        // track rectangle in gameSquareInput
        gameSquareInput.AddRectangle(rectangle);
        gameSquareInput.UpdateRectangleCountText();
        return rectangle;
    }

    public Rectangle CreateClone(Rectangle rectangle)
    {
        Rectangle rectangle1 = CreateRectangle();
        rectangle1.scaleDividend = rectangle.scaleDividend;
        rectangle1.scaleDivisor = rectangle.scaleDivisor;
        rectangle1.scaleDropdownValue = rectangle.scaleDropdownValue;
        rectangle1.SetScale(rectangle1.scaleDividend, rectangle1.scaleDivisor);
        rectangle1.transform.localRotation = rectangle.transform.localRotation;
        return rectangle1;
    }
}
