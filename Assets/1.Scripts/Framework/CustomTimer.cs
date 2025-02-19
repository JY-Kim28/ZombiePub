using System;

public class CustomTimer
{
    public enum TYPE
    {
        Countdown,
        Timer
    }
    TYPE _Type;


    double _OriginRestSec;
    double _RestSec;
    public double RestSec { get => _RestSec; }
    public Action<double> OnChangeTimeCallback;

    Action<double> _OnUpdate;

    public void Reset()
    {
        _OnUpdate = null;
        OnChangeTimeCallback = null;
    }

    public void Init(TYPE type, double restSec, Action<double> onChangeCallback)
    {
        Reset();

        _Type = type;

        _OriginRestSec = restSec;
        _RestSec = restSec;

        OnChangeTimeCallback += onChangeCallback;

        if (_Type == TYPE.Countdown)
            _OnUpdate = Countdown;
        else
            _OnUpdate = Timer;
    }

    public void SetSec(double sec)
    {
        _RestSec = sec;
    }


    public void Update(double updateSec)
    {
        _OnUpdate?.Invoke(updateSec);
    }

    protected void Timer(double sec)
    {
        OnChangeTimeCallback?.Invoke(sec);
    }

    protected void Countdown(double sec)
    {
        double prev = _RestSec;

        _RestSec -= sec;
        if (_RestSec < 0)
            _RestSec = 0;

        if (prev != _RestSec)
        {
            OnChangeTimeCallback?.Invoke(_RestSec);
        }
    }
}