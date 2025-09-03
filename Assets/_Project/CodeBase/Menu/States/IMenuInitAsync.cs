using Cysharp.Threading.Tasks;

namespace _Project.CodeBase.Menu.States
{
  public interface IMenuInitAsync
  {
    public UniTask InitializeAsync();
  }
}