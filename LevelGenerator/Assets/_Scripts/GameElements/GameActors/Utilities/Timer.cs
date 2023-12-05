using System;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] float timerDuration;

    public event Action OnTimerExpired;

    public void StartTimer()
    {
        Invoke(nameof(TimerExpired), timerDuration);
    }

    void TimerExpired()
    {
        OnTimerExpired?.Invoke();
    }
}
