using _Project.CodeBase.Gameplay.UI.Root;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.UI;
using _Project.CodeBase.UI.Core;
using _Project.CodeBase.UI.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.CodeBase.Gameplay.UI.Factory
{
  public class PopUpFactory : IPopUpFactory
  {
    private readonly AddressMap _addressMap;
    private readonly IAssetProvider _assetProvider;
    private readonly PopUpRepository _popUpRepository;
    private readonly IInstantiator _instantiator;

    private PopUpsCanvas _popUpsCanvas;

    public PopUpFactory(AddressMap addressMap, IAssetProvider assetProvider, IInstantiator instantiator,
      PopUpsCanvas popUpsCanvas, PopUpRepository popUpRepository)
    {
      _addressMap = addressMap;
      _assetProvider = assetProvider;
      _instantiator = instantiator;
      _popUpsCanvas = popUpsCanvas;
      _popUpRepository = popUpRepository;
    }

    public async UniTask<TViewModel> CreatePopUp<TPopUp, TViewModel>(bool isFromCache)
      where TPopUp : BasePopUp<TViewModel> where TViewModel : BasePopUpViewModel
    {
      if (isFromCache && _popUpRepository.TryGetValue<TViewModel>(out BasePopUpViewModel cachedPopUp))
        return cachedPopUp as TViewModel;

      string address = _addressMap.GetAddress<TPopUp>();
      GameObject popUpPrefab = await _assetProvider.LoadAssetAsync<GameObject>(address);
      TPopUp popUpView = _instantiator.InstantiatePrefabForComponent<TPopUp>(popUpPrefab, _popUpsCanvas.Root);
      TViewModel viewModel = _instantiator.Instantiate<TViewModel>();
      popUpView.Setup(viewModel);

      _popUpRepository.Register<TViewModel>(viewModel);

      return viewModel;
    }
  }
}