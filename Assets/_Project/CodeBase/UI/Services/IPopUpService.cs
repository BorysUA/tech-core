using _Project.CodeBase.Gameplay.UI.PopUps.Common;
using _Project.CodeBase.UI.Common;
using UnityEngine;

namespace _Project.CodeBase.UI.Services
{
  public interface IPopUpService
  {
    bool ScreenToCanvasPoint(Vector2 screenPoint, out Vector2 localPoint);

    void ShowPopUp<TPopUpView, TPopUpViewModel>(bool loadFromCache = true) where TPopUpView : BasePopUp<TPopUpViewModel>
      where TPopUpViewModel : BasePopUpViewModel;

    void ShowPopUp<TPopUpView, TPopUpViewModel, TData>(TData data, bool loadFromCache = true)
      where TPopUpView : BasePopUp<TPopUpViewModel>
      where TPopUpViewModel : BasePopUpViewModel, IParameterizedPopUp<TData>;
  }
}