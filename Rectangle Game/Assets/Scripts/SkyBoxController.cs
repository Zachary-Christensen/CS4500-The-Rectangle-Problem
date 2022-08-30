using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkyBoxController : MonoBehaviour
{
    public List<Material> skyboxes;
    public RectangleController rectangleController;

    private void Awake()
    {
        RenderSettings.skybox = skyboxes[0];

    }


    public void SetSkyBox()
    {
        RenderSettings.skybox = skyboxes[rectangleController.IdxN];
    }
}
