using System;
using _Project.CodeBase.Data.Progress.ResourceData;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Models.Persistent.Interfaces;
using _Project.CodeBase.Gameplay.Models.Session;
using _Project.CodeBase.Gameplay.Resource.Behaviours;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Gameplay.Services.Resource.Commands;
using _Project.CodeBase.Gameplay.Services.Resource.Results;
using _Project.CodeBase.Gameplay.States;
using _Project.CodeBase.Infrastructure.Services.ProgressProvider;
using _Project.CodeBase.Infrastructure.StateMachine;
using R3;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Services.Resource
{
  public class ResourceService : IResourceService, IGameplayInit, IResourceQuery
  {
    private readonly ISessionProgress _sessionStateModel;
    private readonly ResourceBehaviourMap _resourceBehaviourMap;
    private readonly ICommandBroker _commandBroker;
    private readonly IProgressReader _progressReader;

    private readonly ReactiveProperty<ResourceDropCollectedArgs> _resourceDropCollected = new();

    public ReadOnlyReactiveProperty<ResourceDropCollectedArgs> ResourceDropCollected => _resourceDropCollected;
    public InitPhase InitPhase => InitPhase.Preparation;

    public ResourceService(IProgressReader progressReader, ICommandBroker commandBroker,
      ResourceBehaviourMap resourceBehaviourMap, ISessionProgress sessionStateModel)
    {
      _progressReader = progressReader;
      _commandBroker = commandBroker;
      _resourceBehaviourMap = resourceBehaviourMap;
      _sessionStateModel = sessionStateModel;
    }

    public void Initialize()
    {
      foreach (IResourceReader resource in _progressReader.GameStateModel.ReadOnlyResources.Values)
        _resourceBehaviourMap.Map[resource.Kind].Setup(resource, _sessionStateModel.GetResourceModel(resource.Kind));
    }

    public ResourceStorage Get(ResourceKind kind) =>
      _resourceBehaviourMap.Map.TryGetValue(kind, out IResourceBehaviour resourceBehaviour)
        ? new ResourceStorage(resourceBehaviour.TotalAmount.CurrentValue, resourceBehaviour.TotalCapacity.CurrentValue)
        : default;

    public void AddResource(ResourceKind kind, int amount)
    {
      AddResourceCommand command = new AddResourceCommand(kind, amount);
      _commandBroker.ExecuteCommand<AddResourceCommand, ResourceMutationStatus>(command);
    }

    public bool TrySpend(ResourceKind kind, ResourceSink sink, int amount)
    {
      SpendResourceCommand command = new SpendResourceCommand(kind, amount, sink);
      ResourceMutationStatus mutationStatus =
        _commandBroker.ExecuteCommand<SpendResourceCommand, ResourceMutationStatus>(command);

      return mutationStatus is ResourceMutationStatus.Success;
    }

    public bool TrySpend(ResourceSink sink, params ResourceAmountData[] resources)
    {
      SpendResourcesCommand command = new SpendResourcesCommand(resources, sink);
      ResourceMutationStatus mutationStatus =
        _commandBroker.ExecuteCommand<SpendResourcesCommand, ResourceMutationStatus>(command);

      return mutationStatus is ResourceMutationStatus.Success;
    }

    public bool IncreaseCapacity(ResourceKind kind, int amount)
    {
      IncreaseResourceCapacityCommand command = new IncreaseResourceCapacityCommand(kind, amount);
      return _commandBroker.ExecuteCommand<IncreaseResourceCapacityCommand, bool>(command);
    }

    public bool DecreaseCapacity(ResourceKind kind, int amount)
    {
      DecreaseResourceCapacityCommand command = new DecreaseResourceCapacityCommand(kind, amount);
      return _commandBroker.ExecuteCommand<DecreaseResourceCapacityCommand, bool>(command);
    }

    public bool TryReserve(ResourceKind kind, int amount, out ReservationToken token)
    {
      if (_resourceBehaviourMap.Map.TryGetValue(kind, out IResourceBehaviour resource) &&
          resource is IReservableResourceBehaviour reservableResource)
        return reservableResource.TryReserve(amount, out token);

      token = null;
      return false;
    }

    public ReadOnlyReactiveProperty<int> ObserveResource(ResourceKind kind)
    {
      if (TryGetBehaviour(kind, out IResourceBehaviour resource) && resource.TotalAmount != null)
        return resource.TotalAmount;

      throw new InvalidOperationException(
        $"ObserveResource({kind}) called before Initialize or resource does not exist");
    }

    public ReadOnlyReactiveProperty<int> ObserveCapacity(ResourceKind kind)
    {
      if (TryGetBehaviour(kind, out IResourceBehaviour resource) && resource.TotalCapacity != null)
        return resource.TotalCapacity;

      throw new InvalidOperationException(
        $"ObserveCapacity({kind}) called before Initialize or resource does not exist");
    }

    public void AddResourceDrop(ResourceDropType resourceDropType, Vector3 spawnPoint, Vector3 position)
    {
      AddResourceDropCommand command =
        new AddResourceDropCommand(resourceDropType, spawnPoint, position);

      _commandBroker.ExecuteCommand(command);
    }

    public void CollectDrop(int id)
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

    private bool TryGetBehaviour(ResourceKind kind, out IResourceBehaviour resource) =>
      _resourceBehaviourMap.Map.TryGetValue(kind, out resource);
  }
}