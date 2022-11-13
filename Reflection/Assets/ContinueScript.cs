using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContinueScript : MonoBehaviour
{
    public GameObject continueObject;
    public Slider slider;
    public Button noThanks;
    public Button adButton;
    public static float sliderValue=0;
    private void Start()
    {
        slider.maxValue = 1;
        slider.minValue = 0;
    }

    private void Update()
    {
        slider.value = sliderValue;
        sliderValue += 0.1f * Time.deltaTime;
        if (slider.value>=slider.maxValue || Main.isAdmob)
        {
            continueObject.SetActive(false);
            slider.value = 0;
        }
    }
}
