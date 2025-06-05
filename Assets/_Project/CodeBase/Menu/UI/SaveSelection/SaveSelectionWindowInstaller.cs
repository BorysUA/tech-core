using Zenject;

namespace _Project.CodeBase.Menu.UI.SaveBrowser
{
  public class SaveSelectionWindowInstaller : MonoInstaller
  {
    public override void InstallBindings()
    {
      Container.Bind<SaveSelectionViewModel>().FromResolve();
    }
  }
}