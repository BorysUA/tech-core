using System;

namespace _Project.CodeBase.Data.Progress.Building.ModuleData
{
  [Serializable]
  public class TradeData : IModuleData
  {
    public string BuildingId { get; }
    public int CompletedTradesCount { set; get; }
    public TradeOfferData CurrentOffer { get; set; }
    public float OfferCloseCountdown { get; set; }
    public float NextOfferOpenCountdown { get; set; }

    public TradeData(string buildingId, int completedTradesCount)
    {
      BuildingId = buildingId;
      CompletedTradesCount = completedTradesCount;
    }
  }
}