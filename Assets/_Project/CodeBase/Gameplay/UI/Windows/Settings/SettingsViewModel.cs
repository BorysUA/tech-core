using System;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Services;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Infrastructure.Services.SaveService;
using _Project.CodeBase.UI.Common;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.UI.Windows.Settings
{
  public class SettingsViewModel : BaseWindowViewModel
  {
    private readonly IGameSaveService _gameSaveService;
    private readonly INotificationService _notificationService;

    public SettingsViewModel(IGameSaveService gameSaveService, INotificationService notificationService)
    {
      _gameSaveService = gameSaveService;
      _notificationService = notificationService;
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
  }
}