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
    private readonly ICoroutineRunner _coroutineRunner;
    private readonly WaitForEndOfFrame _waitForEndOfFrame = new();

    public InputHandlerWithUiFilter(PlayerInputHandler inputHandler, ICoroutineRunner coroutineRunner)
    {
      _inputHandler = inputHandler;
      _coroutineRunner = coroutineRunner;
    }

    public override void OnTap(Vector2 inputPosition) =>
      _coroutineRunner.ExecuteCoroutine(InvokeIfNotOverUIOnNextFrame(() => _inputHandler.OnTap(inputPosition)));

    public override void OnTouchStarted(Vector2 inputPoint) =>
      _coroutineRunner.ExecuteCoroutine(InvokeIfNotOverUIOnNextFrame(() => _inputHandler.OnTouchStarted(inputPoint)));

    public override void OnTouchMoved(Vector2 inputPoint) =>
      _coroutineRunner.ExecuteCoroutine(InvokeIfNotOverUIOnNextFrame(() => _inputHandler.OnTouchMoved(inputPoint)));

    public override void OnTouchEnded() =>
      _coroutineRunner.ExecuteCoroutine(InvokeIfNotOverUIOnNextFrame(() => _inputHandler.OnTouchEnded()));

    private IEnumerator InvokeIfNotOverUIOnNextFrame(Action handler)
    {
      yield return _waitForEndOfFrame;

      if (!EventSystem.current.IsPointerOverGameObject())
        handler.Invoke();
    }
  }
}