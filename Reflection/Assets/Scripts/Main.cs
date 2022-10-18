using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    //public Controller.UIGameObjects UI;
    Touch touch;
    bool wayFlag = true;
    public Text score;
    public static float speed = 200;
    void Start()
    {
        transform.RotateAround(Vector3.zero, transform.forward, speed * Time.deltaTime);
    }
    void Update()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
                wayFlag = wayFlag ? false : true;
        }
        Move();
    }
    private void Move()
    {
        if (/*(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Began) && */wayFlag)
        {
            transform.RotateAround(Vector3.zero, transform.forward, speed * Time.deltaTime);
        }
        else if (/*(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Began) && */!wayFlag)
        {
            transform.RotateAround(Vector3.zero, -transform.forward, speed * Time.deltaTime);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "enemy")
        {
            speed = 0;
            Controller.enemySpeed = 0;
            StartCoroutine(GameOver());
        }
        else if (other.gameObject.tag == "coin")
        {
            Destroy(other.gameObject);
            if (!Controller.freeModeFlag)
            {
                if (Convert.ToInt32(Controller.UIStatic.collectMissionGreenMaxScore.text) != 0)
                {
                    Controller.UIStatic.collectMissionGreenMaxScore.text = (Convert.ToInt32(Controller.UIStatic.collectMissionGreenMaxScore.text) - 1).ToString();
                }
                //passLevel maxBallCount
                if (Convert.ToInt32(Controller.UIStatic.collectMissionGreenMaxScore.text) == 0 && Convert.ToInt32(Controller.UIStatic.collectMissionBonusBallMaxScore.text) == 0)
                {
                    speed = 0;
                    Controller.enemySpeed = 0;
                    Controller.nextEnableButton.enabled = true;
                    PlayerPrefs.SetInt("Level-" + Controller.activeLevel, Controller.activeLevel);
                    StartCoroutine(PassLevel());
                }

            }
        }
        else if (other.gameObject.tag == "bonusball")
        {
            Destroy(other.gameObject);
            if (!Controller.freeModeFlag)
            {
                if (Convert.ToInt32(Controller.UIStatic.collectMissionBonusBallMaxScore.text) != 0)
                {
                    Controller.UIStatic.collectMissionBonusBallMaxScore.text = (Convert.ToInt32(Controller.UIStatic.collectMissionBonusBallMaxScore.text) - 1).ToString();
                }
                //passLevel maxBallCount
                if (Convert.ToInt32(Controller.UIStatic.collectMissionGreenMaxScore.text) == 0 && Convert.ToInt32(Controller.UIStatic.collectMissionBonusBallMaxScore.text) == 0)
                {
                    speed = 0;
                    Controller.enemySpeed = 0;
                    Controller.nextEnableButton.enabled = true;
                    PlayerPrefs.SetInt("Level-" + Controller.activeLevel, Controller.activeLevel);
                    StartCoroutine(PassLevel());
                }
            }
        }
    }
    IEnumerator PassLevel()
    {
        yield return new WaitForSeconds(3f);
        Controller.nextLevelX.SetActive(false);
        Controller.UIStatic.startPanel.SetActive(true);
        Controller.UIStatic.stagesButton.SetActive(true);
        Controller.UIStatic.collectGreen.SetActive(false);
        Controller.UIStatic.collectBonusBall.SetActive(false);
        StopAllCoroutines();
        score.text = "0";
        foreach (GameObject item in Controller.spheres)
        {
            Destroy(item);
        }
        //Controller.enemySpeed = 10;
    }
    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(3f);
        //Controller.nextLevelX.SetActive(false);

        if (Controller.freeModeFlag)
        {
            Controller.UIStatic.startPanel.SetActive(true);
            Controller.UIStatic.freeModeButtons.SetActive(true);
            Controller.UIStatic.collectGreen.SetActive(false);
            Controller.UIStatic.collectBonusBall.SetActive(false);
            StopAllCoroutines();
            score.text = "0";
        }
        else
        {
            Controller.UIStatic.startPanel.SetActive(true);
            Controller.UIStatic.stagesButton.SetActive(true);
            Controller.UIStatic.collectGreen.SetActive(false);
            Controller.UIStatic.collectBonusBall.SetActive(false);
            StopAllCoroutines();
            score.text = "0";
        }
        foreach (GameObject item in Controller.spheres)
        {
            Destroy(item);
        }
        //Controller.enemySpeed = 10;
    }
}
