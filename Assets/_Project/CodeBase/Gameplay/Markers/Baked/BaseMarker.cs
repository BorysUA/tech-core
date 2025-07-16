using _Project.CodeBase.Data.StaticData.Map;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Markers.Baked
{
  public abstract class BaseMarker : MonoBehaviour
  {
    public abstract MapEntityData Bake();
  }
}