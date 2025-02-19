using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomTimerManager : Singleton<CustomTimerManager> 
{
    double _TimeSec = 0;
    CustomTimer _TimerTemp;
    List<CustomTimer> _Timers = new List<CustomTimer>();
    List<CustomTimer> _TimerPool = new List<CustomTimer>();
    List<CustomTimer> _RemovableTimers = new List<CustomTimer>();

    private void Update()
    {
        _TimeSec = Time.unscaledDeltaTime;
        for (int i = _Timers.Count - 1; i > -1; --i)
        {
            _TimerTemp = _Timers[i];
            _TimerTemp?.Update(_TimeSec);
        }

        foreach (var remove in _RemovableTimers)
        {
            _Timers.Remove(remove);
            _TimerPool.Add(remove);
        }
        _RemovableTimers.Clear();
    }


    public void Clear()
    {
        for (int i = _Timers.Count - 1; i >= 0; --i)
        {
            RemoveTimer(_Timers[i]);
        }
        _Timers.Clear();
    }


    public CustomTimer CreateCountDown(double restSec, Action<double> onChangeCallback)
    {
        CustomTimer timer;// = GenericPool<CustomTimer>.Get();

        if (_TimerPool.Count > 0)
        {
            timer = _TimerPool[0];
            _TimerPool.RemoveAt(0);
        }
        else
        {
            timer = new CustomTimer();
        }

        timer.Init(CustomTimer.TYPE.Countdown, restSec, onChangeCallback);
        _Timers.Add(timer);

        return timer;
    }

    public CustomTimer CreateTimer(Action<double> onChangeCallback)
    {
        CustomTimer timer;

        if (_TimerPool.Count > 0)
        {
            timer = _TimerPool[0];
            _TimerPool.RemoveAt(0);
        }
        else
        {
            timer = new CustomTimer();
        }

        timer.Init(CustomTimer.TYPE.Timer, 0, onChangeCallback);
        _Timers.Add(timer);

        return timer;
    }

    public void RemoveTimer(CustomTimer timer)
    {
        if (timer == null)
            return;

        timer.Reset();
        _RemovableTimers.Add(timer);
        //_Timers.Remove(timer);
        //_TimerPool.Add(timer);
    }
}
