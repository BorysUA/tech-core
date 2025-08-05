using _Project.CodeBase.Data.StaticData.Building.Modules;
using Cysharp.Threading.Tasks;
using R3;

namespace _Project.CodeBase.Gameplay.Building.Modules.EnergyShield
{
  public class EnergyShieldModule : BuildingModule, IResourceSpenderEventSource, IDamageInterceptor
  {
    private readonly Subject<Unit> _damageIntercepted = new();
    private readonly Subject<AutoResetUniTaskCompletionSource<bool>> _resourceSpendRequest = new();

    private ShieldConfig _shieldConfig;

    public float Radius => _shieldConfig.Radius;
    public Observable<AutoResetUniTaskCompletionSource<bool>> ResourceSpendRequested => _resourceSpendRequest;
    public Observable<Unit> DamageIntercepted => _damageIntercepted;

    public void Setup(ShieldConfig shieldConfig)
    {
      _shieldConfig = shieldConfig;
    }

    public async UniTask<bool> TryInterceptDamage(int damage)
    {
      GuardActive();

      var spendRequestTcs = AutoResetUniTaskCompletionSource<bool>.Create();
      _resourceSpendRequest.OnNext(spendRequestTcs);
      bool isSuccess = await spendRequestTcs.Task;

      if (isSuccess)
        _damageIntercepted.OnNext(Unit.Default);

      return isSuccess;
    }
  }
}