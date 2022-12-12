using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ImageChangerUI : MonoBehaviour
{
    [SerializeField] private Image myImage;
    [SerializeField] Color[] myColors;
    int index = 0;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < myColors.Length; i++)
        {
            myColors[i].a = 1;
        }
    }
    //sadece SetActive(true) da çalýþýyor.
    private void FixedUpdate()
    {
        if (index >= myColors.Length)
        {
            index = 0;
        }
        Debug.Log(index);
        myImage.color = myColors[index];
        index++;
    }
}
