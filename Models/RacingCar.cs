using System;
using System.Threading;

namespace AvaloniaApp.Models;

public class RacingCar
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Distance { get; set; }
    public string Status { get; set; }
    public string Condition { get; set; }
    public int Speed { get; set; }
    public bool IsTireWornOut { get; set; }
    public bool IsCrashed { get; set; }
    public bool IsInPitStop { get; set; }

    public event Action<RacingCar>? OnTireWornOut;
    public event Action<RacingCar>? OnCrash;

    private readonly Random _random = new();
    private bool _isRunning = true;

    public RacingCar(int id, string name, int speed)
    {
        Id = id;
        Name = name;
        Speed = speed;
        Distance = 0;
        Status = "Едет по трассе";
        Condition = "Нормальное";
        IsTireWornOut = false;
        IsCrashed = false;
        IsInPitStop = false;
    }

    public void Drive()
    {
        while (_isRunning)
        {
            Thread.Sleep(1000);
            
            if (IsInPitStop || IsCrashed) continue;
            
            Distance += Speed;
            
            // Проверка на износ покрышек (5% вероятность)
            if (_random.Next(1, 101) <= 5 && !IsTireWornOut)
            {
                IsTireWornOut = true;
                Condition = "Стерлись покрышки";
                Status = "Заехал в бокс";
                IsInPitStop = true;
                OnTireWornOut?.Invoke(this);
            }
            
            // Проверка на аварию (2% вероятность)
            if (_random.Next(1, 101) <= 2)
            {
                IsCrashed = true;
                Condition = "Авария";
                Status = "Ожидает эвакуации";
                OnCrash?.Invoke(this);
            }
        }
    }

    public void Stop()
    {
        _isRunning = false;
    }
}