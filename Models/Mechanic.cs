using System.Threading;

namespace AvaloniaApp.Models;

public class Mechanic
{
    public void ChangeTires(RacingCar car)
    {
        // Имитация времени на замену покрышек
        Thread.Sleep(3000);
        car.IsTireWornOut = false;
        car.Condition = "Нормальное";
        car.Status = "Едет по трассе";
        car.IsInPitStop = false;
    }
}