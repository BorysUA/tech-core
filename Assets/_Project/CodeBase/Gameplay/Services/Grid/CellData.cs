using _Project.CodeBase.Gameplay.Constants;

namespace _Project.CodeBase.Gameplay.Services.Grid
{
  public class CellData : ICellStatus
  {
    public string ConstructionPlotId { get; private set; }
    public int BuildingId { get; private set; }
    public ResourceKind ResourceSpotKind { get; private set; } = ResourceKind.None;
    public CellContentType ContentMask { get; private set; } = CellContentType.None;

    public bool HasContent(CellContentType content) =>
      (ContentMask & content) != 0;

    public void SetConstructionPlot(string id)
    {
      ConstructionPlotId = id;
      AddContentToMask(CellContentType.ConstructionPlot);
    }

    public void SetBuilding(int id)
    {
      BuildingId = id;
      AddContentToMask(CellContentType.Building);
    }

    public void SetResourceSpot(ResourceKind resourceType)
    {
      ResourceSpotKind = resourceType;
      AddContentToMask(CellContentType.ResourceSpot);
    }

    public void SetObstacle()
    {
      AddContentToMask(CellContentType.Obstacle);
    }

    public void RemoveBuilding()
    {
      BuildingId = -1;
      RemoveContentFromMask(CellContentType.Building);
    }

    public void RemoveConstructionPlot()
    {
      ConstructionPlotId = null;
      RemoveContentFromMask(CellContentType.ConstructionPlot);
    }

    public void RemoveResourceSpot()
    {
      ResourceSpotKind = ResourceKind.None;
      RemoveContentFromMask(CellContentType.ResourceSpot);
    }

    private void AddContentToMask(CellContentType flag) =>
      ContentMask |= flag;

    private void RemoveContentFromMask(CellContentType flag) =>
      ContentMask &= ~flag;
  }
}