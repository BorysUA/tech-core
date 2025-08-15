using _Project.CodeBase.Gameplay.Constants;

namespace _Project.CodeBase.Gameplay.Services.Grid
{
  public interface ICellStatus
  {
    public CellContentType ContentMask { get; }
    int ConstructionPlotId { get; }
    int BuildingId { get; }
    ResourceKind ResourceSpotKind { get; }
    bool HasContent(CellContentType content);
  }
}