using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Threading;
using AvaloniaApp.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading;

namespace AvaloniaApp.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<RacingCar> _racingCars = new();

    [ObservableProperty]
    private ObservableCollection<Loader> _loaders = new();

    private readonly Mechanic _mechanic = new();
    private int _nextCarId = 1;
    private DispatcherTimer _updateTimer;

    public MainWindowViewModel()
    {
        // Добавляем 3 погрузчика
        for (int i = 0; i < 3; i++)
        {
            Loaders.Add(new Loader(i + 1));
        }

        // Добавляем 2 машины при запуске
        for (int i = 0; i < 2; i++)
        {
            AddNewCar();
        }

        // Инициализация таймера для обновления данных
        _updateTimer = new DispatcherTimer();
        _updateTimer.Interval = TimeSpan.FromSeconds(3);
        _updateTimer.Tick += (sender, e) => UpdateAllCars();
        _updateTimer.Start();
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

        RacingCars.Add(racingCar);
        Task.Run(() => racingCar.Drive());
    }
    
    [RelayCommand]
    private void AddNewLoader()
    {
        int newLoaderId = Loaders.Count + 1;
        Loaders.Add(new Loader(newLoaderId));
    }


    private void UpdateAllCars()
    {
        foreach (var car in RacingCars)
        {
            // Пропускаем машины, которые уже эвакуированы или находятся в боксе
            if (car.IsCrashed || car.IsInPitStop || car.Condition == "Эвакуирован") continue;

            car.Distance += car.Speed;

            // Проверка на износ покрышек (5% вероятность)
            if (new Random().Next(1, 101) <= 5 && !car.IsTireWornOut)
            {
                car.IsTireWornOut = true;
                car.Condition = "Стерлись покрышки";
                car.Status = "Заехал в бокс";
                car.IsInPitStop = true;
                HandleTireWornOut(car);
            }

            // Проверка на аварию (2% вероятность)
            if (new Random().Next(1, 101) <= 2)
            {
                car.IsCrashed = true;
                car.Condition = "Авария";
                car.Status = "Ожидает эвакуации";
                HandleCrash(car);
            }
        }
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
