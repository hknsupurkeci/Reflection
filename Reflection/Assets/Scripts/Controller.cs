using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public enum Modes { Easy, Normal, Hard, VeryHard};

public class Controller : MonoBehaviour
{
    [System.Serializable] public class Levels
    {
        public Modes mode;
        public Material skyboxMaterial;
        public GameObject levelX;
        public Button enableButton;
        public float characterSpeed;
        public float enemySpeed;
        public float gameTime;
        public int greenBallCount;
        public int bonusBallCount;
        public string levelId;
    }

    [System.Serializable]
    public class UIGameObjects
    {
        public GameObject backgroundButtons;
        public GameObject startButtons;
        public GameObject startPanel;
        public GameObject freeModeButtons;
        public GameObject stagesButton;
        public GameObject freeModMaxScoreButton;
        public GameObject collectGreen;
        public Text collectMissionGreenMaxScore;
        public Text collectMissionBonusBallMaxScore;
        public Text freeModeMaxScore;
    }
    #region Variables
    public List<Levels> levels;
    public List<GameObject> positionRight = new List<GameObject>();
    public List<GameObject> positionLeft = new List<GameObject>();
    public GameObject positionUpRight;
    public GameObject positionUpLeft;
    public GameObject positionDownRight;
    public GameObject positionDownLeft;

    public List<Material> skyboxes = new List<Material>();
    public List<GameObject> sphere;

    public UIGameObjects UI;
    public static UIGameObjects UIStatic;

    public static GameObject nextLevelX;
    public static Button nextEnableButton;
    public static List<GameObject> spheres = new List<GameObject>();
    public static float enemySpeed = 12f;
    public static float gameTime;
    public static int greenBallCount = 0;
    public static int bonusBallCount = 0;
    public static int activeLevel = 0;
    public static bool endFlag = false;
    public static bool freeModeFlag = false;
    public static int maxScore = 0;
    public static Coroutine create;
    #endregion

    #region Sphere Create
    private void Awake()
    {
        GameObject.FindObjectOfType<AdController>().InitializeAds();
        //PlayerPrefs.DeleteAll();
        maxScore = PlayerPrefs.GetInt("maxScore", 0);
        
        System.Random random = new System.Random();
        RenderSettings.skybox = skyboxes[random.Next(0, skyboxes.Count)];
        DynamicGI.UpdateEnvironment();
        UIStatic = UI;
        UIStatic.freeModeMaxScore.text = maxScore.ToString();
        for (int i = 1; i < levels.Count; i++)
        {
            if (PlayerPrefs.GetInt("Level-" + levels[i-1].levelId, 0) != 0)
            {
                levels[i-1].enableButton.enabled = true;
                levels[i-1].levelX.SetActive(false);
            }
        }
    }

    IEnumerator CreateSphere(Modes modes)
    {
        System.Random random = new System.Random();
        while (true)
        {
            int rndRight = random.Next(0, 3);
            int rndLeft = random.Next(0, 3);
            switch (modes)
            {
                case Modes.Easy:
                    {
                        spheres.Add(Instantiate(sphere[random.Next(0, sphere.Count)], positionRight[rndRight].transform.position, Quaternion.Euler(90, 0, 0)));
                        StartCoroutine(MoveSphereRight(spheres[spheres.Count - 1]));
                        break;
                    }
                case Modes.Normal:
                    {
                        spheres.Add(Instantiate(sphere[random.Next(0, sphere.Count)], positionRight[rndRight].transform.position, Quaternion.Euler(90, 0, 0)));
                        StartCoroutine(MoveSphereRight(spheres[spheres.Count - 1]));
                        spheres.Add(Instantiate(sphere[random.Next(0, sphere.Count)], positionLeft[rndLeft].transform.position, Quaternion.Euler(90, 0, 0)));
                        StartCoroutine(MoveSphereLeft(spheres[spheres.Count - 1]));
                        break;
                    }
                case Modes.Hard:
                    {
                        spheres.Add(Instantiate(sphere[random.Next(0, sphere.Count)], positionRight[rndRight].transform.position, Quaternion.Euler(90, 0, 0)));
                        StartCoroutine(MoveSphereRight(spheres[spheres.Count - 1]));
                        spheres.Add(Instantiate(sphere[random.Next(0, sphere.Count)], positionLeft[rndLeft].transform.position, Quaternion.Euler(90, 0, 0)));
                        StartCoroutine(MoveSphereLeft(spheres[spheres.Count - 1]));
                        spheres.Add(Instantiate(sphere[random.Next(0, sphere.Count)], positionUpRight.transform.position, Quaternion.Euler(90, 0, 0)));
                        StartCoroutine(MoveSphereUpRight(spheres[spheres.Count - 1]));
                        break;
                    }
                case Modes.VeryHard:
                    {
                        spheres.Add(Instantiate(sphere[random.Next(0, sphere.Count)], positionRight[rndRight].transform.position, Quaternion.Euler(90, 0, 0)));
                        StartCoroutine(MoveSphereRight(spheres[spheres.Count - 1]));
                        spheres.Add(Instantiate(sphere[random.Next(0, sphere.Count)], positionLeft[rndLeft].transform.position, Quaternion.Euler(90, 0, 0)));
                        StartCoroutine(MoveSphereLeft(spheres[spheres.Count - 1]));
                        //information; Very Hard modunda üstten bir adet aþþaðýdan bir adet gelmesini istiyorum, bu yüzden bir adet 0 veya 1 int deðer alýyorum
                        //bu deðere göre ürünü oluþturup daha sonra StartCoroutine ile çalýþtýrýyorum.
                        //burada gönderdiðim bool array Corutine içinde sphere e yeni pozisyon atarken x ve y sine posizyonlarýna göre belirli deðerleri atamam gerekiyor.
                        //aldýðý bool deðerlere göre (-,-)(+,+)(+,-)(-,+) deðerlerini alabiliyor. (true==+)(false==-)
                        //bu iþlemi de aþþaðýdaki OperationControl fonksiyonunda yapýyor.
                        //up--
                        int x = random.Next(0, 2);
                        spheres.Add(Instantiate(sphere[random.Next(0, sphere.Count)], x == 0 ? positionUpRight.transform.position : positionUpLeft.transform.position, Quaternion.Euler(90, 0, 0)));
                        StartCoroutine(MoveVeryHardMode(spheres[spheres.Count - 1], x == 0 ? new bool[] { false, false } : new bool[] { true, false }));
                        //down--
                        x = random.Next(0, 2);
                        spheres.Add(Instantiate(sphere[random.Next(0, sphere.Count)], x == 0 ? positionDownRight.transform.position : positionDownLeft.transform.position, Quaternion.Euler(90, 0, 0)));
                        StartCoroutine(MoveVeryHardMode(spheres[spheres.Count - 1], x == 0 ? new bool[] { false, true } : new bool[] { true, true }));

                        break;
                    }
            }
            yield return new WaitForSeconds(1f);
        }
    }
    IEnumerator MoveSphereRight(GameObject _sphere)
    {
        while (true)
        {
            _sphere.transform.position = new Vector3(
                _sphere.transform.position.x - enemySpeed * Time.deltaTime,
                _sphere.transform.position.y,
                _sphere.transform.position.z);
            yield return null;
        }
    }
    IEnumerator MoveSphereLeft(GameObject _sphere)
    {
        while (true)
        {
            _sphere.transform.position = new Vector3(
                _sphere.transform.position.x + enemySpeed * Time.deltaTime,
                _sphere.transform.position.y,
                _sphere.transform.position.z);
            yield return null;
        }
    }
    IEnumerator MoveSphereUpRight(GameObject _sphere)
    {
        while (true)
        {
            _sphere.transform.position = new Vector3(
                _sphere.transform.position.x - (enemySpeed / 2) * Time.deltaTime,
                _sphere.transform.position.y - (enemySpeed / 2) * Time.deltaTime,
                _sphere.transform.position.z);
            yield return null;
        }
    }
    IEnumerator MoveVeryHardMode(GameObject _sphere, bool[] operation)
    {
        while (true)
        {
            _sphere.transform.position = new Vector3(
                OperationControl(_sphere.transform.position.x, (enemySpeed / 2) * Time.deltaTime, operation[0]),
                OperationControl(_sphere.transform.position.y, (enemySpeed / 2) * Time.deltaTime, operation[1]),
                _sphere.transform.position.z);
            yield return null;
        }
    }
    #endregion
    #region UI buttons
    public void StartGame(int _id)
    {
        AdController.current.bannerView.Hide();
        DeleteSpheres();
        StopAllCoroutines();
        RenderSettings.skybox = levels[_id - 1].skyboxMaterial;
        DynamicGI.UpdateEnvironment();

        nextEnableButton = levels[_id - 1].enableButton;

        UI.startPanel.SetActive(false);
        UI.stagesButton.SetActive(false);

        activeLevel = _id;
        freeModeFlag = false;
        greenBallCount = levels[_id - 1].greenBallCount;
        bonusBallCount = levels[_id - 1].bonusBallCount;
        nextLevelX = levels[_id - 1].levelX;
        Main.speed = levels[_id - 1].characterSpeed;
        enemySpeed = levels[_id - 1].enemySpeed;
        gameTime = levels[_id - 1].gameTime;

        if(gameTime == 0 && bonusBallCount == 0)
        {
            UI.collectGreen.SetActive(true);
            UI.collectMissionGreenMaxScore.text = greenBallCount.ToString();
        }
        else if(gameTime == 0 && greenBallCount == 0)
        {
            UI.collectGreen.SetActive(true);
            UI.collectMissionBonusBallMaxScore.text = bonusBallCount.ToString();
        }
        else if (gameTime == 0)
        {
            UI.collectGreen.SetActive(true);
            UI.collectMissionGreenMaxScore.text = greenBallCount.ToString();
            UI.collectMissionBonusBallMaxScore.text = bonusBallCount.ToString();
        }
        create = StartCoroutine(CreateSphere(levels[_id-1].mode));
    }
    public void Stages()
    {
        UI.startButtons.SetActive(false);
        UI.stagesButton.SetActive(true);
    }
    public void StagesBack()
    {
        UI.startButtons.SetActive(true);
        UI.stagesButton.SetActive(false);
    }
    public void FreeMode()
    {
        UI.startButtons.SetActive(false);
        UI.freeModeButtons.SetActive(true);
        UI.freeModMaxScoreButton.SetActive(true);
    }
    public void FreeModeBack()
    {
        UI.startButtons.SetActive(true);
        UI.freeModeButtons.SetActive(false);
        UI.freeModMaxScoreButton.SetActive(false);
    }
    public void FreeModeEasy()
    {
        AdController.current.bannerView.Hide();
        DeleteSpheres();
        StopAllCoroutines();
        UIStatic.freeModeMaxScore.text = "0";
        enemySpeed = 12;
        Main.speed = 200;
        freeModeFlag = true;
        UI.startPanel.SetActive(false);
        create = StartCoroutine(CreateSphere(Modes.Easy));
    }
    public void FreeModeNormal()
    {
        AdController.current.bannerView.Hide();
        DeleteSpheres();
        StopAllCoroutines();
        UIStatic.freeModeMaxScore.text = "0";
        enemySpeed = 12;
        Main.speed = 200;
        freeModeFlag = true;
        UI.startPanel.SetActive(false);
        create = StartCoroutine(CreateSphere(Modes.Normal));
    }
    public void FreeModeHard()
    {
        AdController.current.bannerView.Hide();
        DeleteSpheres();
        StopAllCoroutines();
        UIStatic.freeModeMaxScore.text = "0";
        enemySpeed = 12;
        Main.speed = 200;
        freeModeFlag = true;
        UI.startPanel.SetActive(false);
        create = StartCoroutine(CreateSphere(Modes.Hard));
    }
    public void FreeModeVeryHard()
    {
        AdController.current.bannerView.Hide();
        DeleteSpheres();
        StopAllCoroutines();
        UIStatic.freeModeMaxScore.text = "0";
        enemySpeed = 12;
        Main.speed = 250;
        freeModeFlag = true;
        UI.startPanel.SetActive(false);
        create = StartCoroutine(CreateSphere(Modes.VeryHard));
    }
    //public void ScreenOne()
    //{
    //    RenderSettings.skybox = skyboxes[0];
    //    DynamicGI.UpdateEnvironment();
    //}
    //public void ScreenTwo()
    //{
    //    RenderSettings.skybox = skyboxes[1];
    //    DynamicGI.UpdateEnvironment();
    //}
    //public void ScreenThree()
    //{
    //    RenderSettings.skybox = skyboxes[2];
    //    DynamicGI.UpdateEnvironment();
    //}
    //public void ScreenFour()
    //{
    //    RenderSettings.skybox = skyboxes[3];
    //    DynamicGI.UpdateEnvironment();
    //}
    //public void ScreenFive()
    //{
    //    RenderSettings.skybox = skyboxes[4];
    //    DynamicGI.UpdateEnvironment();
    //}
    #endregion

    private void DeleteSpheres()
    {
        foreach (GameObject item in spheres)
        {
            Destroy(item);
        }
    }

    private float OperationControl(float valueFirst, float valueSecond, bool type)
    {
        return type ? valueFirst + valueSecond : valueFirst - valueSecond;
    }
}

