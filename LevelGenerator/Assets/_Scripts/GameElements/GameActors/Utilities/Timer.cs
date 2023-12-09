using System;
using System.Collections;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] float timerDuration;
    Coroutine timerCoroutine;

    public event Action OnTimerExpired;

    public void StartTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
        timerCoroutine = StartCoroutine(TimerCoroutine());
    }

    IEnumerator TimerCoroutine()
    {
        yield return new WaitForSeconds(timerDuration);
        TimerExpired();
    }

    void TimerExpired()
    {
        OnTimerExpired?.Invoke();
    }
}
