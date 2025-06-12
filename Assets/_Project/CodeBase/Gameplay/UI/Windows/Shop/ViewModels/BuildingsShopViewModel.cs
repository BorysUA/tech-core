using _Project.CodeBase.Data.StaticData.Building;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Services.Buildings;
using _Project.CodeBase.Gameplay.Signals;
using _Project.CodeBase.Gameplay.Signals.Command;
using _Project.CodeBase.Gameplay.UI.Windows.Shop.Item;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Services.LogService;
using _Project.CodeBase.UI.Common;
using Zenject;

namespace _Project.CodeBase.Gameplay.UI.Windows.Shop.ViewModels
{
  public class BuildingsShopViewModel : ShopViewModel, IParameterizedWindow<BuildingCategory>
  {
    private readonly ILogService _logService;
    private readonly SignalBus _signalBus;
    private readonly IBuildingService _buildingService;
    private readonly IStaticDataProvider _staticDataProvider;

    public BuildingsShopViewModel(IBuildingService buildingService, ILogService logService,
      IStaticDataProvider staticDataProvider, SignalBus signalBus)
    {
      _buildingService = buildingService;
      _logService = logService;
      _staticDataProvider = staticDataProvider;
      _signalBus = signalBus;
    }

    public void Initialize(BuildingCategory category)
    {
      foreach (BuildingType type in _buildingService.AvailableBuildings)
      {
        BuildingConfig config = _staticDataProvider.GetBuildingConfig(type);

        if (config.Category == category)
        {
          BuildingShopItem shopItem = new BuildingShopItem(
            config.Type,
            config.Title,
            config.Icon,
            config.Price.Amount,
            config.Price.Resource.Icon);

          _items.Add(shopItem);
        }
      }
    }

    public override void BuyItem(IShopItem shopItem)
    {
      if (shopItem is not BuildingShopItem buildingShopItem)
      {
        _logService.LogError(GetType(), $"Invalid ShopItem type: {shopItem.GetType()}");
        return;
      }

      Close();

      _signalBus.Fire(new BuildingPurchaseRequested(buildingShopItem.ItemType));
    }
  }
}