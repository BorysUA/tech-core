using System;
using System.Collections.Generic;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Services.Resource;
using R3;

namespace _Project.CodeBase.Gameplay.UI.HUD.ResourceBar
{
  public class ResourceBarViewModel
  {
    private readonly IResourceService _resourceService;
    private readonly Dictionary<ResourceKind, ReadOnlyReactiveProperty<string>> _amounts = new();
    private readonly Dictionary<ResourceKind, ReadOnlyReactiveProperty<string>> _capacities = new();

    public ResourceBarViewModel(IResourceService resourceService)
    {
      _resourceService = resourceService;
    }

    public void Initialize()
    {
      foreach (ResourceKind kind in Enum.GetValues(typeof(ResourceKind)))
      {
        if (kind == ResourceKind.None)
          continue;

        _amounts[kind] = _resourceService.ObserveResource(kind)
          .Select(v => v.ToString())
          .ToReadOnlyReactiveProperty();

        _capacities[kind] = _resourceService.ObserveCapacity(kind)
          .Select(v => v.ToString())
          .ToReadOnlyReactiveProperty();
      }
    }

    public ReadOnlyReactiveProperty<string> GetAmount(ResourceKind kind) => _amounts[kind];
    public ReadOnlyReactiveProperty<string> GetCapacity(ResourceKind kind) => _capacities[kind];
  }
}