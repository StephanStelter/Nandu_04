using UnityEngine;

public class TimerForDelay
{
    private float timer;
    private float timerDuration;
    private bool isTimerRunning = false;

    public TimerForDelay(float durationInSeconds)
    {
        timerDuration = durationInSeconds;
    }

    public void StartTimer()
    {
        if (!isTimerRunning)
        {
            timer = 0f;
            isTimerRunning = true;
        }
        else
        {
            Debug.Log("Der Timer läuft bereits.");
        }
    }

    public void UpdateTimer(float deltaTime)
    {
        if (isTimerRunning)
        {
            timer += deltaTime;
            if (timer >= timerDuration)
            {
                isTimerRunning = false;
            }
        }
    }

    public bool TimerStillRunning()
    {
        return isTimerRunning;
    }
}
