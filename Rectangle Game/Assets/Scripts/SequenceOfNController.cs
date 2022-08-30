using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SequenceOfNController
{
    public int IdxN { get; set; } = 0;
    private readonly int[] sequenceOfN = new int[] { 2, 5, 8, 6, 10, 9, 7 };
    public int NCount => sequenceOfN.Length;

    public int GetN()
    {
        return sequenceOfN[IdxN];
    }
}
