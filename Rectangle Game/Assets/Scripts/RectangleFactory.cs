using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangleFactory : MonoBehaviour
{
    public GameObject rectanglePrefab; // assigned in editor from prefab folder
    public GameSquareInput gameSquareInput; // everytime a rectangle is created, it must be added to gameSquareInput so it is accounted for during snap and check win routines

    public Rectangle CreateRectangle()
    {
        Rectangle rectangle = Instantiate(rectanglePrefab).GetComponent<Rectangle>();
        rectangle.gameObject.name = $"Rectangle {Guid.NewGuid()}"; // Guid.NewGuid() so that every name is unique so that names can be compared to know when doing a correlation searched against all rectangle perimeters can know when trying to compare a rectangle to itself
        rectangle.InitRectangle();

        // set color to random color
        float minColorRange = 0.3f;
        float maxColorRange = 0.99f;
        rectangle.gameObject.SetSpriteRendererColor(
            new Color(UnityEngine.Random.Range(minColorRange, maxColorRange), 
            UnityEngine.Random.Range(minColorRange, maxColorRange), 
            UnityEngine.Random.Range(minColorRange, maxColorRange), 1));

        // track rectangle in gameSquareInput
        gameSquareInput.AddRectangle(rectangle);
        return rectangle;
    }
}
