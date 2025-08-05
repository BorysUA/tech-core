using _Project.CodeBase.Gameplay.Services.Command;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Services.Resource.Commands
{
  public readonly struct UpdateDropSpawnPointCommand : ICommand<bool>
  {
    public int Id { get; }
    public Vector3 Position { get; }

    public UpdateDropSpawnPointCommand(int id, Vector3 position)
    {
      Id = id;
      Position = position;
    }
  }
}