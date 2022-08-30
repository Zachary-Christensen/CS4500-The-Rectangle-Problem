using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionCursor : MonoBehaviour
{
    public void SetPosition(Vector2 newPosition)
    {
        transform.position = newPosition;
    }

    public Vector2 GetPosition()
    {
        return transform.position;
    }
}
