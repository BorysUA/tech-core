namespace _Project.CodeBase.UI.Core
{
  public interface IParameterizedWindow<in TParam>
  {
    public bool Matches(TParam param);
    public void Initialize(TParam parameter);
    public void Reset();
  }
}