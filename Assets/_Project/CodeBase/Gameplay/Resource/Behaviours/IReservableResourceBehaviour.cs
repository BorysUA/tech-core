namespace _Project.CodeBase.Gameplay.Resource.Behaviours
{
  public interface IReservableResourceBehaviour : IResourceBehaviour
  {
    bool TryReserve(int amount, out ReservationToken token);
    void Release(ReservationToken token);
  }
}