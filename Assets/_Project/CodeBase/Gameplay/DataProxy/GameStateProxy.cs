using System.Collections.Generic;
using _Project.CodeBase.Data.Progress;
using _Project.CodeBase.Gameplay.Constants;
using ObservableCollections;
using R3;

namespace _Project.CodeBase.Gameplay.DataProxy
{
  public class GameStateProxy
  {
    public SessionInfoProxy SessionInfo { get; private set; }
    public ObservableDictionary<string, BuildingDataProxy> BuildingsCollection { get; } = new();
    public ObservableList<ConstructionPlotDataProxy> ConstructionPlotsCollection { get; } = new();
    public ObservableDictionary<ResourceKind, ResourceProxy> Resources { get; } = new();
    public ObservableDictionary<string, ResourceDropProxy> ResourceDrops { get; } = new();

    public GameStateData GameStateData;

    public void Initialize(GameStateData gameStateData)
    {
      GameStateData = gameStateData;

      ObserveResourceDrops();
      ObserveBuildings();
      ObserveConstructionPlots();

      foreach (var resource in gameStateData.Resources)
      {
        ResourceProxy resourceProxy = new ResourceProxy(resource.Value);
        Resources.Add(resourceProxy.Kind, resourceProxy);
      }

      SessionInfo = new SessionInfoProxy(gameStateData.SessionInfo);
    }

    private void ObserveConstructionPlots()
    {
      GameStateData.ConstructionPlots.ForEach(originData =>
      {
        ConstructionPlotDataProxy proxy = new ConstructionPlotDataProxy(originData);
        ConstructionPlotsCollection.Add(proxy);
      });

      ConstructionPlotsCollection
        .ObserveAdd()
        .Subscribe(addEvent =>
        {
          ConstructionPlotDataProxy proxy = addEvent.Value;
          GameStateData.ConstructionPlots.Add(proxy.Origin);
        });

      ConstructionPlotsCollection
        .ObserveRemove()
        .Subscribe(removeEvent =>
        {
          ConstructionPlotDataProxy proxy = removeEvent.Value;
          GameStateData.ConstructionPlots.Remove(proxy.Origin);
        });
    }

    private void ObserveResourceDrops()
    {
      foreach (KeyValuePair<string, ResourceDropData> resourceEntry in GameStateData.ResourceDrops)
        ResourceDrops.Add(resourceEntry.Key, new ResourceDropProxy(resourceEntry.Value));

      ResourceDrops
        .ObserveAdd()
        .Subscribe(addEvent =>
        {
          var resourceEntry = addEvent.Value;
          GameStateData.ResourceDrops.Add(resourceEntry.Key, resourceEntry.Value.Origin);
        });

      ResourceDrops
        .ObserveRemove()
        .Subscribe(removeEvent => GameStateData.ResourceDrops.Remove(removeEvent.Value.Key));
    }

    private void ObserveBuildings()
    {
      foreach (var buildingEntry in GameStateData.Buildings)
      {
        BuildingDataProxy buildingDataProxy = new BuildingDataProxy(buildingEntry.Value);
        BuildingsCollection.Add(buildingEntry.Key, buildingDataProxy);
      }

      BuildingsCollection
        .ObserveAdd()
        .Subscribe(addEvent =>
        {
          var buildingEntry = addEvent.Value;
          BuildingDataProxy newBuilding = buildingEntry.Value;
          GameStateData.Buildings.Add(buildingEntry.Key, newBuilding.Origin);
        });

      BuildingsCollection
        .ObserveRemove()
        .Subscribe(removeEvent =>
        {
          var buildingEntry = removeEvent.Value;
          GameStateData.Buildings.Remove(buildingEntry.Key);
        });
    }
  }
}