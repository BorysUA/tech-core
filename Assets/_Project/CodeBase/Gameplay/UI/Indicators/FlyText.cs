using _Project.CodeBase.Gameplay.Services.Factories;
using _Project.CodeBase.Gameplay.Services.Pool;
using DG.Tweening;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static DG.Tweening.DOTween;

namespace _Project.CodeBase.Gameplay.UI.Indicators
{
  public class FlyText : MonoBehaviour, IResettablePoolItem<PoolUnit>
  {
    private const string POS_FMT = "+{0}";
    private const string NEG_FMT = "{0}";
    private const string ZERO_FMT = "0";

    private ITweenFactory _tweenFactory;
    private Sequence _contentSequence;
    private RectTransform _rectTransform;

    private readonly Subject<Unit> _deactivated = new();

    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private Image _icon;
    [SerializeField] private RectTransform _content;

    [Header("Animation Settings")] [SerializeField]
    private float _contentVerticalOffset = 50f;

    [SerializeField] private float _contentTransitionDuration = 1f;

    [SerializeField] private float _fadeTargetAlpha;
    [SerializeField] private float _fadeTransitionDuration = 1f;

    public Observable<Unit> Deactivated => _deactivated;

    public Vector3 WorldSpawnPoint { get; private set; }

    private void Awake() =>
      _rectTransform = GetComponent<RectTransform>();

    [Inject]
    public void Construct(ITweenFactory tweenFactory)
    {
      _tweenFactory = tweenFactory;
      SetupContentSequence();
    }

    public void Setup(Sprite icon) =>
      _icon.sprite = icon;

    public void Initialize(Vector3 worldPosition, int amount)
    {
      WorldSpawnPoint = worldPosition;

      _title.SetText(amount > 0 ? "+{0}" : "{0}", amount);

      _contentSequence.Play();
    }

    public void Activate(PoolUnit param) =>
      gameObject.SetActive(true);

    private void Deactivate()
    {
      gameObject.SetActive(false);
      _deactivated.OnNext(Unit.Default);
    }

    public void SetPosition(Vector2 position) =>
      _rectTransform.anchoredPosition = position;

    public void Reset() =>
      _contentSequence.Rewind();

    private void SetupContentSequence()
    {
      Tween yMovementTween =
        _tweenFactory.CreateYMovementTween(_content, _contentVerticalOffset, _contentTransitionDuration);
      Tween fadeTween = _tweenFactory.CreateFadeTween(_canvasGroup, _fadeTargetAlpha, _fadeTransitionDuration);

      _contentSequence = Sequence();
      _contentSequence.Join(yMovementTween);
      _contentSequence.Join(fadeTween);

      _contentSequence.OnComplete(Deactivate);
    }
  }
}