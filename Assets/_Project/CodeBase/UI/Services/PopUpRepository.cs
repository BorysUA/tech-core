using System;
using System.Collections.Generic;
using _Project.CodeBase.Gameplay.UI.PopUps.Common;

namespace _Project.CodeBase.UI.Services
{
  public class PopUpRepository
  {
    private readonly Dictionary<Type, BasePopUpViewModel> _popUps = new();

    public bool TryGetValue<TPopUp>(out BasePopUpViewModel popUp) where TPopUp : BasePopUpViewModel
    {
      return _popUps.TryGetValue(typeof(TPopUp), out popUp);
    }

    public void Register<TPopUp>(BasePopUpViewModel popUp) where TPopUp : BasePopUpViewModel
    {
      if (_popUps.ContainsKey(typeof(TPopUp)))
        return;

      _popUps.Add(typeof(TPopUp), popUp);
    }
  }
}