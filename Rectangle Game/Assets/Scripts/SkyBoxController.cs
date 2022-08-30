using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkyBoxController : MonoBehaviour
{
    public List<Material> skyboxes;
    public SequenceOfNController sequenceOfNController;

    private void Awake()
    {
        RenderSettings.skybox = skyboxes[0];

    }


    public void SetSkyBox()
    {
        RenderSettings.skybox = skyboxes[sequenceOfNController.IdxN];
    }
}
