using _Project.CodeBase.UI.Core;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.CodeBase.Menu.UI.DifficultySelection
{
  public class DifficultySelectionWindow : BaseWindow<DifficultySelectionViewModel>
  {
    [SerializeField] private SelectDifficultyButton[] _buttons;
    [SerializeField] private Button _backToMenuButton;

    public override void Setup(BaseWindowViewModel viewModel)
    {
      base.Setup(viewModel);

      foreach (SelectDifficultyButton button in _buttons)
      {
        button.OnClickAsObservable
          .Subscribe(difficulty => ViewModel.ApplyDifficultySelection(difficulty))
          .AddTo(this);
      }

      _backToMenuButton
        .OnClickAsObservable()
        .Subscribe(_ => ViewModel.BackToMenu())
        .AddTo(this);
    }
  }
}