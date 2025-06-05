using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Data;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Gameplay.Services.Resource.Commands;
using _Project.CodeBase.Gameplay.Services.Resource.Results;
using R3;

namespace _Project.CodeBase.Gameplay.Resource.Behaviours
{
  public class ConsumableBehaviour : IResourceBehaviour
  {
    private ResourceProxy _resourceProxy;

    private readonly ResourceKind _kind;
    private readonly ICommandBroker _commandBroker;

    public ReadOnlyReactiveProperty<int> AvailableAmount => _resourceProxy.Amount;

    public ConsumableBehaviour(ResourceKind kind, ICommandBroker commandBroker)
    {
      _kind = kind;
      _commandBroker = commandBroker;
    }

    public void Setup(ResourceProxy resourceProxy) =>
      _resourceProxy = resourceProxy;

    public bool CanSpend(int toSpend) =>
      AvailableAmount.CurrentValue - toSpend > 0;

    public void Add(int amount)
    {
      AddResourceCommand command = new AddResourceCommand(_kind, amount);
      _commandBroker.ExecuteCommand<AddResourceCommand, AddResourceResult>(command);
    }

    public bool TrySpend(int amount)
    {
      SpendResourceCommand command = new SpendResourceCommand(_kind, amount);
      bool result = _commandBroker.ExecuteCommand<SpendResourceCommand, bool>(command);
      return result;
    }

    public bool IncreaseCapacity(int amount)
    {
      IncreaseResourceCapacityCommand command = new IncreaseResourceCapacityCommand(_kind, amount);
      return _commandBroker.ExecuteCommand<IncreaseResourceCapacityCommand, bool>(command);
    }

    public bool DecreaseCapacity(int amount)
    {
      DecreaseResourceCapacityCommand command = new DecreaseResourceCapacityCommand(_kind, amount);
      return _commandBroker.ExecuteCommand<DecreaseResourceCapacityCommand, bool>(command);
    }
  }
}