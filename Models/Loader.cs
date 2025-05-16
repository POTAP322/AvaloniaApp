namespace AvaloniaApp.Models;
public interface ILoader
{
    void Load(RacingCar car);
}

public class Loader : ILoader
{
    public void Load(RacingCar car)
    {
        car.IsCrashed = false;
    }
}
