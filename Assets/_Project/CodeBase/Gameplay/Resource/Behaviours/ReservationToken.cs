using System;

namespace _Project.CodeBase.Gameplay.Resource.Behaviours
{
  public class ReservationToken
  {
    private readonly Action<ReservationToken> _cancel;
    public Guid Id { get; }
    public int Amount { get; }
    public bool IsActive { get; private set; } = true;

    public event Action Canceled;

    public ReservationToken(Guid id, int amount, Action<ReservationToken> cancel)
    {
      Id = id;
      Amount = amount;
      _cancel = cancel;
    }

    public void Cancel()
    {
      if (!IsActive)
        return;

      IsActive = false;
      _cancel?.Invoke(this);
      Canceled?.Invoke();
    }
  }
}