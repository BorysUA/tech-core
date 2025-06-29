using R3;

namespace _Project.CodeBase.UI.Core
{
  public class BasePopUpViewModel
  {
    private readonly Subject<Unit> _popUpShowed = new();
    private readonly Subject<Unit> _popUpHidden = new();

    public Observable<Unit> PopUpShowed => _popUpShowed;
    public Observable<Unit> PopUpHidden => _popUpHidden;

    public virtual void Show()
    {
      _popUpShowed.OnNext(Unit.Default);
    }

    public virtual void Hide()
    {
      _popUpHidden.OnNext(Unit.Default);
    }
  }
}