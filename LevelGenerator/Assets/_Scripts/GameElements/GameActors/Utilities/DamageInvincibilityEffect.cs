using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageInvincibilityEffect : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] float delayBetweenEffects;

    Coroutine effectCoroutine;

    public void StartEffect(float effectDuraction)
    {
        if (effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine);
        }
        effectCoroutine = StartCoroutine(StartEffectCoroutine(effectDuraction));
    }

    IEnumerator StartEffectCoroutine(float effectDuraction)
    {
        int effectCount = Mathf.CeilToInt(effectDuraction / delayBetweenEffects);

        for (int i = 0; i < effectCount; i++)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(delayBetweenEffects / 2);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(delayBetweenEffects / 2);
        }

        spriteRenderer.color = Color.blue;
    }
}
