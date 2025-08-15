using Cysharp.Threading.Tasks;

namespace _Project.CodeBase.Gameplay.Buildings.Modules.EnergyShield
{
  public interface IDamageInterceptor
  {
    public UniTask<bool> TryInterceptDamage(int damage);
  }
}