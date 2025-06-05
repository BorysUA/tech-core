using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Resource;
using _Project.CodeBase.Gameplay.Services;
using _Project.CodeBase.Gameplay.Services.Resource;
using _Project.CodeBase.Services.InputService;
using _Project.CodeBase.Services.LogService;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.InputHandlers
{
  public class ResourceCollector : PlayerInputHandler
  {
    private readonly CoordinateMapper _coordinateMapper;
    private readonly IResourceService _resourceService;
    private readonly ILogService _logService;
    private readonly Collider[] _resources = new Collider[10];

    private static readonly LayerMask ResourcesLayerMask = 1 << LayerMask.NameToLayer(LayerName.ResourceDrop);

    private readonly float _grabRadius = 0.2f;

    public ResourceCollector(CoordinateMapper coordinateMapper, IResourceService resourceService,
      ILogService logService)
    {
      _coordinateMapper = coordinateMapper;
      _resourceService = resourceService;
      _logService = logService;
    }

    public override void OnTouchStarted(Vector2 inputPoint)
    {
      Vector3 worldPosition = _coordinateMapper.ScreenToWorldPoint(inputPoint);

      int hitCount = Physics.OverlapSphereNonAlloc(worldPosition, _grabRadius, _resources, ResourcesLayerMask);

      if (hitCount > 0)
      {
        for (int i = 0; i < hitCount; i++)
        {
          if (_resources[i].TryGetComponent(out ResourceDropView resourceDropView))
            _resourceService.CollectDrop(resourceDropView.Id);
          else
            _logService.LogWarning(GetType(),
              $"Component of type {typeof(ResourceDropView)} not found on {_resources[i].gameObject.name} in OverlapSphereNonAlloc result. " +
              $"This might indicate an unexpected object on the layer.");
        }
      }
    }
  }
}