using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChange : MonoBehaviour {

    private Color colorPrimary;
    private Color colorSecondary;
    private Material mat;
    public float m_Hue;
    float m_Saturation;
    float m_Value;

    // Use this for initialization
    void Start () {

        //Material mat = GetComponent<Renderer>().material;
        //m_Hue = 0.5f;
        //m_Saturation = 0.8f;
        //m_Value = 0.8f;

        //Color colorPrimary = Color.HSVToRGB(m_Hue, m_Saturation, m_Value);
        //Color colorSecondary = Color.HSVToRGB(m_Hue, m_Saturation / 4, m_Value);
        //mat.SetColor("_Color1", colorPrimary);
        //mat.SetColor("_Color2", colorSecondary);
    }

    // Update is called once per frame
    void Update(){

        //Material mat = GetComponent<Renderer>().material;

        //Color colorPrimary = Color.HSVToRGB(m_Hue, m_Saturation, m_Value);
        //Color colorSecondary = Color.HSVToRGB(m_Hue, m_Saturation / 4, m_Value);
        //mat.SetColor("_Color1", colorPrimary);
        //mat.SetColor("_Color2", colorSecondary);

    }
}
