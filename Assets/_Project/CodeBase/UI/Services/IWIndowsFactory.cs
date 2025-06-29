using System.Threading;
using _Project.CodeBase.Gameplay.UI.Factory;
using _Project.CodeBase.UI.Core;
using Cysharp.Threading.Tasks;

namespace _Project.CodeBase.UI.Services
{
  public interface IWindowsFactory
  {
    UniTask<WindowCreationResult<TViewModel>> CreateWindow<TWindow, TViewModel>(CancellationToken token,
      bool useCache = true)
      where TWindow : IWindow
      where TViewModel : BaseWindowViewModel;
  }
}