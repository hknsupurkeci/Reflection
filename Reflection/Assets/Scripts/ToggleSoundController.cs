using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleSoundController : MonoBehaviour
{
    [SerializeField] GameObject soundOnButton;
    [SerializeField] GameObject soundOffButton;
    [SerializeField] AudioSource backGroundMusic;
    public void Open()
    {
        backGroundMusic.Stop();
        soundOffButton.SetActive(true);
        soundOnButton.SetActive(false);
    }
    public void Close()
    {
        backGroundMusic.Play();
        soundOnButton.SetActive(true);
        soundOffButton.SetActive(false);
    }
}