using TMPro;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class HealthBarController : MonoBehaviour
{
    RectTransform rectTransform;
    [SerializeField] TMP_Text lifeText;

    public int MaxLife { get; set; }

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (lifeText == null)
        {
            Debug.LogError("Life text not assign");
        }
    }

    public void UpdateLife(int life)
    {
        rectTransform.localScale = new Vector3((float)life / 100f, rectTransform.localScale.y, rectTransform.localScale.y);
        lifeText.text = life.ToString() + " / " + MaxLife.ToString();
    }
}
