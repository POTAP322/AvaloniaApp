using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Threading;
using AvaloniaApp.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApp.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<RacingCar> _racingCars = new();

    [ObservableProperty]
    private ObservableCollection<Loader> _loaders = new();

    private readonly Mechanic _mechanic = new();
    private int _nextCarId = 1;

    public MainWindowViewModel()
    {
        // Добавляем 3 погрузчика через Dispatcher
        Dispatcher.UIThread.Post(() =>
        {
            for (int i = 0; i < 3; i++)
            {
                Loaders.Add(new Loader(i + 1));
            }
        });

        // Добавляем 2 машины при запуске
        for (int i = 0; i < 2; i++)
        {
            AddNewCar();
        }
    }

    [RelayCommand]
    private void AddNewCar()
    {
        var racingCar = new RacingCar(
            _nextCarId++, 
            $"Car{_nextCarId}", 
            new Random().Next(80, 120));
        
        racingCar.OnTireWornOut += HandleTireWornOut;
        racingCar.OnCrash += HandleCrash;

        // Добавляем машину через Dispatcher
        Dispatcher.UIThread.Post(() => RacingCars.Add(racingCar));
        
        Task.Run(() => racingCar.Drive());
    }

    private async void HandleTireWornOut(RacingCar car)
    {
        await Task.Run(() => _mechanic.ChangeTires(car));
        UpdateCarState(car);
    }

    private async void HandleCrash(RacingCar car)
    {
        await Task.Run(() =>
        {
            foreach (var loader in Loaders)
            {
                if (loader.Status == "Ожидает" && !loader.HasWorked)
                {
                    loader.Load(car);
                    UpdateLoaderState(loader);
                    UpdateCarState(car);
                    break;
                }
            }
        });
    }

    private void UpdateCarState(RacingCar car)
    {
        Dispatcher.UIThread.Post(() =>
        {
            var index = RacingCars.IndexOf(car);
            if (index >= 0)
            {
                RacingCars[index] = car;
            }
        });
    }

    private void UpdateLoaderState(Loader loader)
    {
        Dispatcher.UIThread.Post(() =>
        {
            var index = Loaders.IndexOf(loader);
            if (index >= 0)
            {
                Loaders[index] = loader;
            }
        });
    }

    [RelayCommand]
    private void StopAllCars()
    {
        foreach (var car in RacingCars)
        {
            car.Stop();
        }
    }
}