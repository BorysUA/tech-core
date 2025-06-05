using Zenject;

namespace _Project.CodeBase.Gameplay.Services.Command
{
  public class CommandFactory
  {
    private readonly IInstantiator _instantiator;

    public CommandFactory(IInstantiator instantiator) =>
      _instantiator = instantiator;

    public THandler CreateHandler<THandler>() => 
      _instantiator.Instantiate<THandler>();
  }
}