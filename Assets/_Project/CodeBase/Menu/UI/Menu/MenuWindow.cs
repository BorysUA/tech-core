using _Project.CodeBase.UI.Common;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.CodeBase.Menu.UI.Window
{
  public class MenuWindow : BaseWindow<MenuViewModel>
  {
    [SerializeField] private Button _play;
    [SerializeField] private Button _loadGame;
    [SerializeField] private Button _settings;

    public override void Setup(BaseWindowViewModel viewModel)
    {
      base.Setup(viewModel);

      _play.OnClickAsObservable()
        .Subscribe(_ => ViewModel.SwitchToDifficultySelection())
        .AddTo(this);
      
      _loadGame.OnClickAsObservable()
        .Subscribe(_ => ViewModel.SwitchToLoadSavedGames())
        .AddTo(this);
    }
  }
}