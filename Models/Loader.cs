namespace AvaloniaApp.Models;

public interface ILoader
{
    void Load(RacingCar car);
}

public class Loader : ILoader
{
    public int Id { get; set; }
    public string Status { get; set; }
    public int? EvacuatedCarId { get; set; }
    public bool HasWorked { get; set; }

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
        System.Threading.Thread.Sleep(5000);
        
        car.IsCrashed = false;
        car.Condition = "Эвакуирован";
        car.Status = "Снят с трассы";
    }
}