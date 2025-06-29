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
    private readonly ObservableList<ConstructionPlotType> _itemsToShow = new();

    public IObservableCollection<ConstructionPlotType> ItemsToShow => _itemsToShow;

    public PlotsShopViewModel(IConstructionPlotService constructionPlotService,
      SignalBus signalBus)
    {
      _signalBus = signalBus;

      foreach (ConstructionPlotInfo plotInfo in constructionPlotService.AvailablePlots)
        _itemsToShow.Add(plotInfo.Type);
    }

    public void BuyItem(ConstructionPlotType plotType)
    {
      Close();
      _signalBus.Fire(new ConstructionPlotPurchaseRequested(plotType));
    }
  }
}