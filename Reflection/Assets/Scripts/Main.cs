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
    public static float speed = 200;
    float gameOverSpeed;
    float gameOverEnemySpeed;
    Rigidbody rigidbody;
    public static bool isAdmob = false;
    public GameObject continueObject;
    int score = 0;
    Coroutine stop;
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
            other.gameObject.GetComponent<SphereCollider>().enabled = false;
            coinAudioSource.PlayOneShot(gameOverSound, 0.7f);
            explosion.Play();
            gameOverSpeed = speed;
            gameOverEnemySpeed = Controller.enemySpeed;
            speed = 0;
            Controller.enemySpeed = 0;
            rigidbody.useGravity = true;
            rigidbody.isKinematic = false;
            stop = StartCoroutine(GameOver());
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
            //Freemode
            else
            {
                score++;
                Controller.UIStatic.freeModeMaxScore.text = score.ToString();
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
            //Free mode
            else
            {
                score += 2;
                Controller.UIStatic.freeModeMaxScore.text = score.ToString();
            }
        }
    }
    IEnumerator PassLevel()
    {
        yield return new WaitForSeconds(3f);

        Controller.nextLevelX.SetActive(false);
        MainScreenSet(true);
        StopAllCoroutines();
        speed = gameOverSpeed;

        foreach (GameObject item in Controller.spheres)
        {
            Destroy(item);
        }
    }
    IEnumerator GameOver()
    {
        if (!Controller.freeModeFlag)
        {
            continueObject.SetActive(true);
            yield return new WaitForSeconds(10f);
        }
        else
            yield return new WaitForSeconds(2f);
        GameOverParameters();
    }

    private void GameOverParameters()
    {
        if (Controller.freeModeFlag)
        {
            MainScreenSet(false);
            StopAllCoroutines();
            if (score > Controller.maxScore)
            {
                Controller.maxScore = score;
                PlayerPrefs.SetInt("maxScore", score);
                Controller.UIStatic.freeModeMaxScore.text = score.ToString();
            }
            else
            {
                Controller.UIStatic.freeModeMaxScore.text = Controller.maxScore.ToString();
            }
            score = 0;
        }
        else
        {
            if (!isAdmob)
            {
                MainScreenSet(true);
                StopAllCoroutines();
            }
            else
            {
                Controller.enemySpeed = gameOverEnemySpeed;
            }
        }
        #region Character rigidbody process
        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;
        rigidbody.transform.position = new Vector3(2.5f, 0, 0);
        rigidbody.transform.rotation = Quaternion.identity;
        #endregion

        speed = gameOverSpeed;
        ContinueScript.sliderValue = 0;

        foreach (GameObject item in Controller.spheres)
        {
            Destroy(item);
        }
        continueObject.SetActive(false);
    }

    public void AdButton()
    {
        //uzun reklam videosu buraya gelecek.
        //playerprefs i�inde int bir count olacak nerede �l�rse �ls�n 2 �lmeden sonra reklam gelecek.
        StopCoroutine(stop);
        isAdmob = true;
        GameOverParameters();
        isAdmob = false;
        //continueObject.SetActive(false);
    }
    public void NoThanksButton()
    {
        isAdmob = false;
        GameOverParameters();
        //continueObject.SetActive(false);

    }
    public void MainScreenSet(bool stagesFlag)
    {
        Controller.UIStatic.startPanel.SetActive(true);
        Controller.UIStatic.collectGreen.SetActive(false);
        if(stagesFlag)
            Controller.UIStatic.stagesButton.SetActive(true);
        else
            Controller.UIStatic.stagesButton.SetActive(false);
    }
}
