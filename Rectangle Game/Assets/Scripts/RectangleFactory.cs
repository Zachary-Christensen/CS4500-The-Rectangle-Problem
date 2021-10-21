using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangleFactory : MonoBehaviour
{
    public GameObject rectanglePrefab;
    public GameSquareInput gameSquareInput;

    public Rectangle CreateRectangle()
    {
        Rectangle rectangle = Instantiate(rectanglePrefab).GetComponent<Rectangle>();
        rectangle.gameObject.name = $"Rectangle {Guid.NewGuid()}";
        rectangle.InitRectangle();

        float minColorRange = 0.3f;
        float maxColorRange = 0.99f;
        rectangle.gameObject.SetSpriteRendererColor(
            new Color(UnityEngine.Random.Range(minColorRange, maxColorRange), 
            UnityEngine.Random.Range(minColorRange, maxColorRange), 
            UnityEngine.Random.Range(minColorRange, maxColorRange), 1));

        gameSquareInput.AddRectangle(rectangle);
        return rectangle;
    }
}
