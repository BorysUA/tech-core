using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Services.Buildings;
using _Project.CodeBase.Gameplay.Signals.Command;
using _Project.CodeBase.UI.Core;
using ObservableCollections;
using Zenject;

namespace _Project.CodeBase.Gameplay.UI.Windows.Shop.ViewModels
{
  public class BuildingsShopViewModel : BaseWindowViewModel, IParameterizedWindow<BuildingCategory>
  {
    private readonly SignalBus _signalBus;
    private readonly IBuildingService _buildingService;
    private readonly ObservableList<BuildingType> _itemsToShow = new();

    private BuildingCategory _currentCategory;

    public IObservableCollection<BuildingType> ItemsToShow => _itemsToShow;

    public BuildingsShopViewModel(IBuildingService buildingService, SignalBus signalBus)
    {
      _buildingService = buildingService;
      _signalBus = signalBus;
    }

    public void Initialize(BuildingCategory category)
    {
      _currentCategory = category;

      foreach (BuildingInfo buildingInfo in _buildingService.AvailableSortedBuildings[category])
        _itemsToShow.Add(buildingInfo.Type);
    }

    public bool Matches(BuildingCategory param) =>
      _currentCategory == param;

    public override void Reset() =>
      _itemsToShow.Clear();

    public void BuyItem(BuildingType buildingType)
    {
      Close();
      _signalBus.Fire(new BuildingPurchaseRequested(buildingType));
    }
  }
}