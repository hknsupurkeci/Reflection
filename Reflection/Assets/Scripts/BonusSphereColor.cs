using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusSphereColor : MonoBehaviour
{
    MeshRenderer meshRenderer;
    [SerializeField] [Range(0f, 5f)] float lerpTime;
    [SerializeField] Color[] myColors;
    int colorIndex = 0;
    float t = 0f;
    int len;
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        len = myColors.Length;
    }

    void Update()
    {
        meshRenderer.material.color = Color.Lerp(meshRenderer.material.color, myColors[colorIndex], lerpTime * Time.deltaTime);
        t = Mathf.Lerp(t, 5f, lerpTime * Time.deltaTime);
        if (t > .9f)
        {
            t = 0f;
            colorIndex++;
            colorIndex = (colorIndex >= len) ? 0 : colorIndex;
        }
    }
}