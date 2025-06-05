using System.Threading.Tasks;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Meteorite;
using _Project.CodeBase.Gameplay.Resource;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Services
{
  public interface IGameplayFactory
  {
    void Initialize();
    UniTask<MeteoriteViewModel> CreateMeteorite(MeteoriteType type, Vector3 position);
    UniTask<ResourceDropViewModel> CreateResourceDrop(ResourceDropType type, Vector3 position);
  }
}