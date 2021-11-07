using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Solutions : MonoBehaviour
{
    private Dictionary<int, List<Rectangle>> solutions = new Dictionary<int, List<Rectangle>>();

    public void AddSolution(int N, List<Rectangle> solution)
    {
        solutions.Add(N, new List<Rectangle>());
        solution.ForEach(x => { solutions[N].Add(x); x.GetComponent<SpriteRenderer>().sortingLayerName = "Debug"; }); // set to layer above rectangle so that when the solution is shown, rectangles from current solution do not have to be hidden
    }

    public void ShowSolution(int N)
    {
        foreach (var key in solutions.Keys)
        {
            if (key != N)
            {
                solutions[key].ForEach(x => x.gameObject.SetActive(false));
            }
            else
            {
                solutions[key].ForEach(x => x.gameObject.SetActive(true));
            }
        }
    }

    public void HideSolutions()
    {
        foreach (var key in solutions.Keys)
        {
            solutions[key].ForEach(x => x.gameObject.SetActive(false));
        }
    }

}
