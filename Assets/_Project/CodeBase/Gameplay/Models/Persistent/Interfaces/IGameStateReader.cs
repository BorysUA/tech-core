using _Project.CodeBase.Gameplay.Constants;

namespace _Project.CodeBase.Gameplay.Models.Persistent.Interfaces
{
  public interface IGameStateReader
  {
    public ISessionInfoReader SessionInfoReader { get; }
    public ReadOnlyDictionaryProjection<int, BuildingModel, IBuildingDataReader> ReadOnlyBuildings { get; }
    public ReadOnlyDictionaryProjection<ResourceKind, ResourceModel, IResourceReader> ReadOnlyResources { get; }
    public ReadOnlyDictionaryProjection<int, ResourceDropModel, IResourceDropReader> ReadOnlyResourceDrops { get; }
    ReadOnlyListProjection<ConstructionPlotModel, IPlotDataReader> ReadOnlyPlots { get; }
  }
}