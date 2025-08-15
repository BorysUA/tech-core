using System.Collections.Generic;
using _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus;
using R3;
using Vector3 = _Project.CodeBase.Gameplay.Models.Vector3;

namespace _Project.CodeBase.Gameplay.Buildings
{
  public interface IBuildingIndicatorReader
  {
    public int Id { get; }
    public IEnumerable<IBuildingIndicatorSource> Indicators { get; }
    public Observable<Unit> Destroyed { get; }
    public Vector3 WorldPosition { get; }
  }
}