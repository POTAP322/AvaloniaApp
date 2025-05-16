namespace AvaloniaApp.Models;
using System;
using System.Threading;

public class RacingCar
{
    public string Name { get; set; }
    public int Speed { get; set; }
    public bool IsTireWornOut { get; set; }
    public bool IsCrashed { get; set; }

    public event Action<string>? OnTireWornOut;
    public event Action<string>? OnCrash;

    public RacingCar(string name, int speed)
    {
        Name = name;
        Speed = speed;
    }

    public void Drive()
    {
        Random random = new Random();
        while (true)
        {
            Thread.Sleep(1000);
            if (random.Next(1, 100) < 5)
            {
                IsTireWornOut = true;
                OnTireWornOut?.Invoke(Name);
            }
            if (random.Next(1, 100) < 2)
            {
                IsCrashed = true;
                OnCrash?.Invoke(Name);
            }
        }
    }
}
