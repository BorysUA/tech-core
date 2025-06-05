using System.Collections.Generic;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Resource.Behaviours;
using Zenject;

namespace _Project.CodeBase.Gameplay.Services.Resource
{
  public class ResourceBehaviourMap
  {
    public Dictionary<ResourceKind, IResourceBehaviour> Map { get; }

    public ResourceBehaviourMap(
      [Inject(Id = ResourceKind.Metal)] IResourceBehaviour metal,
      [Inject(Id = ResourceKind.Energy)] IResourceBehaviour energy,
      [Inject(Id = ResourceKind.Population)] IResourceBehaviour population,
      [Inject(Id = ResourceKind.Coin)] IResourceBehaviour coin)
    {
      Map = new Dictionary<ResourceKind, IResourceBehaviour>
      {
        [ResourceKind.Metal] = metal,
        [ResourceKind.Energy] = energy,
        [ResourceKind.Population] = population,
        [ResourceKind.Coin] = coin
      };
    }
  }
}