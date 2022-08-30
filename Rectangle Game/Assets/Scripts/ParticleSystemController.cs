using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemController : MonoBehaviour
{
    [field: SerializeField]
    public ParticleSystem ParticleSystem { get; private set; }

    public void SetScale(float scale)
    {
        ParticleSystem.transform.localScale = Vector3.one * Mathf.Sqrt(scale);
    }


    public void EmitAt(List<Vector2> positions)
    {
        foreach (var pos in positions)
        {
            ParticleSystem.transform.position = pos;
            ParticleSystem.Emit(1);
        }
    }
}
