namespace _Project.CodeBase.Data.Progress.Building.ModuleData
{
  public interface IReadOnlyTradeData : IModuleData
  {
    public int CompletedTradesCount { get; }
    public TradeOfferData CurrentOffer { get; }
    public float OfferCloseCountdown { get; }
    public float NextOfferOpenCountdown { get; }
  }
}