using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public enum Modes { Easy, Normal, Hard };

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
        public int maxBallCount;
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
        public GameObject collectMission;
        public Text collectMissionMaxScore;
    }

    public List<Levels> levels;
    public List<GameObject> positionRight = new List<GameObject>();
    public List<GameObject> positionLeft = new List<GameObject>();
    public List<GameObject> positionUp = new List<GameObject>();
    public List<GameObject> positionDown = new List<GameObject>();
    public List<Material> skyboxes = new List<Material>();
    public List<GameObject> sphere;

    public UIGameObjects UI;
    public static UIGameObjects UIStatic;

    public static GameObject nextLevelX;
    public static Button nextEnableButton;
    public static List<GameObject> spheres = new List<GameObject> ();
    public static float enemySpeed = 12f;
    public static float gameTime;
    public static int maxBallCount = 0;
    public static bool endFlag = false;
    public static bool freeModeFlag = false;
    public static int activeLevel = 0;

    #region Sphere Create
    private void Awake()
    {
        //PlayerPrefs.DeleteAll();
        System.Random random = new System.Random();
        RenderSettings.skybox = skyboxes[random.Next(0, skyboxes.Count)];
        DynamicGI.UpdateEnvironment();
        UIStatic = UI;
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
            int sphereIndex = random.Next(0, 2);
            switch (modes)
            {
                case Modes.Easy:
                    {
                        spheres.Add(Instantiate(sphere[sphereIndex], positionRight[rndRight].transform.position, Quaternion.Euler(90, 0, 0)));
                        StartCoroutine(MoveSphereRight(spheres[spheres.Count-1]));
                        break;
                    }
                case Modes.Normal:
                    {
                        spheres.Add(Instantiate(sphere[random.Next(0, 2)], positionRight[rndRight].transform.position, Quaternion.Euler(90, 0, 0)));
                        StartCoroutine(MoveSphereRight(spheres[spheres.Count - 1]));
                        spheres.Add(Instantiate(sphere[random.Next(0, 2)], positionLeft[rndLeft].transform.position, Quaternion.Euler(90, 0, 0)));
                        StartCoroutine(MoveSphereLeft(spheres[spheres.Count - 1]));
                        break;
                    }
                case Modes.Hard:
                    {
                        spheres.Add(Instantiate(sphere[random.Next(0, 2)], positionRight[rndRight].transform.position, Quaternion.Euler(90, 0, 0)));
                        StartCoroutine(MoveSphereRight(spheres[spheres.Count - 1]));
                        spheres.Add(Instantiate(sphere[random.Next(0, 2)], positionLeft[rndLeft].transform.position, Quaternion.Euler(90, 0, 0)));
                        StartCoroutine(MoveSphereLeft(spheres[spheres.Count - 1]));
                        spheres.Add(Instantiate(sphere[random.Next(0, 2)], positionUp[rndRight].transform.position, Quaternion.Euler(90, 0, 0)));
                        StartCoroutine(MoveSphereUp(spheres[spheres.Count - 1]));
                        //StartCoroutine(MoveSphereDown(Instantiate(sphere[random.Next(0, 2)], positionDown[rndLeft].transform.position, Quaternion.Euler(90, 0, 0))));
                        break;
                    }
            }
            yield return new WaitForSeconds(1f);
        }
    }
    IEnumerator MoveSphereRight(GameObject _sphere/*, float speedX, float speedY, float speedZ*/)
    {
        while (true)
        {
            _sphere.transform.position = new Vector3(
                _sphere.transform.position.x - enemySpeed * Time.deltaTime,
                _sphere.transform.position.y, /*+ speedY * Time.deltaTime,*/
                _sphere.transform.position.z /*+ speedZ * Time.deltaTime*/);
            yield return null;
        }
    }
    IEnumerator MoveSphereLeft(GameObject _sphere/*, float speedX, float speedY, float speedZ*/)
    {
        while (true)
        {
            _sphere.transform.position = new Vector3(
                _sphere.transform.position.x + enemySpeed * Time.deltaTime,
                _sphere.transform.position.y, /*+ speedY * Time.deltaTime,*/
                _sphere.transform.position.z /*+ speedZ * Time.deltaTime*/);
            yield return null;
        }
    }
    IEnumerator MoveSphereUp(GameObject _sphere/*, float speedX, float speedY, float speedZ*/)
    {
        while (true)
        {
            _sphere.transform.position = new Vector3(
                _sphere.transform.position.x - (enemySpeed / 2) * Time.deltaTime,
                _sphere.transform.position.y - (enemySpeed / 2) * Time.deltaTime, /*+ speedY * Time.deltaTime,*/
                _sphere.transform.position.z /*+ speedZ * Time.deltaTime*/);
            yield return null;
        }
    }
    IEnumerator MoveSphereDown(GameObject _sphere/*, float speedX, float speedY, float speedZ*/)
    {
        while (true)
        {
            _sphere.transform.position = new Vector3(
                _sphere.transform.position.x + (enemySpeed / 2) * Time.deltaTime,
                _sphere.transform.position.y + (enemySpeed / 2) * Time.deltaTime, /*+ speedY * Time.deltaTime,*/
                _sphere.transform.position.z /*+ speedZ * Time.deltaTime*/);
            yield return null;
        }
    }
    #endregion
    #region UI buttons
    public void StartGame(int _id)
    {
        DeleteSpheres();
        StopAllCoroutines();

        nextEnableButton = levels[_id - 1].enableButton;

        UI.startPanel.SetActive(false);
        UI.stagesButton.SetActive(false);

        activeLevel = _id;
        freeModeFlag = false;
        maxBallCount = levels[_id-1].maxBallCount;
        nextLevelX = levels[_id-1].levelX;
        Main.speed = levels[_id - 1].characterSpeed;
        enemySpeed = levels[_id - 1].enemySpeed;
        gameTime = levels[_id - 1].gameTime;

        if(gameTime == 0)
        {
            UI.collectMission.SetActive(true);
            UI.collectMissionMaxScore.text = maxBallCount.ToString();
        } 
        StartCoroutine(CreateSphere(levels[_id-1].mode));
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
    }
    public void FreeModeBack()
    {
        UI.startButtons.SetActive(true);
        UI.freeModeButtons.SetActive(false);
    }
    public void FreeModeEasy()
    {
        DeleteSpheres();
        StopAllCoroutines();
        enemySpeed = 12;
        Main.speed = 200;
        freeModeFlag = true;
        UI.startPanel.SetActive(false);
        StartCoroutine(CreateSphere(Modes.Easy));
    }
    public void FreeModeNormal()
    {
        DeleteSpheres();
        StopAllCoroutines();
        enemySpeed = 12;
        Main.speed = 200;
        freeModeFlag = true;
        UI.startPanel.SetActive(false);
        StartCoroutine(CreateSphere(Modes.Normal));
    }
    public void FreeModeHard()
    {
        DeleteSpheres();
        StopAllCoroutines();
        enemySpeed = 12;
        Main.speed = 200;
        freeModeFlag = true;
        UI.startPanel.SetActive(false);
        StartCoroutine(CreateSphere(Modes.Hard));
    }
    public void ScreenOne()
    {
        RenderSettings.skybox = skyboxes[0];
        DynamicGI.UpdateEnvironment();
    }
    public void ScreenTwo()
    {
        RenderSettings.skybox = skyboxes[1];
        DynamicGI.UpdateEnvironment();
    }
    public void ScreenThree()
    {
        RenderSettings.skybox = skyboxes[2];
        DynamicGI.UpdateEnvironment();
    }
    public void ScreenFour()
    {
        RenderSettings.skybox = skyboxes[3];
        DynamicGI.UpdateEnvironment();
    }
    public void ScreenFive()
    {
        RenderSettings.skybox = skyboxes[4];
        DynamicGI.UpdateEnvironment();
    }
    #endregion

    private void DeleteSpheres()
    {
        foreach (GameObject item in spheres)
        {
            Destroy(item);
        }
    }
}

