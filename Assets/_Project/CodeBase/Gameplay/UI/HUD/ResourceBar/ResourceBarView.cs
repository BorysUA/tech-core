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
      viewModel.Metal
        .Subscribe(value => _metalResource.text = value)
        .AddTo(this);
      
      viewModel.Energy
        .Subscribe(value => _energyResource.text = value)
        .AddTo(this);
      
      viewModel.Population
        .Subscribe(value => _populationResource.text = value)
        .AddTo(this);
      
      viewModel.Coin
        .Subscribe(value => _coinResource.text = value)
        .AddTo(this);
    }
  }
}