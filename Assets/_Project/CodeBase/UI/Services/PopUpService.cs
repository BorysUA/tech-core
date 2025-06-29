using _Project.CodeBase.Gameplay.UI.Factory;
using _Project.CodeBase.Gameplay.UI.Root;
using _Project.CodeBase.UI.Core;
using UnityEngine;

namespace _Project.CodeBase.UI.Services
{
  public class PopUpService : IPopUpService
  {
    private readonly PopUpsCanvas _canvas;
    private readonly IPopUpFactory _popUpFactory;

    public PopUpService(PopUpsCanvas canvas, IPopUpFactory popUpFactory)
    {
      _canvas = canvas;
      _popUpFactory = popUpFactory;
    }

    public async void ShowPopUp<TPopUpView, TPopUpViewModel>(bool loadFromCache )
      where TPopUpView : BasePopUp<TPopUpViewModel>
      where TPopUpViewModel : BasePopUpViewModel
    {
      TPopUpViewModel popUp = await _popUpFactory.CreatePopUp<TPopUpView, TPopUpViewModel>(loadFromCache);
      popUp.Show();
    }

    public async void ShowPopUp<TPopUpView, TPopUpViewModel, TData>(TData data, bool loadFromCache)
      where TPopUpView : BasePopUp<TPopUpViewModel>
      where TPopUpViewModel : BasePopUpViewModel, IParameterizedPopUp<TData>
    {
      TPopUpViewModel popUp = await _popUpFactory.CreatePopUp<TPopUpView, TPopUpViewModel>(loadFromCache);
      popUp.Initialize(data);
      popUp.Show();
    }

    public bool ScreenToCanvasPoint(Vector2 screenPoint, out Vector2 localPoint)
    {
      return RectTransformUtility.ScreenPointToLocalPointInRectangle(
        _canvas.RectTransform, screenPoint, null, out localPoint);
    }
  }
}