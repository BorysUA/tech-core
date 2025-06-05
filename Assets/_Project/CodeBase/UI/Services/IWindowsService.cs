using System.Threading;
using _Project.CodeBase.UI.Common;
using Cysharp.Threading.Tasks;
using R3;

namespace _Project.CodeBase.UI.Services
{
  public interface IWindowsService
  {
    Observable<BaseWindowViewModel> WindowOpened { get; }


    UniTask OpenWindow<TWindow, TViewModel>(bool loadFromCache = true, CancellationToken token = default)
      where TWindow : IWindow
      where TViewModel : BaseWindowViewModel;

    UniTask OpenWindow<TWindow, TViewModel, TData>(TData data, bool loadFromCache = true,
      CancellationToken token = default)
      where TWindow : IWindow
      where TViewModel : BaseWindowViewModel, IParameterizedWindow<TData>;
  }
}