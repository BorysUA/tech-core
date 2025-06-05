using _Project.CodeBase.Gameplay.Services.Command;

namespace _Project.CodeBase.Gameplay.Services.Resource.Commands
{
  public readonly struct CollectResourceDropCommand : ICommand
  {
    public string Id { get; }

    public CollectResourceDropCommand(string id)
    {
      Id = id;
    }
  }
}