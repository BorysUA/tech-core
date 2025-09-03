using System;
using System.Collections;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Project.CodeBase.Services.InputService
{
  public class InputHandlerWithUiFilter : PlayerInputHandler
  {
    private readonly PlayerInputHandler _inputHandler;
    private readonly ICoroutineProvider _coroutineProvider;
    private readonly WaitForEndOfFrame _waitForEndOfFrame = new();

    private bool _isTouchValid;

    public InputHandlerWithUiFilter(PlayerInputHandler inputHandler, ICoroutineProvider coroutineProvider)
    {
      _inputHandler = inputHandler;
      _coroutineProvider = coroutineProvider;
    }

    public override void OnTouchStarted(Vector2 inputPoint)
    {
      _coroutineProvider.ExecuteCoroutine(InvokeIfNotOverUIOnNextFrame(() =>
      {
        _isTouchValid = true;
        _inputHandler.OnTouchStarted(inputPoint);
      }));
    }

    public override void OnTap(Vector2 inputPoint)
    {
      _coroutineProvider.ExecuteCoroutine(InvokeIfNotOverUIOnNextFrame(() => _inputHandler.OnTap(inputPoint)));
    }

    public override void OnTouchMoved(Vector2 inputPoint)
    {
      if (_isTouchValid)
        _inputHandler.OnTouchMoved(inputPoint);
    }

    public override void OnTouchEnded()
    {
      if (_isTouchValid)
        _inputHandler.OnTouchEnded();

      _isTouchValid = false;
    }

    private IEnumerator InvokeIfNotOverUIOnNextFrame(Action handler)
    {
      yield return _waitForEndOfFrame;

      if (!EventSystem.current.IsPointerOverGameObject())
        handler.Invoke();
    }
  }
}