using System;
using _Project.CodeBase.Data.Progress.ResourceData;

namespace _Project.CodeBase.Gameplay.Services.Resource
{
  public interface IResourceMutator
  {
    ResourceMutationResult TryAddStrict(ReadOnlySpan<ResourceAmountData> requested,
      Span<ResourceAmountData> resultBuffer);

    ResourceMutationResult TryAddFlexible(ReadOnlySpan<ResourceAmountData> requested,
      Span<ResourceAmountData> resultBuffer);

    ResourceMutationResult TrySpend(ReadOnlySpan<ResourceAmountData> requested);

    ResourceMutationResult TryTransferFlexible(ReadOnlySpan<ResourceAmountData> requestedSpend,
      ReadOnlySpan<ResourceAmountData> requestedAdd, Span<ResourceAmountData> resultBuffer);
  }
}