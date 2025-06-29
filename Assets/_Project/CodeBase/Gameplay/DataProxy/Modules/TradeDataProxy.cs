using System;
using _Project.CodeBase.Data.Progress.Building.ModuleData;
using R3;

namespace _Project.CodeBase.Gameplay.DataProxy.Modules
{
  public class TradeDataProxy : IDisposable
  {
    private readonly TradeData _tradeData;
    private readonly CompositeDisposable _disposable = new();

    public ReactiveProperty<float> OfferCloseCountdown = new();
    public ReactiveProperty<float> NextOfferOpenCountdown = new();
    public ReactiveProperty<TradeOfferData> CurrentTradeOffer = new();

    public string BuildingId => _tradeData.BuildingId;
    public int CompletedTradesCount => _tradeData.CompletedTradesCount;

    public TradeDataProxy(TradeData tradeData)
    {
      _tradeData = tradeData;

      OfferCloseCountdown.Value = _tradeData.OfferCloseCountdown;
      NextOfferOpenCountdown.Value = _tradeData.NextOfferOpenCountdown;
      CurrentTradeOffer.Value = _tradeData.CurrentOffer;

      OfferCloseCountdown
        .Subscribe(x => tradeData.OfferCloseCountdown = x)
        .AddTo(_disposable);

      NextOfferOpenCountdown
        .Subscribe(x => tradeData.NextOfferOpenCountdown = x)
        .AddTo(_disposable);

      CurrentTradeOffer
        .Subscribe(x => tradeData.CurrentOffer = x)
        .AddTo(_disposable);
    }

    public void IncrementFulfillOffersCount()
    {
      _tradeData.CompletedTradesCount++;
    }

    public void Dispose()
    {
      _disposable?.Dispose();
    }
  }
}