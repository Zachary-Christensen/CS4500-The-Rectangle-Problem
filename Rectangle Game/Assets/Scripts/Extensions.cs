using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Extensions
{
    public static void SetSpriteRendererColor(this GameObject gameObject, Color color)
    {
        gameObject.GetComponent<SpriteRenderer>().color = color;
    }


}
