using System;
using _Project.CodeBase.Gameplay.Buildings.Modules;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Buildings.VFX
{
  public abstract class ModuleEffect : MonoBehaviour
  {
    public abstract Type ModuleType { get; }
    public abstract void BindModule(BuildingModule module);
  }
}