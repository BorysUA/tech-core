using _Project.CodeBase.Gameplay.Constants;

namespace _Project.CodeBase.Gameplay.DataProxy
{
  public interface ICellStatus
  {
    public CellContentType ContentMask { get; }
    string ConstructionPlotId { get; }
    string BuildingId { get; }
    ResourceKind ResourceSpotKind { get; }
    bool HasContent(CellContentType content);
  }
}