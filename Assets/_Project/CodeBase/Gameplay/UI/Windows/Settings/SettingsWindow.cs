using _Project.CodeBase.UI.Core;
using _Project.CodeBase.UI.Helpers;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.CodeBase.Gameplay.UI.Windows.Settings
{
  public class SettingsWindow : BaseWindow<SettingsViewModel>
  {
    [SerializeField] private Button _saveProgressButton;
    [SerializeField] private Button _exitToMenuButton;
    [SerializeField] private Button _topRightCloseButton;
    [SerializeField] private PointerDownListener _fullScreenCloseButton;

    public override void Setup(BaseWindowViewModel viewModel)
    {
      base.Setup(viewModel);

      _saveProgressButton.onClick.AddListener(() => ViewModel.SaveCurrentProgress());
      _exitToMenuButton.onClick.AddListener(() => ViewModel.ExitToMenu());

      BindCloseActions(_topRightCloseButton.onClick.AsObservable(), _fullScreenCloseButton.PointerDown);
    }
  }
}