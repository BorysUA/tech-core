using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Services.Resource;
using R3;

namespace _Project.CodeBase.Gameplay.UI.HUD.ResourceBar
{
  public class ResourceBarViewModel
  {
    public readonly ReadOnlyReactiveProperty<string> Metal;
    public readonly ReadOnlyReactiveProperty<string> Energy;
    public readonly ReadOnlyReactiveProperty<string> Population;
    public readonly ReadOnlyReactiveProperty<string> Coin;

    public ResourceBarViewModel(IResourceService resourceService)
    {
      Metal = resourceService
        .ObserveResource(ResourceKind.Metal)
        .Select(value => value.ToString())
        .ToReadOnlyReactiveProperty();

      Energy = resourceService
        .ObserveResource(ResourceKind.Energy)
        .Select(value => value.ToString())
        .ToReadOnlyReactiveProperty();

      Population = resourceService
        .ObserveResource(ResourceKind.Population)
        .Select(value => value.ToString())
        .ToReadOnlyReactiveProperty();
      
      Coin = resourceService
        .ObserveResource(ResourceKind.Coin)
        .Select(value => value.ToString())
        .ToReadOnlyReactiveProperty();
    }
  }
}