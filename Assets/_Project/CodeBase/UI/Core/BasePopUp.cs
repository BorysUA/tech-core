using R3;
using UnityEngine;

namespace _Project.CodeBase.UI.Core
{
  public class BasePopUp<TViewModel> : MonoBehaviour where TViewModel : BasePopUpViewModel
  {
    protected TViewModel ViewModel;

    public virtual void Setup(TViewModel viewModel)
    {
      ViewModel = viewModel;

      ViewModel.PopUpShowed
        .Subscribe(_ => Show())
        .AddTo(this);

      ViewModel.PopUpHidden
        .Subscribe(_ => Hide())
        .AddTo(this);
    }

    protected virtual void Show() =>
      gameObject.SetActive(true);

    protected virtual void Hide() =>
      gameObject.SetActive(false);
  }
}