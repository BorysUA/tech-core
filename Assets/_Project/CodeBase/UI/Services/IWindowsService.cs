using System.Threading;
using _Project.CodeBase.UI.Core;
using Cysharp.Threading.Tasks;
using R3;

namespace _Project.CodeBase.UI.Services
{
  public interface IWindowsService
  {
    Observable<BaseWindowViewModel> WindowOpened { get; }
    ReadOnlyReactiveProperty<bool> AnyWindowOpen { get; }


    UniTask OpenWindow<TWindow, TViewModel>(bool loadFromCache = true, CancellationToken token = default)
      where TWindow : IWindow
      where TViewModel : BaseWindowViewModel;

    UniTask OpenWindow<TWindow, TViewModel, TData>(TData param, bool loadFromCache = true,
      CancellationToken token = default)
      where TWindow : IWindow
      where TViewModel : BaseWindowViewModel, IParameterizedWindow<TData>;
  }
}