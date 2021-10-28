using System;
using UnityEngine;

/*
 * Creates sprites at positions to aide visual debugging when working with coordinates
 */
public class DebugSprites : MonoBehaviour
{

    public GameObject debugPrefab;


    public GameObject CreateDebugObject()
    {
        GameObject gameObject = Instantiate(debugPrefab, transform);
        gameObject.name = $"DebugObject {Guid.NewGuid()}";
        return gameObject;
    }
    public GameObject CreateDebugObject(Vector2 position)
    {
        GameObject gameObject = CreateDebugObject();
        gameObject.transform.position = position;
        return gameObject;
    }

}

