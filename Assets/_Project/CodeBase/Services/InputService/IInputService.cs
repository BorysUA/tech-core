namespace _Project.CodeBase.Services.InputService
{
  public interface IInputService
  {
    void Subscribe(PlayerInputHandler playerInputHandler);
    void Unsubscribe(PlayerInputHandler playerInputHandler);
    void SubscribeWithUiFilter(PlayerInputHandler playerInputHandler);
  }
}