using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;
public class Main : MonoBehaviour
{
    //public Controller.UIGameObjects UI;
    public GameObject passLevelConfetti;
    public GameObject passLevelStars;
    [SerializeField] public ParticleSystem explosion;
    public AudioSource coinAudioSource;
    public AudioClip coinSound, gameOverSound;
    public GameObject GetCoinParticelObject;
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
            AdController.current.bannerView.Show();
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
            Instantiate(GetCoinParticelObject, transform.position, Quaternion.identity);
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
            Instantiate(GetCoinParticelObject, transform.position, Quaternion.identity);
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
                    AdController.current.bannerView.Show();
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
    int adCount = 0;
    IEnumerator PassLevel()
    {
        passLevelConfetti.SetActive(true);
        passLevelStars.SetActive(true);
        yield return new WaitForSeconds(4f);
        passLevelConfetti.SetActive(false);
        passLevelStars.SetActive(false);
        adCount++;
        if (adCount % 3 == 0)
            AdController.current.interstitial.Show();
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
        adCount++;
        if (Controller.freeModeFlag)
        {
            if (adCount % 3 == 0)
                AdController.current.interstitial.Show();
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
            StopCoroutine(Controller.create);
            score = 0;
        }
        else
        {
            if (!isAdmob)
            {
                if(adCount % 3 == 0)
                    AdController.current.interstitial.Show();
                MainScreenSet(true);
                StopAllCoroutines();
                StopCoroutine(Controller.create);
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
        if (AdController.current.rewardedAd.IsLoaded())
        {
            AdController.current.rewardedAd.Show();
            //playerprefs içinde int bir count olacak nerede ölürse ölsün 2 ölmeden sonra reklam gelecek.
            AdController.current.bannerView.Hide();
            StopCoroutine(stop);
            isAdmob = true;
            GameOverParameters();
            isAdmob = false;
            //continueObject.SetActive(false);
        }
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
