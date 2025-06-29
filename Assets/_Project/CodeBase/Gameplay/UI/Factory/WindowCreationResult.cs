using _Project.CodeBase.UI.Core;

namespace _Project.CodeBase.Gameplay.UI.Factory
{
  public class WindowCreationResult<TViewModel> : IWindowCreationResult<TViewModel>
    where TViewModel : BaseWindowViewModel
  {
    public TViewModel Instance { get; }
    public bool FromCache { get; }

    public WindowCreationResult(TViewModel instance, bool fromCache)
    {
      Instance = instance;
      FromCache = fromCache;
    }

    public void Deconstruct(out TViewModel instance, out bool fromCache)
    {
      instance = Instance;
      fromCache = FromCache;
    }
  }

  public interface IWindowCreationResult<out TViewModel> where TViewModel : BaseWindowViewModel
  {
    public TViewModel Instance { get; }
    public bool FromCache { get; }
  }
}