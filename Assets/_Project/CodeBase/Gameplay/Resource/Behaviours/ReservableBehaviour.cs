using System;
using System.Collections.Generic;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Data;
using _Project.CodeBase.Utility;
using R3;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Resource.Behaviours
{
  public class ReservableBehaviour : IReservableResourceBehaviour, IDisposable
  {
    private readonly IResourceBehaviour _innerBehaviour;
    private readonly ResourceKind _kind;
    private readonly ReactiveProperty<int> _reservedAmount = new();
    private readonly Dictionary<Guid, ReservationToken> _reservations = new();
    private readonly CompositeDisposable _disposable = new();

    public ReadOnlyReactiveProperty<int> AvailableAmount { get; private set; }

    public ReservableBehaviour(IResourceBehaviour innerBehaviour) =>
      _innerBehaviour = innerBehaviour;

    public void Setup(ResourceProxy resourceProxy)
    {
      _innerBehaviour.Setup(resourceProxy);

      AvailableAmount = _innerBehaviour.AvailableAmount
        .CombineLatest(_reservedAmount, (current, reserved) => current - reserved)
        .ToReadOnlyReactiveProperty()
        .AddTo(_disposable);

      AvailableAmount
        .Where(value => value < 0)
        .Subscribe(_ => ForcedRelease(Mathf.Abs(AvailableAmount.CurrentValue)))
        .AddTo(_disposable);
    }

    public void Add(int amount) =>
      _innerBehaviour.Add(amount);

    public bool CanSpend(int amount) =>
      _innerBehaviour.CanSpend(amount);

    public bool TrySpend(int amount) =>
      _innerBehaviour.TrySpend(amount);

    public bool IncreaseCapacity(int amount) =>
      _innerBehaviour.IncreaseCapacity(amount);

    public bool DecreaseCapacity(int amount) =>
      _innerBehaviour.DecreaseCapacity(amount);

    public bool TryReserve(int amount, out ReservationToken token)
    {
      if (AvailableAmount.CurrentValue < amount)
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