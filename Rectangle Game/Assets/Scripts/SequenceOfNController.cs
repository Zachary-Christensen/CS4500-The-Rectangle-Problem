using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SequenceOfNController : MonoBehaviour
{
    public int IdxN { get; private set; } = 0;
    private readonly int[] sequenceOfN = new int[] { 2, 5, 8, 6, 10, 9, 7 };
    public int NCount => sequenceOfN.Length;

    public int GetN()
    {
        return sequenceOfN[IdxN];
    }

    public void AdvanceN()
    {
        if (IdxN < sequenceOfN.Length - 1) IdxN++; // do not go past last solution in sequence
    }

}
