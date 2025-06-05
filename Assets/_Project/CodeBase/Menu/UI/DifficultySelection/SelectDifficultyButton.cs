using R3;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.CodeBase.Menu.UI.DifficultySelection
{
  public class SelectDifficultyButton : MonoBehaviour
  {
    [SerializeField] private Button _button;
    [SerializeField] private GameDifficulty _gameDifficulty;

    public Observable<GameDifficulty> OnClickAsObservable =>
      _button.OnClickAsObservable().Select(_ => _gameDifficulty);
  }
}