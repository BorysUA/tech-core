using UnityEngine;

namespace _Project.CodeBase.Data.StaticData.Meteorite
{
  [CreateAssetMenu(fileName = "MeteoriteVFX", menuName = "ScriptableObjects/MeteoriteVFX", order = 0)]
  public class MeteoriteVFXConfig : ScriptableObject
  {
    public MeteoriteVFX[] MeteoriteVFX;
  }
}