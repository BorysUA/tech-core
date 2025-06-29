using System.Collections.Generic;
using _Project.CodeBase.Data.Progress.ResourceData;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.DataProxy;
using _Project.CodeBase.Gameplay.Resource;
using _Project.CodeBase.Gameplay.Resource.Behaviours;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Gameplay.Services.Factories;
using _Project.CodeBase.Gameplay.Services.Resource.Commands;
using _Project.CodeBase.Gameplay.Services.Resource.Results;
using _Project.CodeBase.Gameplay.States;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Infrastructure.StateMachine;
using _Project.CodeBase.Services.LogService;
using ObservableCollections;
using R3;
using UnityEngine;
using Zenject;

namespace _Project.CodeBase.Gameplay.Services.Resource
{
  public class ResourceService : IResourceService, IGameplayInit
  {
    private readonly Dictionary<string, ResourceDropViewModel> _resourceDrops = new();
    private readonly ResourceBehaviourMap _resourceBehaviourMap;
    private readonly ICommandBroker _commandBroker;
    private readonly ILogService _logService;
    private readonly IGameplayFactory _gameplayFactory;
    private readonly IProgressService _progressService;
    private readonly CompositeDisposable _compositeDisposable = new();

    private readonly ReactiveProperty<ResourceDropCollectedArgs> _resourceDropCollected = new();

    public ReadOnlyReactiveProperty<ResourceDropCollectedArgs> ResourceDropCollected => _resourceDropCollected;

    public ResourceService(IProgressService progressService, ICommandBroker commandBroker, ILogService logService,
      IGameplayFactory gameplayFactory, ResourceBehaviourMap resourceBehaviourMap)
    {
      _progressService = progressService;
      _commandBroker = commandBroker;
      _logService = logService;
      _gameplayFactory = gameplayFactory;
      _resourceBehaviourMap = resourceBehaviourMap;
    }

    public void Initialize()
    {
      foreach (var resource in _progressService.GameStateProxy.Resources)
        _resourceBehaviourMap.Map[resource.Key].Setup(resource.Value);

      foreach (var resourceEntry in _progressService.GameStateProxy.ResourceDrops)
        CreateResourceDropView(resourceEntry.Value);

      _progressService.GameStateProxy.ResourceDrops.ObserveAdd()
        .Subscribe(entry => CreateResourceDropView(entry.Value.Value))
        .AddTo(_compositeDisposable);

      _progressService.GameStateProxy.ResourceDrops.ObserveRemove()
        .Subscribe(entry => DestroyResourceDropView(entry.Value.Value))
        .AddTo(_compositeDisposable);
    }

    public bool TrySpend(ResourceKind kind, int amount) =>
      TryGetBehaviour(kind, out IResourceBehaviour resource) && resource.TrySpend(amount);

    public bool TrySpend(params ResourceAmountData[] resources)
    {
      var toSpend = new List<(IResourceBehaviour resourceBehaviour, int amount)>(resources.Length);

      foreach (ResourceAmountData resourceAmount in resources)
        if (TryGetBehaviour(resourceAmount.Kind, out IResourceBehaviour resourceBehaviour))
          toSpend.Add((resourceBehaviour, resourceAmount.Amount));

      foreach ((IResourceBehaviour resource, int amount) in toSpend)
        if (!resource.CanSpend(amount))
          return false;

      foreach ((IResourceBehaviour resource, int amount) in toSpend)
        resource.TrySpend(amount);

      return true;
    }

    public bool IncreaseCapacity(ResourceKind kind, int amount) =>
      TryGetBehaviour(kind, out IResourceBehaviour resource) && resource.IncreaseCapacity(amount);

    public bool DecreaseCapacity(ResourceKind kind, int amount) =>
      TryGetBehaviour(kind, out IResourceBehaviour resource) && resource.DecreaseCapacity(amount);
    
    public void AddResource(ResourceKind kind, int amount)
    {
      if (TryGetBehaviour(kind, out IResourceBehaviour resource))
        resource.Add(amount);
    }

    public bool TryReserve(ResourceKind kind, int amount, out ReservationToken token)
    {
      if (_resourceBehaviourMap.Map.TryGetValue(kind, out IResourceBehaviour resource) &&
          resource is IReservableResourceBehaviour reservableResource)
        return reservableResource.TryReserve(amount, out token);

      token = null;
      return false;
    }

    public ReadOnlyReactiveProperty<int> ObserveResource(ResourceKind kind) =>
      TryGetBehaviour(kind, out IResourceBehaviour resource) ? resource.AvailableAmount : null;

    public void AddResourceDrop(ResourceDropType resourceDropType, ResourceKind resourceKind, Vector3 spawnPoint,
      Vector3 position, int amount)
    {
      AddResourceDropCommand command =
        new AddResourceDropCommand(resourceDropType, resourceKind, spawnPoint, position, amount);

      _commandBroker.ExecuteCommand(command);
    }

    public void CollectDrop(string id)
    {
      CollectResourceDropCommand command = new CollectResourceDropCommand(id);
      CollectResourceDropResult result =
        _commandBroker.ExecuteCommand<CollectResourceDropCommand, CollectResourceDropResult>(command);

      if (result.IsSuccessful)
      {
        ResourceDropCollectedArgs eventArgs =
          new ResourceDropCollectedArgs(
            result.ResourceKind,
            result.Amount,
            result.Position);

        _resourceDropCollected.OnNext(eventArgs);
      }
    }

    private bool TryGetBehaviour(ResourceKind kind, out IResourceBehaviour resource)
    {
      return _resourceBehaviourMap.Map.TryGetValue(kind, out resource);
    }

    private async void CreateResourceDropView(ResourceDropProxy resourceDropProxy)
    {
      ResourceDropViewModel viewModel =
        await _gameplayFactory.CreateResourceDrop(resourceDropProxy.ResourceDropType,
          resourceDropProxy.SpawnPoint.CurrentValue);

      viewModel.Setup(resourceDropProxy);

      _resourceDrops.Add(resourceDropProxy.Id, viewModel);
    }

    private void DestroyResourceDropView(ResourceDropProxy resourceDropProxy)
    {
      if (_resourceDrops.Remove(resourceDropProxy.Id, out ResourceDropViewModel viewModel))
        viewModel.Deactivate();
      else
        _logService.LogError(GetType(), $"Key '{resourceDropProxy.Id}' not found in '{nameof(_resourceDrops)}'.");
    }
  }
}