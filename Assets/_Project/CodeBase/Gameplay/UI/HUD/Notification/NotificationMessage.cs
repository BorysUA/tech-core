using _Project.CodeBase.Gameplay.Services;
using _Project.CodeBase.Gameplay.Services.Factories;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;

namespace _Project.CodeBase.Gameplay.UI.HUD.Notification
{
  public class NotificationMessage : MonoBehaviour
  {
    private ITweenFactory _tweenFactory;
    private Sequence _contentSequence;

    [SerializeField] private TextMeshProUGUI _messageText;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _content;

    [Header("Animation Settings")] [SerializeField]
    private float _contentVerticalOffset = 50f;

    [SerializeField] private float _contentTransitionDuration = 1f;

    [SerializeField] private float _fadeTargetAlpha;
    [SerializeField] private float _fadeTransitionDuration = 1f;


    [Inject]
    public void Construct(ITweenFactory tweenFactory) =>
      _tweenFactory = tweenFactory;

    public void Setup(ToastData data)
    {
      _messageText.text = data.Text;
      _fadeTransitionDuration = data.Duration;
      _contentTransitionDuration = data.Duration;

      if (data.TextColor != null)
        _messageText.color = data.TextColor.Value;

      SetupContentSequence();
    }

    public void Show()
    {
      _contentSequence.Play();
    }

    private void SetupContentSequence()
    {
      Tween yMovementTween =
        _tweenFactory.CreateYMovementTween(_content, _contentVerticalOffset, _contentTransitionDuration);
      Tween fadeTween = _tweenFactory.CreateFadeTween(_canvasGroup, _fadeTargetAlpha, _fadeTransitionDuration);
      
      _contentSequence = DOTween.Sequence();
      _contentSequence.Join(yMovementTween);
      _contentSequence.Join(fadeTween);

      _contentSequence.OnComplete(() => Destroy(gameObject));
    }
  }
}