using System.Collections.Generic;
using _Project.CodeBase.Data.Progress;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Models.Persistent.Interfaces;
using ObservableCollections;
using R3;

namespace _Project.CodeBase.Gameplay.Models.Persistent
{
  public class GameStateModel : IGameStateReader, IGameStateWriter, IGameStateSaver
  {
    private ObservableDictionary<ResourceKind, ResourceModel> _resources;
    private ObservableDictionary<int, ResourceDropModel> _resourceDrops;
    private ObservableDictionary<int, BuildingModel> _buildingsCollection;
    private ObservableList<ConstructionPlotModel> _constructionPlotsCollection;
    private SessionInfoModel _sessionInfo;

    public GameStateData GameStateData { get; private set; }
    public ISessionInfoReader SessionInfoReader => _sessionInfo;
    public ISessionInfoWriter SessionInfoWriter => _sessionInfo;
    public ReadOnlyListProjection<ConstructionPlotModel, IPlotDataReader> ReadOnlyPlots { get; private set; }
    public WritableListProjection<ConstructionPlotModel, IPlotDataReader> WriteOnlyPlots { get; private set; }

    // @formatter:off
    public ReadOnlyDictionaryProjection<int, BuildingModel, IBuildingDataReader> ReadOnlyBuildings { get; private set; }
    public WritableDictionaryProjection<int, BuildingModel, IBuildingDataWriter> WriteOnlyBuildings { get; private set; }
    public ReadOnlyDictionaryProjection<ResourceKind, ResourceModel, IResourceReader> ReadOnlyResources { get; private set; }
    public WritableDictionaryProjection<ResourceKind, ResourceModel, IResourceWriter> WriteOnlyResources { get; private set; }
    public ReadOnlyDictionaryProjection<int, ResourceDropModel, IResourceDropReader> ReadOnlyResourceDrops { get; private set; }
    public WritableDictionaryProjection<int, ResourceDropModel, IResourceDropWriter> WriteOnlyResourceDrops { get; private set; }
    // @formatter:on

    public GameStateModel(GameStateData gameStateData)
    {
      GameStateData = gameStateData;

      InitializeResourceDrops();
      InitializeBuildings();
      InitializeConstructionPlots();
      InitializeSessionInfo();
      InitializeResources();
    }

    private void InitializeResources()
    {
      _resources = new ObservableDictionary<ResourceKind, ResourceModel>(GameStateData.Resources.Count,
        EqualityComparer<ResourceKind>.Default);

      ReadOnlyResources = new ReadOnlyDictionaryProjection<ResourceKind, ResourceModel, IResourceReader>(_resources);
      WriteOnlyResources = new WritableDictionaryProjection<ResourceKind, ResourceModel, IResourceWriter>(_resources);

      foreach (var resource in GameStateData.Resources)
      {
        ResourceModel resourceModel = new ResourceModel(resource.Value);
        _resources.Add(resourceModel.Kind, resourceModel);
      }
    }

    private void InitializeConstructionPlots()
    {
      _constructionPlotsCollection =
        new ObservableList<ConstructionPlotModel>(GameStateData.ConstructionPlots.Count);

      ReadOnlyPlots =
        new ReadOnlyListProjection<ConstructionPlotModel, IPlotDataReader>(_constructionPlotsCollection);

      WriteOnlyPlots = new WritableListProjection<ConstructionPlotModel, IPlotDataReader>(_constructionPlotsCollection);

      GameStateData.ConstructionPlots.ForEach(originData =>
      {
        ConstructionPlotModel proxy = new ConstructionPlotModel(originData);
        _constructionPlotsCollection.Add(proxy);
      });

      _constructionPlotsCollection
        .ObserveAdd()
        .Subscribe(addEvent =>
        {
          ConstructionPlotModel proxy = addEvent.Value;
          GameStateData.ConstructionPlots.Add(proxy.Source);
        });

      _constructionPlotsCollection
        .ObserveRemove()
        .Subscribe(removeEvent =>
        {
          ConstructionPlotModel proxy = removeEvent.Value;
          GameStateData.ConstructionPlots.Remove(proxy.Source);
        });
    }

    private void InitializeSessionInfo()
    {
      _sessionInfo = new SessionInfoModel(GameStateData.SessionInfo);
    }

    private void InitializeResourceDrops()
    {
      _resourceDrops = new ObservableDictionary<int, ResourceDropModel>(GameStateData.ResourceDrops.Count,
        EqualityComparer<int>.Default);

      ReadOnlyResourceDrops =
        new ReadOnlyDictionaryProjection<int, ResourceDropModel, IResourceDropReader>(_resourceDrops);

      WriteOnlyResourceDrops =
        new WritableDictionaryProjection<int, ResourceDropModel, IResourceDropWriter>(_resourceDrops);

      foreach (KeyValuePair<int, ResourceDropData> resourceEntry in GameStateData.ResourceDrops)
        _resourceDrops.Add(resourceEntry.Key, new ResourceDropModel(resourceEntry.Value));

      _resourceDrops
        .ObserveAdd()
        .Subscribe(addEvent =>
        {
          var resourceEntry = addEvent.Value;
          GameStateData.ResourceDrops.Add(resourceEntry.Key, resourceEntry.Value.Source);
        });

      _resourceDrops
        .ObserveRemove()
        .Subscribe(removeEvent => GameStateData.ResourceDrops.Remove(removeEvent.Value.Key));
    }

    private void InitializeBuildings()
    {
      _buildingsCollection = new ObservableDictionary<int, BuildingModel>(GameStateData.Buildings.Count,
        EqualityComparer<int>.Default);

      ReadOnlyBuildings =
        new ReadOnlyDictionaryProjection<int, BuildingModel, IBuildingDataReader>(_buildingsCollection);

      WriteOnlyBuildings =
        new WritableDictionaryProjection<int, BuildingModel, IBuildingDataWriter>(_buildingsCollection);

      foreach (var buildingEntry in GameStateData.Buildings)
      {
        BuildingModel buildingModel = new BuildingModel(buildingEntry.Value);
        _buildingsCollection.Add(buildingEntry.Key, buildingModel);
      }

      _buildingsCollection
        .ObserveAdd()
        .Subscribe(addEvent =>
        {
          var buildingEntry = addEvent.Value;
          BuildingModel newBuilding = buildingEntry.Value;
          GameStateData.Buildings.Add(buildingEntry.Key, newBuilding.Source);
        });

      _buildingsCollection
        .ObserveRemove()
        .Subscribe(removeEvent =>
        {
          var buildingEntry = removeEvent.Value;
          GameStateData.Buildings.Remove(buildingEntry.Key);
        });
    }
  }
}