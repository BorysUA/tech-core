using System.Threading;
using _Project.CodeBase.UI.Common;
using Cysharp.Threading.Tasks;

namespace _Project.CodeBase.UI.Services
{
  public interface IWindowsFactory
  {
    UniTask<TViewModel> CreateWindow<TWindow, TViewModel>(CancellationToken token, bool useCache = true) where TWindow : IWindow
      where TViewModel : BaseWindowViewModel;
  }
}