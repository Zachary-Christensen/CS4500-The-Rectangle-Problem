using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextController : MonoBehaviour
{

    public Text winText; // currently the set as the same object as rectangleCommandText
    public Text rectangleCount;
    public Text rectangleCommandText; // tells keyboard key to move or drop rectangle
    public Text topText; // writes the N being solved for and the current number of rectangles on the square

    public void ResetRectangleCommand()
    {
        rectangleCommandText.text = "Welcome to the Rectangle Game";
    }

    public void SetWinText(string text)
    {
        winText.text = text;
    }

    public void SetRectangleCountText(string text)
    {
        rectangleCount.text = text;
    }

    public void SetRectangleCommandText(string text)
    {
        rectangleCommandText.text = text;
    }
}
