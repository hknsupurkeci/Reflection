using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    //public Controller.UIGameObjects UI;
    [SerializeField] public ParticleSystem explosion;
    public AudioSource coinAudioSource;
    public AudioClip coinSound, gameOverSound;
    Touch touch;
    bool wayFlag = true;
    public Text score;
    public static float speed = 200;
    Rigidbody rigidbody;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
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
        if (wayFlag)
        {
            transform.RotateAround(Vector3.zero, transform.forward, speed * Time.deltaTime);
        }
        else if (!wayFlag)
        {
            transform.RotateAround(Vector3.zero, -transform.forward, speed * Time.deltaTime);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "enemy")
        {
            coinAudioSource.PlayOneShot(gameOverSound, 0.4f);
            speed = 0;
            Controller.enemySpeed = 0;
            explosion.Play();
            rigidbody.useGravity = true;
            rigidbody.isKinematic = false;
            StartCoroutine(GameOver());
        }
        else if (other.gameObject.tag == "coin")
        {
            coinAudioSource.PlayOneShot(coinSound, 0.7f);
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
            coinAudioSource.PlayOneShot(coinSound, 0.7f);
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
        StopAllCoroutines();
        score.text = "0";
        foreach (GameObject item in Controller.spheres)
        {
            Destroy(item);
        }
    }
    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(3f);
        
        if (Controller.freeModeFlag)
        {
            Controller.UIStatic.startPanel.SetActive(true);
            Controller.UIStatic.freeModeButtons.SetActive(true);
            Controller.UIStatic.collectGreen.SetActive(false);
            StopAllCoroutines();
            score.text = "0";
        }
        else
        {
            Controller.UIStatic.startPanel.SetActive(true);
            Controller.UIStatic.stagesButton.SetActive(true);
            Controller.UIStatic.collectGreen.SetActive(false);
            StopAllCoroutines();
            score.text = "0";
        }
        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;
        rigidbody.transform.position = new Vector3(2.5f, 0, 0);
        rigidbody.transform.rotation = Quaternion.identity;
        speed = 200;
        foreach (GameObject item in Controller.spheres)
        {
            Destroy(item);
        }
    }
}
