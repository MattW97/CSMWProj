using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanningTextureScript : MonoBehaviour
{
    private float ScrollX = 0.0f;
    private float ScrollY = 1.0f;

    void Update()
    {
        float OffsetX = Time.time * ScrollX;
        float OffsetY = Time.time * ScrollY;
        GetComponent<Renderer>().material.mainTextureOffset = new Vector2(OffsetX, OffsetY);
    }
}
