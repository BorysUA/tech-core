using System;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Signals.Command;
using _Project.CodeBase.Gameplay.UI.HUD.Notification;
using _Project.CodeBase.Infrastructure.Services.SaveService;
using _Project.CodeBase.UI.Core;
using UnityEngine;
using Zenject;

namespace _Project.CodeBase.Gameplay.UI.Windows.Settings
{
  public class SettingsViewModel : BaseWindowViewModel
  {
    private readonly IGameSaveService _gameSaveService;
    private readonly INotificationService _notificationService;
    private readonly SignalBus _signalBus;

    public SettingsViewModel(IGameSaveService gameSaveService, INotificationService notificationService,
      SignalBus signalBus)
    {
      _gameSaveService = gameSaveService;
      _notificationService = notificationService;
      _signalBus = signalBus;
    }

    public async void SaveCurrentProgress()
    {
      try
      {
        await _gameSaveService.SaveManualAsync();
        _notificationService.ShowMessage(NotificationMessages.SaveSuccess, Color.green);
      }
      catch (Exception)
      {
        _notificationService.ShowMessage(NotificationMessages.SaveFailure, Color.red);
      }
    }

    public void ExitToMenu() =>
      _signalBus.Fire<ExitToMenuRequested>();
  }
}