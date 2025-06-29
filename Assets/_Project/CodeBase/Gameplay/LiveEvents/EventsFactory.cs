using System;
using Zenject;

namespace _Project.CodeBase.Gameplay.LiveEvents
{
  public class EventsFactory
  {
    private readonly IInstantiator _instantiator;

    public EventsFactory(IInstantiator instantiator) =>
      _instantiator = instantiator;

    public GameEventBase CreateGameEvent(Type type) =>
      (GameEventBase)_instantiator.Instantiate(type);
  }
}