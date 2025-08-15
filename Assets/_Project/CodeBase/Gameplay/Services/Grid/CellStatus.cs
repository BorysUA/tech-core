using _Project.CodeBase.Gameplay.Constants;

namespace _Project.CodeBase.Gameplay.Services.Grid
{
  public readonly struct CellStatus
  {
    public static readonly CellStatus None = default;

    public CellContentType Mask { get; init; }
    public int BuildingId { get; init; }
    public int PlotId { get; init; }
    public ResourceKind ResourceSpotKind { get; init; }

    public bool IsEmpty =>
      Mask == CellContentType.None &&
      BuildingId == 0 &&
      PlotId == 0 &&
      ResourceSpotKind == ResourceKind.None;

    public bool HasContent(CellContentType c) => (Mask & c) != 0;
    public bool HasBuilding => BuildingId != 0;
    public bool HasPlot => PlotId != 0;
    public bool HasResourceSpot => ResourceSpotKind != ResourceKind.None;
  }
}