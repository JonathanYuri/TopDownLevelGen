using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] float timeToModifyLoadingText;

    public void StartLoading()
    {
        gameObject.SetActive(true);
        //StartCoroutine(ModifyLoadingTextCoroutine());
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
        StopAllCoroutines();
        text.text = "Carregando";
        gameObject.SetActive(false);
    }
}
