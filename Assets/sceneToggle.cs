using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class sceneToggle : MonoBehaviour
{
    [SerializeField] private bool captureAllScreenshots;
    private GameObject buttonTemplate;
    private bool showingStats;
    private int currentSceneIndex;
    private String currentSceneName;

    private static bool started;
    // Start is called before the first frame update
    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        SceneManager.LoadScene(1);
        currentSceneName = SceneManager.GetActiveScene().name;
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        /*
        buttonTemplate = transform.GetChild(0).gameObject;
        GameObject go;
        Transform layout = transform.GetChild(1);
        for (int s = 0;s<SceneManager.sceneCountInBuildSettings;s++)
        {
            go = Instantiate(buttonTemplate);
            go.transform.SetParent(layout);
            go.transform.GetChild(0).GetComponent<Text>().text = "" + (s + 1);
            int index = s;
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(() => SceneLoad(index));
        }
        
        
        buttonTemplate.SetActive(false); */
        DontDestroyOnLoad(gameObject); 
        if (captureAllScreenshots && !started)
        {
            started = true;
            StartCoroutine(CaptureScreenshots());
        }
    }
#if UNITY_EDITOR
    [InitializeOnLoad]
    class SaveProjectPath
    {
        static SaveProjectPath()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                print("LOADED");
                PlayerPrefs.SetString("projectPath", Application.dataPath);
            }
            
        }
    }
    #endif

    private IEnumerator CaptureScreenshots()
    {
        int totalSceneCount = SceneManager.sceneCountInBuildSettings;
        print("capturing  "+totalSceneCount);
        for (int scene = 0;scene<totalSceneCount;scene++)
        {
            SceneManager.LoadScene(scene);
            yield return null;
            print("loading scene  "+scene);
            ScreenCapture.CaptureScreenshot("Screenshot "+scene+": "+SceneManager.GetActiveScene().name);
            yield return new WaitForSeconds(2f);
        }
    }

    private void SceneLoad(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    private void OnGUI()
    {
        if (currentSceneIndex == 1){
            GUILayout.Label("Current gfx API: "+SystemInfo.graphicsDeviceType);
            GUILayout.Label("Deice GPU: "+SystemInfo.graphicsDeviceName);
            GUILayout.Label("scene "+currentSceneIndex+": "+currentSceneName);
        }
    }
    
    void Update()
    {
        if (Input.touchCount == 2)
        {
            currentSceneName = SceneManager.GetActiveScene().name;
            currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            Touch touch2 = Input.GetTouch(1);
            int sceneToLoadIndex;
            if(touch2.phase == TouchPhase.Began){
                if (touch2.position.x < Screen.width / 2f)
                {
                    if (SceneManager.GetActiveScene().buildIndex == 1) {
                        SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
                    } else {
                        SceneManager.LoadScene(currentSceneIndex - 1);
                    }
                } else {
                    if (SceneManager.GetActiveScene().buildIndex >= SceneManager.sceneCountInBuildSettings-1 ){
                        SceneManager.LoadScene(1);
                    } else {
                        SceneManager.LoadScene(currentSceneIndex + 1);
                    }
                }
            }
        }
    }

    private IEnumerator ShowingStats()
    {
        showingStats = true;
        yield return new WaitForSeconds(1);
        showingStats = false;
    }
    
}
