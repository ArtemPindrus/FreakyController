using System;
using System.Collections.Generic;
using FlaxEngine;

namespace FreakyController;

/// <summary>
/// SinTween class.
/// </summary>
public struct SinTween
{
    public event Action? Finished;

    public bool Reversed;

    public float Time { get; private set; }
    public float Duration { get; private set; }
    public float Start { get; private set; }
    public float End { get; private set; }
    public float Displacement { get; private set; }

    public SinTween(float start, float end, float duration) : this()
    {
        SetEndPoints(start, end);
        Duration = duration;
    }

    public float Update(float deltaTime)
    {
        Time += Reversed ? -deltaTime : deltaTime;

        if (Time >= Duration)
        {
            Finished?.Invoke();
            Time = Duration;
            return End;
        }
        else if (Time < 0)
        {
            Finished?.Invoke();
            Time = 0;
            return Start;
        }

        float res = Displacement * Mathf.Sin(Mathf.Pi / (2 * Duration) * Time) + Start;
        return res;
    }

    public void Reverse() => Reversed = !Reversed;

    public void SetEndPoints(float start, float end)
    {
        Start = start;
        End = end;
        Displacement = end - start;
    }
}
