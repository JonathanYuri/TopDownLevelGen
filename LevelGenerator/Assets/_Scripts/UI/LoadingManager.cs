using System.Collections;
using TMPro;
using UnityEngine;

public class LoadingManager : MonoBehaviour
{
    InputManager inputManager;

    [SerializeField] TMP_Text text;
    [SerializeField] float timeToModifyLoadingText;

    public void StartLoading()
    {
        if (inputManager == null)
        {
            inputManager = FindObjectOfType<InputManager>();
        }

        if (inputManager != null)
        {
            inputManager.DisableInput();
        }
        gameObject.SetActive(true);
        StartCoroutine(ModifyLoadingTextCoroutine());
    }

    IEnumerator ModifyLoadingTextCoroutine()
    {
        while (true)
        {
            if (text.text == "Carregando")
            {
                text.text = "Carregando.";
            }
            else if (text.text == "Carregando.")
            {
                text.text = "Carregando..";
            }
            else if (text.text == "Carregando..")
            {
                text.text = "Carregando...";
            }
            else if (text.text == "Carregando...")
            {
                text.text = "Carregando";
            }

            yield return new WaitForSeconds(timeToModifyLoadingText);
        }
    }

    public void StopLoading()
    {
        inputManager.EnableInput();
        StopAllCoroutines();
        text.text = "Carregando...";
        gameObject.SetActive(false);
    }
}
