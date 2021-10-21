using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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

