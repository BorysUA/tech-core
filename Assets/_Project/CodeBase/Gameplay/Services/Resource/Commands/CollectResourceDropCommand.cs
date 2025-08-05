using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Gameplay.Services.Resource.Results;

namespace _Project.CodeBase.Gameplay.Services.Resource.Commands
{
  public readonly struct CollectResourceDropCommand : ICommand<CollectResourceDropResult>
  {
    public int Id { get; }

    public CollectResourceDropCommand(int id)
    {
      Id = id;
    }
  }
}