using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneNamesConstants
{
    public static string MENU = "Menu";
    public static string GAME = "Game";
}

public class SceneChangeManager : SingletonMonoBehaviour<SceneChangeManager>
{
    public bool LoadingScene { get; set; }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LoadScene(string sceneName)
    {
        LoadingScene = true;
        SceneManager.LoadScene(sceneName);
    }

    public void LoadSceneAsync(string sceneName)
    {
        LoadingScene = true;
        SceneManager.LoadSceneAsync(sceneName);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LoadingScene = false;
    }
}
