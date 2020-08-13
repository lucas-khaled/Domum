using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class RotateMundo : MonoBehaviour
{
    [SerializeField]
    private UIParticleSystem uiPart;
    [SerializeField]
    private Vector2 tiling = Vector2.one;
    [SerializeField]
    private float frameRate = 30;

    private float xOffset = 0,yOffset = 1;
    private Vector2 rate;

    void Start()
    {
        SetTiles();
        InvokeRepeating("ScrollMap", 0.1f, 1 /frameRate);
    }

    void SetTiles()
    {
        uiPart.material.SetTextureOffset("_MainTex", Vector2.up);
        rate = new Vector2(1 / tiling.x, -1/tiling.y);
    }

    void ScrollMap()
    {
        if (xOffset >= 1 - rate.x)
            yOffset = (yOffset >= 1 / tiling.y) ? yOffset + rate.y : 1;

        xOffset = (xOffset < 1-rate.x) ? xOffset + rate.x : 0;

        uiPart.material.SetTextureOffset("_MainTex", new Vector2(xOffset, yOffset));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
