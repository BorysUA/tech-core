using _Project.CodeBase.Data.StaticData.Building;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Services.BuildingPlots;
using _Project.CodeBase.Gameplay.Signals;
using _Project.CodeBase.Gameplay.UI.Windows.Shop.Item;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Services.LogService;
using _Project.CodeBase.UI.Services;
using Zenject;

namespace _Project.CodeBase.Gameplay.UI.Windows.Shop.ViewModels
{
  public class ConstructionPlotsShopViewModel : ShopViewModel
  {
    private readonly SignalBus _signalBus;
    private readonly ILogService _logService;
    private readonly IWindowsService _windowsService;

    public ConstructionPlotsShopViewModel(IConstructionPlotService constructionPlotService,
      SignalBus signalBus, ILogService logService, IStaticDataProvider staticDataProvider,
      IWindowsService windowsService)
    {
      _signalBus = signalBus;
      _logService = logService;
      _windowsService = windowsService;

      foreach (ConstructionPlotType type in constructionPlotService.AvailableConstructionPlots)
      {
        ConstructionPlotConfig config = staticDataProvider.GetConstructionPlotConfig(type);

        ConstructionPlotShopItem shopItem = new ConstructionPlotShopItem(
          config.Type,
          config.Title,
          config.Icon,
          config.Price.Amount,
          config.Price.Resource.Icon);

        _items.Add(shopItem);
      }
    }

    public override void BuyItem(IShopItem shopItem)
    {
      if (shopItem is not ConstructionPlotShopItem constructionPlotShopItem)
      {
        _logService.LogError(GetType(), $"Invalid ShopItem type: {shopItem.GetType()}");
        return;
      }

      Close();

      _signalBus.Fire(new ConstructionPlotPurchaseRequested(constructionPlotShopItem.ItemType));
    }
  }
}