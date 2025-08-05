using System;
using System.Collections.Generic;
using _Project.CodeBase.Data.Progress.ResourceData;
using _Project.CodeBase.Gameplay.Services.Command;

namespace _Project.CodeBase.Gameplay.Services.Resource.Commands
{
  public readonly struct SpendResourcesCommand : ICommand<ResourceMutationStatus>
  {
    private readonly ResourceAmountData[] _resources;

    public readonly ResourceSink Sink;
    public ReadOnlySpan<ResourceAmountData> Resources => _resources;

    public SpendResourcesCommand(ResourceAmountData[] resources, ResourceSink sink)
    {
      _resources = resources;
      Sink = sink;
    }
  }
}