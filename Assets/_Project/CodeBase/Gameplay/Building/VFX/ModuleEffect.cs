using System;
using _Project.CodeBase.Gameplay.Building.Modules;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Building.VFX
{
  public abstract class ModuleEffect : MonoBehaviour
  {
    public abstract Type ModuleType { get; }
    public abstract void BindModule(BuildingModule module);
  }
}