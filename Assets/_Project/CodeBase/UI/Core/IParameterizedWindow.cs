using System;

namespace _Project.CodeBase.UI.Common
{
  public interface IParameterizedWindow<in TData>
  {
    public void Initialize(TData parameter);
  }
}