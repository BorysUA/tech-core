using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Services.Command;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Services.Resource.Commands
{
  public readonly struct AddResourceDropCommand : ICommand<Unit>
  {
    public ResourceDropType ResourceDropType { get; }
    public Vector3 SpawnPoint { get; }
    public Vector3 Position { get; }

    public AddResourceDropCommand(ResourceDropType resourceDropType, Vector3 spawnPoint, Vector3 position)
    {
      ResourceDropType = resourceDropType;
      Position = position;
      SpawnPoint = spawnPoint;
    }
  }
}