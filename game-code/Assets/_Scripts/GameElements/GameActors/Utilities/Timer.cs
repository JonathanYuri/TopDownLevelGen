using System;
using System.Collections;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] float timerDuration;
    Coroutine timerCoroutine;

    public float TimerDuration { get => timerDuration; set => timerDuration = value; }

    public event Action OnTimerExpired;

    public void StartTimer()
    {
        StopTimer();
        timerCoroutine = StartCoroutine(TimerCoroutine());
    }

    public void StopTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
    }

    IEnumerator TimerCoroutine()
    {
        yield return new WaitForSeconds(TimerDuration);
        TimerExpired();
    }

    void TimerExpired()
    {
        OnTimerExpired?.Invoke();
    }
}
