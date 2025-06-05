using R3;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.CodeBase.Gameplay.UI.Windows.Common
{
  public class ObservableButton : MonoBehaviour
  {
    [SerializeField] private Button _button;
    
    public Observable<Unit> OnClick =>
      _button.OnClickAsObservable();
  }
}