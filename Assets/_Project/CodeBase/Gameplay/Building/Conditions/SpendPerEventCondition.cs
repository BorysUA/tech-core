using System.Threading.Tasks;
using _Project.CodeBase.Data.Progress.ResourceData;
using _Project.CodeBase.Gameplay.Services.Resource;
using Cysharp.Threading.Tasks;
using R3;

namespace _Project.CodeBase.Gameplay.Building.Conditions
{
  public class SpendPerEventCondition : PermanentCondition
  {
    private readonly IResourceService _resourceService;

    private ResourceAmountData _cost;
    private Observable<AutoResetUniTaskCompletionSource<bool>> _onTriggered;

    public SpendPerEventCondition(IResourceService resourceService)
    {
      _resourceService = resourceService;
    }

    public void Setup(ResourceAmountData cost, Observable<AutoResetUniTaskCompletionSource<bool>> onTriggered)
    {
      _cost = cost;
      _onTriggered = onTriggered;
    }

    protected override Observable<bool> BuildPermanentCondition() =>
      _onTriggered.Select(tks =>
      {
        bool result = _resourceService.TrySpend(_cost);
        tks.TrySetResult(result);
        return result;
      }).Prepend(true);
  }
}