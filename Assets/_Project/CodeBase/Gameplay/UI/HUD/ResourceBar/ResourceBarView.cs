using _Project.CodeBase.Gameplay.Constants;
using R3;
using TMPro;
using UnityEngine;
using Zenject;

namespace _Project.CodeBase.Gameplay.UI.HUD.ResourceBar
{
  public class ResourceBarView : MonoBehaviour
  {
    [SerializeField] private TextMeshProUGUI _metalResource;
    [SerializeField] private TextMeshProUGUI _populationResource;
    [SerializeField] private TextMeshProUGUI _energyResource;
    [SerializeField] private TextMeshProUGUI _coinResource;

    [Inject]
    public void Setup(ResourceBarViewModel viewModel)
    {
      viewModel.Initialize();

      Bind(_metalResource, ResourceKind.Metal, viewModel);
      Bind(_energyResource, ResourceKind.Energy, viewModel);
      Bind(_populationResource, ResourceKind.Population, viewModel);
      Bind(_coinResource, ResourceKind.Coin, viewModel);
    }
    
    private void Bind(TextMeshProUGUI textMeshPro, ResourceKind kind, ResourceBarViewModel viewModel)
    {
      Observable.CombineLatest(
          viewModel.GetAmount(kind),
          viewModel.GetCapacity(kind),
          (amount, capacity) => $"{amount}/{capacity}")
        .Subscribe(result => textMeshPro.text = result)
        .AddTo(this);
    }
  }
}