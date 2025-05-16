using System.Threading;

namespace AvaloniaApp.Models;

public interface ILoader
{
    int Id { get; }
    string Status { get; }
    int? EvacuatedCarId { get; }
    bool HasWorked { get; }
    void Load(RacingCar car);
}

public class Loader : ILoader
{
    public int Id { get; }
    public string Status { get; private set; }
    public int? EvacuatedCarId { get; private set; }
    public bool HasWorked { get; private set; }

    public Loader(int id)
    {
        Id = id;
        Status = "Ожидает";
        EvacuatedCarId = null;
        HasWorked = false;
    }

    public void Load(RacingCar car)
    {
        if (HasWorked) return;

        Status = "Эвакуирует";
        EvacuatedCarId = car.Id;
        HasWorked = true;

        // Имитация времени эвакуации
        Thread.Sleep(5000);

        car.IsCrashed = false;
        car.Condition = "Эвакуирован";
        car.Status = "Снят с трассы";
        car.Stop();

        Status = "Закончил работу";
    }

}