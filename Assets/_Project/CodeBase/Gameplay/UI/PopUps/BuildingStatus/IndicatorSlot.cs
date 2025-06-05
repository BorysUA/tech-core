using System.Collections.Generic;
using R3;

namespace _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus
{
  public class IndicatorSlot
  {
    public readonly List<Indicator> Indicators = new();
    public readonly CompositeDisposable Disposable = new();
    public Indicator ActiveIndicator;
  }
}