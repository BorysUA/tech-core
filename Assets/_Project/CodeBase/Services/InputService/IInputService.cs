
using R3;
using UnityEngine;

namespace _Project.CodeBase.Services.InputService
{
  public interface IInputService
  {
    void Initialize();

    void Subscribe(PlayerInputHandler playerInputHandler);
    void Unsubscribe(PlayerInputHandler playerInputHandler);
    void SubscribeWithUiFilter(PlayerInputHandler playerInputHandler);
  }
}