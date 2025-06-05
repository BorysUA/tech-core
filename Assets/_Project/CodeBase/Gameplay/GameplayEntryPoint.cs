using _Project.CodeBase.Gameplay.Services.BuildingPlots;
using _Project.CodeBase.Gameplay.Services.Buildings;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Gameplay.Services.Resource;
using _Project.CodeBase.Gameplay.Services.Resource.Handlers;
using _Project.CodeBase.Gameplay.States.GameStates;
using _Project.CodeBase.Infrastructure.StateMachine;
using Zenject;

namespace _Project.CodeBase.Gameplay
{
  public class GameplayEntryPoint : IInitializable
  {
    private readonly GameStatesFactory _gameStatesFactory;
    private readonly GameStateMachine _stateMachine;
    private readonly ICommandBroker _commandBroker;
    private readonly CommandFactory _commandFactory;

    public GameplayEntryPoint(GameStatesFactory gameStatesFactory, GameStateMachine stateMachine,
      ICommandBroker commandBroker, CommandFactory commandFactory)
    {
      _gameStatesFactory = gameStatesFactory;
      _stateMachine = stateMachine;
      _commandBroker = commandBroker;
      _commandFactory = commandFactory;
    }

    public void Initialize()
    {
      _stateMachine.RegisterSceneState(_gameStatesFactory.CreateState<LoadGameplayState>());
      _stateMachine.RegisterSceneState(_gameStatesFactory.CreateState<GameplayState>());

      _commandBroker.Register(_commandFactory.CreateHandler<PlaceBuildingHandler>());
      _commandBroker.Register(_commandFactory.CreateHandler<AddResourceHandler>());
      _commandBroker.Register(_commandFactory.CreateHandler<AddResourceDropHandler>());
      _commandBroker.Register(_commandFactory.CreateHandler<CollectResourceDropHandler>());
      _commandBroker.Register(_commandFactory.CreateHandler<SpendResourceHandler>());
      _commandBroker.Register(_commandFactory.CreateHandler<PlaceConstructionPlotHandler>());
      _commandBroker.Register(_commandFactory.CreateHandler<SpendResourcesHandler>());
      _commandBroker.Register(_commandFactory.CreateHandler<DestroyBuildingHandler>());
      _commandBroker.Register(_commandFactory.CreateHandler<IncreaseResourceCapacityHandler>());
      _commandBroker.Register(_commandFactory.CreateHandler<DecreaseResourceCapacityHandler>());

      _stateMachine.Enter<LoadGameplayState>();
    }
  }
}