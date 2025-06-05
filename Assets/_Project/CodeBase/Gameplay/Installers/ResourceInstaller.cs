using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Resource.Behaviours;
using Zenject;

namespace _Project.CodeBase.Gameplay.Installers
{
  public class ResourceInstaller : MonoInstaller
  {
    private const string InnerSuffix = "_Inner";

    public override void InstallBindings()
    {
      BindSimple(ResourceKind.Metal);
      BindSimple(ResourceKind.Energy);
      BindSimple(ResourceKind.Coin);

      BindReservable(ResourceKind.Population);
    }

    private void BindSimple(ResourceKind kind)
    {
      Container.Bind<IResourceBehaviour>()
        .WithId(kind)
        .To<ConsumableBehaviour>()
        .AsTransient()
        .WithArguments(kind);
    }

    private void BindReservable(ResourceKind kind)
    {
      Container.Bind<IResourceBehaviour>()
        .WithId($"{kind}{InnerSuffix}")
        .To<ConsumableBehaviour>()
        .AsTransient()
        .WithArguments(kind);

      Container.Bind<IResourceBehaviour>()
        .WithId(kind)
        .FromMethod(context =>
        {
          var innerBehaviour = context.Container.ResolveId<IResourceBehaviour>($"{kind}{InnerSuffix}");
          return context.Container.Instantiate<ReservableBehaviour>(new object[] { innerBehaviour });
        })
        .AsTransient();
    }
  }
}