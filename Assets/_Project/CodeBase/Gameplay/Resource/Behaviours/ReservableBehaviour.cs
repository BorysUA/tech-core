using System;
using System.Collections.Generic;
using _Project.CodeBase.Extensions;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Models.Persistent.Interfaces;
using _Project.CodeBase.Gameplay.Models.Session;
using _Project.CodeBase.Services.LogService;
using _Project.CodeBase.Utility;
using R3;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Resource.Behaviours
{
  public class ReservableBehaviour : IReservableResourceBehaviour, IDisposable
  {
    private readonly IResourceBehaviour _innerBehaviour;

    private readonly ReactiveProperty<int> _reservedAmount = new();
    private readonly Dictionary<Guid, ReservationToken> _reservations = new();

    private readonly ILogService _logService;
    private readonly CompositeDisposable _disposable = new();

    public ResourceKind Kind => _innerBehaviour.Kind;
    public ReadOnlyReactiveProperty<int> TotalAmount { get; private set; }
    public ReadOnlyReactiveProperty<int> TotalCapacity => _innerBehaviour.TotalCapacity;
    
    public ReservableBehaviour(IResourceBehaviour innerBehaviour, ILogService logService)
    {
      _innerBehaviour = innerBehaviour;
      _logService = logService;
    }

    public void Setup(IResourceReader resourceProxy, ResourceSessionModel resourceSessionModel)
    {
      _innerBehaviour.Setup(resourceProxy, resourceSessionModel);

      TotalAmount = _innerBehaviour.TotalAmount
        .CombineLatest(_reservedAmount, resourceSessionModel.RuntimeAmount,
          (current, reserved, runtime) => current + runtime - reserved)
        .ToStabilizedReadOnlyReactiveProperty()
        .AddTo(_disposable);
      
      TotalAmount
        .Where(value => value < 0)
        .Subscribe(value => ForcedRelease(Mathf.Abs(value)))
        .AddTo(_disposable);

      resourceSessionModel.RuntimeAmount
        .Where(value => value < 0)
        .Subscribe(value => _logService.LogError(GetType(),
          $"Runtime resource value is below zero: {value}. This indicates a logic error in the reserve/release system."));
    }

    public bool TryReserve(int amount, out ReservationToken token)
    {
      if (TotalAmount.CurrentValue < amount)
      {
        token = null;
        return false;
      }

      token = CreateToken(amount);
      return true;
    }

    public void Release(ReservationToken token)
    {
      if (_reservations.Remove(token.Id, out ReservationToken reservationToken))
        _reservedAmount.Value -= reservationToken.Amount;
    }

    public void Dispose()
    {
      _disposable.Dispose();

      foreach (ReservationToken token in _reservations.Values)
        token.Cancel();

      _reservations.Clear();
    }

    private void ForcedRelease(int amount)
    {
      List<ReservationToken> tokensToRelease = new();

      foreach (ReservationToken token in _reservations.Values)
      {
        if (amount <= 0)
          break;

        amount -= token.Amount;
        tokensToRelease.Add(token);
      }

      foreach (ReservationToken token in tokensToRelease)
        token.Cancel();
    }

    private ReservationToken CreateToken(int amount)
    {
      Guid id = UniqueIdGenerator.GenerateGuid();
      ReservationToken token = new ReservationToken(id, amount, Release);

      _reservedAmount.Value += amount;
      _reservations.Add(token.Id, token);
      return token;
    }
  }
}