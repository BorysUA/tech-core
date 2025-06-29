using _Project.CodeBase.UI.Core;
using Cysharp.Threading.Tasks;

namespace _Project.CodeBase.Gameplay.UI.Factory
{
  public interface IPopUpFactory
  {
    UniTask<TViewModel> CreatePopUp<TPopUp, TViewModel>(bool isFromCache = true)
      where TPopUp : BasePopUp<TViewModel> where TViewModel : BasePopUpViewModel;
  }
}