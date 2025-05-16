using System;
using System.Collections.ObjectModel;
using System.Threading;
using AvaloniaApp.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApp.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    public ObservableCollection<RacingCar> RacingCars { get; set; }

    [RelayCommand]
    private void AddRacingCar()
    {
        var racingCar = new RacingCar("Car" + (RacingCars.Count + 1), 100);
        racingCar.OnTireWornOut += (name) => _mechanic.ChangeTires(racingCar);
        racingCar.OnCrash += (name) => _loader.Load(racingCar);

        RacingCars.Add(racingCar);
        Thread thread = new Thread(racingCar.Drive);
        thread.Start();
    }

    private Mechanic _mechanic = new Mechanic();
    private ILoader _loader = new Loader();

    public MainWindowViewModel()
    {
        RacingCars = new ObservableCollection<RacingCar>();
    }
}