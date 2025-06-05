using R3;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.InputHandlers
{
  public interface ICameraMovement
  {
    ReadOnlyReactiveProperty<Vector3> MovementDelta { get; }
  }
}