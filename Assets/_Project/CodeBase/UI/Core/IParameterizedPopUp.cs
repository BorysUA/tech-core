using System;
using _Project.CodeBase.Gameplay.UI.PopUps.Common;
using R3;

namespace _Project.CodeBase.UI.Common
{
  public interface IParameterizedPopUp<in TData>
  {
    public event Action Initialized;
    public void Initialize(TData parameter);
  }
}