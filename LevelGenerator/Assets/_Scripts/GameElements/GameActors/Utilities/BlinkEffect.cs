using System.Collections;
using UnityEngine;

public class BlinkEffect : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] int effectCount;

    [SerializeField] Color[] colors;
    [SerializeField] bool lerp;

    Coroutine effectCoroutine;

    Color initialColor;

    void Awake()
    {
        initialColor = spriteRenderer.color;
    }

    public void StartEffect(float effectDuraction)
    {
        if (effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine);
        }
        effectCoroutine = StartCoroutine(PlayColorEffectCoroutine(effectDuraction));
    }

    IEnumerator PlayColorEffectCoroutine(float effectDuraction)
    {
        float delayBetweenEffects = effectDuraction / effectCount;

        for (int i = 0; i < effectCount; i++)
        {
            if (lerp)
            {
                yield return TransitionColorCoroutine(delayBetweenEffects);
            }
            else
            {
                yield return FlashColorsCoroutine(delayBetweenEffects);
            }
        }

        spriteRenderer.color = initialColor;
    }

    IEnumerator TransitionColorCoroutine(float delayBetweenEffects)
    {
        int colorsCount = colors.Length;

        for (int i = 0; i < colorsCount; i++)
        {
            Color firstColor = colors[i];
            Color secondColor = i + 1 < colorsCount ? colors[i + 1] : colors[0];

            float elapsedTime = 0f;
            while (elapsedTime < delayBetweenEffects / colorsCount)
            {
                spriteRenderer.color = Color.Lerp(firstColor, secondColor, elapsedTime / (delayBetweenEffects / colorsCount));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }

    IEnumerator FlashColorsCoroutine(float delayBetweenEffects)
    {
        int colorsCount = colors.Length;
        for (int i = 0; i < colorsCount; i++)
        {
            spriteRenderer.color = colors[i];
            yield return new WaitForSeconds(delayBetweenEffects / colorsCount);
        }
    }
}
