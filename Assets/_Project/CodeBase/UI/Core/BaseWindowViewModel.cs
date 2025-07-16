using R3;

namespace _Project.CodeBase.UI.Core
{
  public abstract class BaseWindowViewModel
  {
    private readonly Subject<Unit> _windowOpened = new();
    private readonly Subject<Unit> _windowClosed = new();

    public Observable<Unit> WindowOpened => _windowOpened;
    public Observable<Unit> WindowClosed => _windowClosed;

    public virtual void Initialize()
    {
    }

    public virtual void Reset()
    {
    }

    public virtual void Open()
    {
      _windowOpened.OnNext(Unit.Default);
    }

    public virtual void Close()
    {
      _windowClosed.OnNext(Unit.Default);
    }
  }
}