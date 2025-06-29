using System;
using System.Threading.Tasks;
using _Project.CodeBase.Gameplay.LiveEvents;
using _Project.CodeBase.Gameplay.UI.Factory;
using R3;
using UnityEngine;
using Zenject;

namespace _Project.CodeBase.Gameplay.UI.HUD.GameEvent
{
  public class GameEventsView : MonoBehaviour
  {
    [SerializeField] private Transform _container;

    private GameEventViewModel _viewModel;
    private GameEventIndicator _currentGameEvent;
    private IGameplayUiFactory _uiFactory;

    [Inject]
    public void Construct(GameEventViewModel viewModel, IGameplayUiFactory uiFactory)
    {
      _viewModel = viewModel;
      _uiFactory = uiFactory;

      _viewModel.Initialize();

      _viewModel.FocusedEvent
        .Select(gameEvent => Observable.FromAsync(_ => ShowGameEvent(gameEvent)))
        .Concat()
        .Subscribe()
        .AddTo(this);
    }

    public void OnDestroy() =>
      _viewModel.Dispose();

    private async ValueTask ShowGameEvent(GameEventViewData gameEvent)
    {
      _currentGameEvent?.Destroy();

      if (gameEvent.Type == GameEventType.None)
        return;

      _currentGameEvent = await _uiFactory.CreateGameEventIndicator(gameEvent.Type, _container);
      _currentGameEvent.Initialize(gameEvent.IsActive, gameEvent.Countdown);
    }
  }
}