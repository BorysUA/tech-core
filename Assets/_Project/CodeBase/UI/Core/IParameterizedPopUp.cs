using System;

namespace _Project.CodeBase.UI.Core
{
  public interface IParameterizedPopUp<in TData>
  {
    public event Action Initialized;
    public void Initialize(TData parameter);
  }
}