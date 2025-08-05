using _Project.CodeBase.Gameplay.Constants;

namespace _Project.CodeBase.Gameplay.Models.Persistent.Interfaces
{
  public interface IGameStateWriter
  {
    public ISessionInfoWriter SessionInfoWriter { get; }
    public WritableDictionaryProjection<int, BuildingModel, IBuildingDataWriter> WriteOnlyBuildings { get; }
    public WritableDictionaryProjection<ResourceKind, ResourceModel, IResourceWriter> WriteOnlyResources { get; }
    public WritableDictionaryProjection<int, ResourceDropModel, IResourceDropWriter> WriteOnlyResourceDrops { get; }
    WritableListProjection<ConstructionPlotModel, IPlotDataReader> WriteOnlyPlots { get; }
  }
}