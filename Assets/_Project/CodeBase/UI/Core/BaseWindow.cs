using R3;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.CodeBase.UI.Common
{
  public abstract class BaseWindow<TViewModel> : MonoBehaviour, IWindow where TViewModel : BaseWindowViewModel
  {
    protected TViewModel ViewModel;

    public virtual void Setup(BaseWindowViewModel viewModel)
    {
      ViewModel = viewModel as TViewModel;

      ViewModel.WindowOpened
        .Subscribe(_ => Open())
        .AddTo(this);

      ViewModel.WindowClosed
        .Subscribe(_ => Close())
        .AddTo(this);
    }

    protected virtual void Open() =>
      gameObject.SetActive(true);

    protected virtual void Close() =>
      gameObject.SetActive(false);

    protected void BindCloseActions(params Observable<Unit>[] closeActions)
    {
      Observable.Merge(closeActions)
        .Subscribe(_ => ViewModel.Close())
        .AddTo(this);
    }
  }
}