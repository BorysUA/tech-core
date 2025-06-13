using System;
using System.Collections;
using System.Collections.Generic;
using _Project.CodeBase.Gameplay.Services;
using _Project.CodeBase.Gameplay.States;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Infrastructure.StateMachine;
using _Project.CodeBase.Services.LogService;
using R3;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace _Project.CodeBase.Services.InputService
{
  public class InputService : IInputService, IDisposable, IGameplayInit
  {
    private readonly InputSystemActions _inputActions;
    private readonly ICoroutineRunner _coroutineRunner;
    private readonly ILogService _logService;
    private readonly IDisposable _disposable;

    private readonly Dictionary<Type, PlayerInputHandler> _playerInputHandlers = new();
    private readonly List<InputHandler> _inputHandlers = new();

    public InputService(InputSystemActions inputActions, ICoroutineRunner coroutineRunner, ILogService logService,
      TapDetector tapDetector)
    {
      _inputActions = inputActions;
      _coroutineRunner = coroutineRunner;
      _logService = logService;

      _disposable = tapDetector.OnTapDetected.Subscribe(FireTap);

      _inputHandlers.Add(tapDetector);
    }

    public void Initialize()
    {
      _inputActions.Enable();

      _inputActions.Player.TouchPoint.started += BroadcastTouchStarted;
      _inputActions.Player.TouchPoint.performed += BroadcastTouchPerformed;
      _inputActions.Player.TouchPoint.canceled += BroadcastTouchCanceled;
    }

    public void Subscribe(PlayerInputHandler playerInputHandler)
    {
      if (IsInputHandlerDuplicated(playerInputHandler))
        return;

      _playerInputHandlers.Add(playerInputHandler.GetType(), playerInputHandler);
      playerInputHandler.OnActivated();
    }

    public void SubscribeWithUiFilter(PlayerInputHandler playerInputHandler)
    {
      if (IsInputHandlerDuplicated(playerInputHandler))
        return;

      InputHandlerWithUiFilter inputHandlerWithUiFilter =
        new InputHandlerWithUiFilter(playerInputHandler, _coroutineRunner);

      _playerInputHandlers.Add(playerInputHandler.GetType(), inputHandlerWithUiFilter);
      playerInputHandler.OnActivated();
    }

    public void Unsubscribe(PlayerInputHandler playerInputHandler)
    {
      if (!_playerInputHandlers.ContainsKey(playerInputHandler.GetType()))
      {
        _logService.LogWarning(GetType(), $"PlayerInputHandler not found in '{nameof(_playerInputHandlers)}'.");
        return;
      }

      _playerInputHandlers.Remove(playerInputHandler.GetType());
      playerInputHandler.OnDeactivated();
    }

    public void Dispose()
    {
      _inputActions.Player.TouchPoint.started -= BroadcastTouchStarted;
      _inputActions.Player.TouchPoint.performed -= BroadcastTouchPerformed;
      _inputActions.Player.TouchPoint.canceled -= BroadcastTouchCanceled;
      _disposable?.Dispose();
    }

    private void BroadcastTouchStarted(InputAction.CallbackContext context)
    {
      Vector2 inputPoint = context.ReadValue<Vector2>();
      FireTouchStarted(inputPoint);
    }

    private void BroadcastTouchPerformed(InputAction.CallbackContext context)
    {
      Vector2 inputPoint = context.ReadValue<Vector2>();
      FireTouchPerformed(inputPoint);
    }

    private void BroadcastTouchCanceled(InputAction.CallbackContext context)
    {
      FireTouchCanceled();
    }

    private void FireTouchStarted(Vector2 inputPoint)
    {
      foreach (var entry in _playerInputHandlers)
        entry.Value.OnTouchStarted(inputPoint);

      foreach (InputHandler inputHandler in _inputHandlers)
        inputHandler.OnTouchStarted(inputPoint);
    }

    private void FireTouchPerformed(Vector2 inputPoint)
    {
      foreach (var entry in _playerInputHandlers)
        entry.Value.OnTouchMoved(inputPoint);

      foreach (InputHandler inputHandler in _inputHandlers)
        inputHandler.OnTouchMoved(inputPoint);
    }

    private void FireTouchCanceled()
    {
      foreach (var entry in _playerInputHandlers)
        entry.Value.OnTouchEnded();

      foreach (InputHandler inputHandler in _inputHandlers)
        inputHandler.OnTouchEnded();
    }

    private void FireTap(Vector2 inputPoint)
    {
      foreach (var entry in _playerInputHandlers)
        entry.Value.OnTap(inputPoint);
    }

    private bool IsInputHandlerDuplicated(PlayerInputHandler playerInputHandler)
    {
      if (_playerInputHandlers.ContainsKey(playerInputHandler.GetType()))
      {
        _logService.LogWarning(GetType(),
          $"Duplicate playerInputHandler detected: '{playerInputHandler.GetType().Name}' is already registered in '{nameof(_playerInputHandlers)}'.");
        return true;
      }

      return false;
    }
  }
}