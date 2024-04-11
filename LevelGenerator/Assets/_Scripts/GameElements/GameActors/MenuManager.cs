using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] TMP_InputField username;
    [SerializeField] GameObject errorMessage;

    LoadingManager loadingManager;

    void Start()
    {
        loadingManager = FindObjectOfType<LoadingManager>(true);
    }

    public void OnUsernameChanged()
    {
        errorMessage.SetActive(false);
    }

    public void OnStartGameClick()
    {
        if (username.text.Length == 0)
        {
            errorMessage.SetActive(true);
            return;
        }

        FindObjectOfType<PlayerInfo>().Username = username.text;

        loadingManager.StartLoading();
        SceneChangeManager.Instance.LoadSceneAsync(SceneNamesConstants.GAME);
    }
}
