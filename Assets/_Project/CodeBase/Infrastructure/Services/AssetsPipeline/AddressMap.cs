using System;
using System.Collections.Generic;
using _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus;
using _Project.CodeBase.Gameplay.UI.PopUps.ConfirmPlace;
using _Project.CodeBase.Gameplay.UI.Windows.Settings;
using _Project.CodeBase.Gameplay.UI.Windows.Shop.Windows;
using _Project.CodeBase.Gameplay.UI.Windows.Trade;
using _Project.CodeBase.Infrastructure.Constants;
using _Project.CodeBase.Menu.UI.DifficultySelection;
using _Project.CodeBase.Menu.UI.Menu;
using _Project.CodeBase.Menu.UI.SaveSelection;
using _Project.CodeBase.Services.LogService;

namespace _Project.CodeBase.Infrastructure.Services.AssetsPipeline
{
  public class AddressMap
  {
    private readonly ILogService _logService;

    private readonly Dictionary<Type, string> _map = new()
    {
      { typeof(MenuWindow), AssetAddress.MenuMainWindow },
      { typeof(DifficultySelectionWindow), AssetAddress.MenuDifficultySelectionWindow },
      { typeof(BuildingsShopWindow), AssetAddress.BuildingsShopWindow },
      { typeof(PlotsShopWindow), AssetAddress.PlotsShopWindow },
      { typeof(ConfirmPlacePopUp), AssetAddress.ConfirmBuildingPlacePopUp },
      { typeof(BuildingIndicatorsPopUp), AssetAddress.BuildingIndicatorsPopUp },
      { typeof(TradeWindow), AssetAddress.TradeWindow },
      { typeof(SaveSelectionWindow), AssetAddress.SaveSelectionWindow },
      { typeof(SettingsWindow), AssetAddress.SettingsWindow }
    };

    public AddressMap(ILogService logService) =>
      _logService = logService;

    public string GetAddress<T>()
    {
      if (_map.TryGetValue(typeof(T), out string address))
        return address;

      _logService.LogError(GetType(), $"No mapping for {typeof(T).Name}");
      return null;
    }
  }
}