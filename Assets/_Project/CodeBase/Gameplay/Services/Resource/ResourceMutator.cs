using System;
using _Project.CodeBase.Data.Progress.ResourceData;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Infrastructure.Services.ProgressProvider;

namespace _Project.CodeBase.Gameplay.Services.Resource
{
  public class ResourceMutator : IResourceMutator
  {
    private readonly IProgressWriter _progress;
    private readonly IResourceQuery _query;

    public ResourceMutator(IProgressWriter progress, IResourceQuery query)
    {
      _progress = progress;
      _query = query;
    }

    public ResourceMutationResult TryAddStrict(ReadOnlySpan<ResourceAmountData> requested,
      Span<ResourceAmountData> resultBuffer)
    {
      ResourceMutationStatus resourceMutationStatus =
        CanAdd(requested, (free, resource) => free >= resource.Amount, resultBuffer);

      if (resourceMutationStatus != ResourceMutationStatus.None)
        return new ResourceMutationResult(resourceMutationStatus);

      foreach (ResourceAmountData resource in resultBuffer)
        _progress.GameStateModel.WriteOnlyResources[resource.Kind].Amount.Value += resource.Amount;

      return ResourceMutationResult.Success;
    }

    public ResourceMutationResult TryAddFlexible(ReadOnlySpan<ResourceAmountData> requested,
      Span<ResourceAmountData> resultBuffer)
    {
      ResourceMutationStatus resourceMutationStatus = CanAdd(requested, (free, _) => free > 0, resultBuffer);

      if (resourceMutationStatus != ResourceMutationStatus.None)
        return new ResourceMutationResult(resourceMutationStatus);

      foreach (ResourceAmountData resource in resultBuffer)
        _progress.GameStateModel.WriteOnlyResources[resource.Kind].Amount.Value += resource.Amount;

      return ResourceMutationResult.Success;
    }

    public ResourceMutationResult TrySpend(ReadOnlySpan<ResourceAmountData> requested)
    {
      ResourceMutationStatus resourceMutationStatus = CanSpend(requested);

      if (resourceMutationStatus != ResourceMutationStatus.None)
        return new ResourceMutationResult(resourceMutationStatus);

      foreach (ResourceAmountData resourceAmountData in requested)
        _progress.GameStateModel.WriteOnlyResources[resourceAmountData.Kind].Amount.Value -= resourceAmountData.Amount;

      return ResourceMutationResult.Success;
    }

    public ResourceMutationResult TryTransferFlexible(ReadOnlySpan<ResourceAmountData> requestedSpend,
      ReadOnlySpan<ResourceAmountData> requestedAdd, Span<ResourceAmountData> resultBuffer)
    {
      ResourceMutationStatus spendResourceMutationStatus = CanSpend(requestedSpend);
      ResourceMutationStatus addResourceMutationStatus = CanAdd(requestedAdd, (free, _) => free > 0, resultBuffer);

      if (spendResourceMutationStatus != ResourceMutationStatus.None)
        return new ResourceMutationResult(spendResourceMutationStatus);
      if (addResourceMutationStatus != ResourceMutationStatus.None)
        return new ResourceMutationResult(addResourceMutationStatus);

      foreach (ResourceAmountData resourceAmountData in requestedSpend)
        _progress.GameStateModel.WriteOnlyResources[resourceAmountData.Kind].Amount.Value -= resourceAmountData.Amount;

      foreach (ResourceAmountData resource in resultBuffer)
        _progress.GameStateModel.WriteOnlyResources[resource.Kind].Amount.Value += resource.Amount;

      return ResourceMutationResult.Success;
    }

    private ResourceMutationStatus CanSpend(ReadOnlySpan<ResourceAmountData> bundle)
    {
      foreach (ResourceAmountData resource in bundle)
      {
        if (resource.Amount <= 0)
          return ResourceMutationStatus.InvalidAmount;

        if (!_progress.GameStateModel.WriteOnlyResources.ContainsKey(resource.Kind))
          return ResourceMutationStatus.InvalidResource;

        ResourceStorage storage = _query.Get(resource.Kind);
        if (resource.Amount > storage.Amount)
          return ResourceMutationStatus.NotEnoughResources;
      }

      return ResourceMutationStatus.None;
    }

    private ResourceMutationStatus CanAdd(ReadOnlySpan<ResourceAmountData> bundle,
      Func<int, ResourceAmountData, bool> comparison, Span<ResourceAmountData> planBuffer)
    {
      if (planBuffer.Length < bundle.Length)
        return ResourceMutationStatus.BufferOverflow;

      for (int i = 0; i < bundle.Length; i++)
      {
        ResourceAmountData resource = bundle[i];

        if (resource.Amount <= 0)
          return ResourceMutationStatus.InvalidAmount;

        if (!_progress.GameStateModel.WriteOnlyResources.ContainsKey(resource.Kind))
          return ResourceMutationStatus.InvalidResource;

        ResourceStorage storage = _query.Get(resource.Kind);
        int free = Math.Max(0, storage.Capacity - storage.Amount);

        if (!comparison.Invoke(free, resource))
          return ResourceMutationStatus.NoCapacity;

        planBuffer[i] = new ResourceAmountData(resource.Kind, Math.Clamp(resource.Amount, 0, free));
      }

      return ResourceMutationStatus.None;
    }
  }
}