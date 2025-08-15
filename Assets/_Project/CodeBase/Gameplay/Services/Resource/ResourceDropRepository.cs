using System;
using System.Collections.Generic;
using _Project.CodeBase.Gameplay.Models.Persistent.Interfaces;
using _Project.CodeBase.Gameplay.Resource;
using _Project.CodeBase.Gameplay.Services.Factories;
using _Project.CodeBase.Gameplay.States;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Infrastructure.Services.ProgressProvider;
using _Project.CodeBase.Services.LogService;
using Cysharp.Threading.Tasks;
using R3;

namespace _Project.CodeBase.Gameplay.Services.Resource
{
  public class ResourceDropRepository : IGameplayInit, IDisposable
  {
    private readonly ILogService _logService;
    private readonly IGameplayFactory _gameplayFactory;
    private readonly Dictionary<int, ResourceDropViewModel> _resourceDrops = new();
    private readonly IProgressReader _progressReader;
    private readonly CompositeDisposable _compositeDisposable = new();

    public ResourceDropRepository(ILogService logService, IGameplayFactory gameplayFactory,
      IProgressReader progressReader)
    {
      _logService = logService;
      _gameplayFactory = gameplayFactory;
      _progressReader = progressReader;
    }

    public void Initialize()
    {
      foreach (IResourceDropReader resourceEntry in _progressReader.GameStateModel.ReadOnlyResourceDrops.Values)
        CreateResourceDropView(resourceEntry).Forget();

      _progressReader.GameStateModel.ReadOnlyResourceDrops.ObserveAdd()
        .Subscribe(entry => CreateResourceDropView(entry.Value).Forget())
        .AddTo(_compositeDisposable);

      _progressReader.GameStateModel.ReadOnlyResourceDrops.ObserveRemove()
        .Subscribe(entry => DestroyResourceDropView(entry.Value))
        .AddTo(_compositeDisposable);
    }

    public void Dispose()
    {
      _compositeDisposable?.Dispose();
    }

    private async UniTaskVoid CreateResourceDropView(IResourceDropReader resourceDropProxy)
    {
      ResourceDropViewModel viewModel =
        await _gameplayFactory.CreateResourceDrop(resourceDropProxy.ResourceDropType,
          resourceDropProxy.SpawnPoint.CurrentValue);

      viewModel.Setup(resourceDropProxy);

      _resourceDrops.Add(resourceDropProxy.Id, viewModel);
    }

    private void DestroyResourceDropView(IResourceDropReader resourceDropProxy)
    {
      if (_resourceDrops.Remove(resourceDropProxy.Id, out ResourceDropViewModel viewModel))
        viewModel.Deactivate();
      else
        _logService.LogError(GetType(), $"Key '{resourceDropProxy.Id}' not found in '{nameof(_resourceDrops)}'.");
    }
  }
}