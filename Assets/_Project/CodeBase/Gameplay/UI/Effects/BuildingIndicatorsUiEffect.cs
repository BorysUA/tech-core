using System;
using _Project.CodeBase.Gameplay.Buildings;
using _Project.CodeBase.Gameplay.Services.Buildings;
using _Project.CodeBase.Gameplay.States;
using _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus;
using _Project.CodeBase.UI.Services;
using R3;

namespace _Project.CodeBase.Gameplay.UI.Effects
{
  public class BuildingIndicatorsUiEffect : IGameplayInit, IDisposable
  {
    private readonly IBuildingRepository _buildingRepository;
    private readonly IPopUpService _popUpService;

    private readonly CompositeDisposable _subscriptions = new();

    public BuildingIndicatorsUiEffect(IPopUpService popUpService, IBuildingRepository buildingRepository)
    {
      _popUpService = popUpService;
      _buildingRepository = buildingRepository;
    }

    public void Initialize()
    {
      _buildingRepository.BuildingsAdded
        .Subscribe(viewModel =>
          _popUpService.ShowPopUp<BuildingIndicatorsPopUp, BuildingIndicatorsViewModel, BuildingViewModel>(viewModel,
            false))
        .AddTo(_subscriptions);
    }

    public void Dispose()
    {
      _subscriptions?.Dispose();
    }
  }
}