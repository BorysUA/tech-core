using Cysharp.Threading.Tasks;

namespace _Project.CodeBase.Gameplay.States
{
  public interface IGameplayInitAsync
  {
    public UniTask InitializeAsync();
  }
}