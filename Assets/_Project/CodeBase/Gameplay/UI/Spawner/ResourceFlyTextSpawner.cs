using System;
using System.Collections.Generic;
using _Project.CodeBase.Data.StaticData.Resource;
using _Project.CodeBase.Gameplay.InputHandlers;
using _Project.CodeBase.Gameplay.Services;
using _Project.CodeBase.Gameplay.Services.Resource;
using _Project.CodeBase.Gameplay.States;
using _Project.CodeBase.Gameplay.UI.Factory;
using _Project.CodeBase.Gameplay.UI.Indicators;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Infrastructure.StateMachine;
using _Project.CodeBase.UI.Services;
using R3;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.UI.Spawner
{
  public class ResourceFlyTextSpawner : IDisposable, IGameplayInit
  {
    private readonly IResourceService _resourceService;
    private readonly CompositeDisposable _disposable = new();
    private readonly IGameplayUiFactory _gameplayUiFactory;
    private readonly IStaticDataProvider _staticDataProvider;
    private readonly IPopUpService _popUpService;
    private readonly ICameraMovement _cameraMovement;
    private readonly CoordinateMapper _coordinateMapper;

    private readonly List<FlyText> _activeFlyTexts = new();

    public ResourceFlyTextSpawner(IResourceService resourceService, IGameplayUiFactory gameplayUiFactory,
      IStaticDataProvider staticDataProvider, IPopUpService popUpService, ICameraMovement cameraMovement,
      CoordinateMapper coordinateMapper)
    {
      _resourceService = resourceService;
      _gameplayUiFactory = gameplayUiFactory;
      _staticDataProvider = staticDataProvider;
      _popUpService = popUpService;
      _cameraMovement = cameraMovement;
      _coordinateMapper = coordinateMapper;
    }

    public void Initialize()
    {
      _resourceService.ResourceDropCollected
        .Skip(1)
        .Subscribe(SpawnFlyText)
        .AddTo(_disposable);

      _cameraMovement.MovementDelta
        .Subscribe(UpdateFlyTextPositions)
        .AddTo(_disposable);
    }

    public void Dispose() =>
      _disposable?.Dispose();

    private void UpdateFlyTextPositions(Vector3 movementDelta)
    {
      foreach (FlyText flyText in _activeFlyTexts)
      {
        Vector2 screenPoint = _coordinateMapper.WorldToScreenPoint(flyText.WorldSpawnPoint);
        if (_popUpService.ScreenToCanvasPoint(screenPoint, out Vector2 localPoint))
          flyText.SetPosition(localPoint);
      }
    }

    private async void SpawnFlyText(ResourceDropCollectedArgs resourceDropInfo)
    {
      ResourceConfig resourceConfig = _staticDataProvider.GetResourceConfig(resourceDropInfo.ResourceKind);
      FlyText flyText = await _gameplayUiFactory.CreateFlyText();

      Vector2 screenPoint = _coordinateMapper.WorldToScreenPoint(resourceDropInfo.Position);
      if (_popUpService.ScreenToCanvasPoint(screenPoint, out Vector2 localPoint))
        flyText.SetPosition(localPoint);

      _activeFlyTexts.Add(flyText);
      
      flyText.Deactivated
        .Subscribe(_ => _activeFlyTexts.Remove(flyText))
        .AddTo(_disposable);

      flyText.Setup(resourceDropInfo.Position, resourceDropInfo.Amount, resourceConfig.Icon);
    }
  }
}