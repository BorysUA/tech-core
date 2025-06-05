using System.Collections.Generic;
using _Project.CodeBase.Data.Progress.ResourceData;
using _Project.CodeBase.Gameplay.Services.Command;

namespace _Project.CodeBase.Gameplay.Services.Resource.Commands
{
  public readonly struct SpendResourcesCommand : ICommand
  {
    public IEnumerable<ResourceAmountData> Resources { get; }

    public SpendResourcesCommand(IEnumerable<ResourceAmountData> resources)
    {
      Resources = resources;
    }
  }
}