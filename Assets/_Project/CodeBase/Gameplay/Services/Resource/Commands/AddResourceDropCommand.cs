using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Services.Command;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Services.Resource.Commands
{
  public readonly struct AddResourceDropCommand : ICommand
  {
    public ResourceKind ResourceKind { get; }
    public ResourceDropType ResourceDropType { get; }
    public Vector3 SpawnPoint { get; }
    public Vector3 Position { get; }
    public int Amount { get; }

    public AddResourceDropCommand(ResourceDropType resourceDropType, ResourceKind resourceKind, Vector3 spawnPoint,
      Vector3 position, int amount)
    {
      ResourceDropType = resourceDropType;
      ResourceKind = resourceKind;
      Position = position;
      Amount = amount;
      SpawnPoint = spawnPoint;
    }
  }
}