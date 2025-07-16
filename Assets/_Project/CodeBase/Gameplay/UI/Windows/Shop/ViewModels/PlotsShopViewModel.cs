using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Services.BuildingPlots;
using _Project.CodeBase.Gameplay.Signals.Command;
using _Project.CodeBase.UI.Core;
using ObservableCollections;
using Zenject;

namespace _Project.CodeBase.Gameplay.UI.Windows.Shop.ViewModels
{
  public class PlotsShopViewModel : BaseWindowViewModel
  {
    private readonly SignalBus _signalBus;
    private readonly IConstructionPlotService _constructionPlotService;
    private readonly ObservableList<ConstructionPlotType> _itemsToShow = new();

    public IObservableCollection<ConstructionPlotType> ItemsToShow => _itemsToShow;

    public PlotsShopViewModel(IConstructionPlotService constructionPlotService,
      SignalBus signalBus)
    {
      _signalBus = signalBus;
      _constructionPlotService = constructionPlotService;
    }

    public override void Initialize()
    {
      foreach (ConstructionPlotInfo plotInfo in _constructionPlotService.AvailablePlots)
        _itemsToShow.Add(plotInfo.Type);
    }

    public override void Reset() =>
      _itemsToShow.Clear();

    public void BuyItem(ConstructionPlotType plotType)
    {
      Close();
      _signalBus.Fire(new ConstructionPlotPurchaseRequested(plotType));
    }
  }
}