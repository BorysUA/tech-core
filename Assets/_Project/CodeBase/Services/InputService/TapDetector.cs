using System;
using R3;
using UnityEngine;

namespace _Project.CodeBase.Services.InputService
{
  public class TapDetector : InputHandler
  {
    private readonly Subject<Vector2> _onTapDetected = new();

    private const float TapDurationThreshold = 1f;
    private const float TapDistanceThreshold = 0.02f;

    private Vector2 _startPosition;
    private Vector2 _lastPosition;
    private float _startTime;
    private bool _isTapCandidate;

    public Observable<Vector2> OnTapDetected => _onTapDetected;

    public override void OnTouchStarted(Vector2 position)
    {
      _startPosition = position;
      _startTime = Time.time;
      _isTapCandidate = true;
    }

    public override void OnTouchMoved(Vector2 position)
    {
      _lastPosition = position;

      float displacement = CalculateNormalizedDistance(_startPosition, position);

      if (displacement > TapDistanceThreshold)
        _isTapCandidate = false;
    }

    public override void OnTouchEnded()
    {
      float duration = Time.time - _startTime;

      if (_isTapCandidate && duration <= TapDurationThreshold)
        _onTapDetected.OnNext(_lastPosition);

      _isTapCandidate = false;
    }

    private float CalculateNormalizedDistance(Vector2 start, Vector2 current)
    {
      Vector2 normalizedStart = start / new Vector2(Screen.width, Screen.height);
      Vector2 normalizedCurrent = current / new Vector2(Screen.width, Screen.height);
      return Vector2.Distance(normalizedStart, normalizedCurrent);
    }
  }
}