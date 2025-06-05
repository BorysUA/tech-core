using System;
using _Project.CodeBase.Data.Progress.Building.ModuleData;
using _Project.CodeBase.Data.Progress.ResourceData;
using _Project.CodeBase.Gameplay.Building.Modules.Spaceport;
using _Project.CodeBase.Gameplay.Services.Resource;
using _Project.CodeBase.Services.LogService;
using _Project.CodeBase.UI.Common;
using R3;

namespace _Project.CodeBase.Gameplay.UI.Windows.Trade
{
  public class TradeViewModel : BaseWindowViewModel, IParameterizedWindow<SpaceportTradeModule>
  {
    private SpaceportTradeModule _tradeModule;

    private readonly ReactiveProperty<bool> _isOfferFulfillable = new();
    private readonly IResourceService _resourceService;
    private readonly ILogService _logService;

    private DisposableBag _resourceTrackSubscription;
    private DisposableBag _inactiveSubscription;

    public TradeOfferData CurrentTradeOffer => _tradeModule.CurrentTradeOffer;
    public bool AreOfferAvailable => _tradeModule.CurrentTradeOffer is not null;

    public ReadOnlyReactiveProperty<float> NextOfferOpenCountdown => _tradeModule.NextOfferOpenCountdown;
    public ReadOnlyReactiveProperty<bool> IsOfferFulfillable => _isOfferFulfillable;

    public event Action TradeOfferOpened;
    public event Action TradeOfferClosed;

    public TradeViewModel(IResourceService resourceService, ILogService logService)
    {
      _resourceService = resourceService;
      _logService = logService;
    }

    public void Initialize(SpaceportTradeModule tradeModule) =>
      _tradeModule = tradeModule;

    public override void Open()
    {
      if (!_tradeModule.IsActive.CurrentValue)
        return;

      _tradeModule.IsActive
        .Where(value => !value)
        .Subscribe(_ => Close())
        .AddTo(ref _inactiveSubscription);

      _tradeModule.TradeOfferOpened += OnTradeOfferOpened;
      _tradeModule.TradeOfferClosed += OnTradeOfferClosed;

      if (AreOfferAvailable)
        OnTradeOfferOpened();
      else
        OnTradeOfferClosed();

      base.Open();
    }

    public override void Close()
    {
      _resourceTrackSubscription.Clear();
      _inactiveSubscription.Clear();

      _tradeModule.TradeOfferOpened -= OnTradeOfferOpened;
      _tradeModule.TradeOfferClosed -= OnTradeOfferClosed;

      _tradeModule = null;
      base.Close();
    }

    public void SellItems()
    {
      if (!_tradeModule.IsActive.CurrentValue)
      {
        _logService.LogError(GetType(), "Attempted to fulfill trade offer while trade module is inactive.");
        return;
      }

      _tradeModule.FulfillOffer();
    }

    private void OnTradeOfferOpened()
    {
      foreach (ResourceAmountData resourceToSell in CurrentTradeOffer.PurchaseResources)
      {
        _resourceService.ObserveResource(resourceToSell.Kind)
          .Subscribe(resourceAmount => { _isOfferFulfillable.Value = resourceAmount >= resourceToSell.Amount; })
          .AddTo(ref _resourceTrackSubscription);
      }

      TradeOfferOpened?.Invoke();
    }

    private void OnTradeOfferClosed()
    {
      _resourceTrackSubscription.Clear();
      TradeOfferClosed?.Invoke();
    }
  }
}