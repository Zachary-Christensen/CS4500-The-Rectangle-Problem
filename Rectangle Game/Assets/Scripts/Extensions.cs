using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Extensions
{
    // mainly used after creating a debug sprite to change its color more easily
    public static void SetSpriteRendererColor(this GameObject gameObject, Color color)
    {
        gameObject.GetComponent<SpriteRenderer>().color = color;
    }


}
