using System;
using System.Collections.ObjectModel;
using System.Threading;
using AvaloniaApp.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApp.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<RacingCar> _racingCars;

    [ObservableProperty]
    private ObservableCollection<Loader> _loaders;

    private Mechanic _mechanic = new Mechanic();

    public MainWindowViewModel()
    {
        RacingCars = new ObservableCollection<RacingCar>();
        Loaders = new ObservableCollection<Loader>();

        // Добавим 3 погрузчика
        for (int i = 0; i < 3; i++)
        {
            Loaders.Add(new Loader(i + 1));
        }

        // Добавим 2 машины при запуске
        for (int i = 0; i < 2; i++)
        {
            AddNewCar();
        }
    }

    [RelayCommand]
    private void AddNewCar()
    {
        var racingCar = new RacingCar(RacingCars.Count + 1, $"Car{RacingCars.Count + 1}", new Random().Next(80, 120));
        racingCar.OnTireWornOut += (car) => _mechanic.ChangeTires(car);
        racingCar.OnCrash += HandleCrash;

        RacingCars.Add(racingCar);
        Thread thread = new Thread(racingCar.Drive);
        thread.IsBackground = true;
        thread.Start();
    }

    private void HandleCrash(RacingCar car)
    {
        foreach (var loader in Loaders)
        {
            if (loader.Status == "Ожидает" && !loader.HasWorked)
            {
                loader.Load(car);
                break;
            }
        }
    }
}
