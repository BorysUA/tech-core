using _Project.CodeBase.Gameplay.Services;
using _Project.CodeBase.Gameplay.Services.Factories;
using DG.Tweening;
using R3;
using UnityEngine;
using Zenject;

namespace _Project.CodeBase.Gameplay.Resource
{
  public class ResourceDropView : MonoBehaviour
  {
    private ResourceDropViewModel _viewModel;
    private ITweenFactory _tweenFactory;

    private Tween _moveTween;
    private Vector3[] _animationPath = new Vector3[3];
    [SerializeField] private float _animationArcHeight = 1f;
    [SerializeField] private float _animationDuration = 1f;

    public int Id => _viewModel.Id;

    [Inject]
    public void Initialize(ITweenFactory tweenFactory)
    {
      _tweenFactory = tweenFactory;
    }

    public void Setup(ResourceDropViewModel viewModel)
    {
      _viewModel = viewModel;

      _viewModel.Activated
        .Subscribe(_ => Activate())
        .AddTo(this);

      _viewModel.Deactivated
        .Subscribe(_ => Deactivate())
        .AddTo(this);

      _viewModel.SpawnPoint
        .Skip(1)
        .Subscribe(SetPosition)
        .AddTo(this);

      _viewModel.Position
        .Skip(1)
        .Subscribe(SetPositionWithAnimateMovement)
        .AddTo(this);
    }

    private void Activate() =>
      gameObject.SetActive(true);

    private void Deactivate()
    {
      _moveTween?.Kill();
      gameObject.SetActive(false);
    }

    private void SetPosition(Vector3 position) =>
      transform.position = position;

    private void SetPositionWithAnimateMovement(Vector3 toPosition)
    {
      Vector3 controlPoint = (transform.position + toPosition) / 2;
      controlPoint.y += _animationArcHeight;

      _animationPath = new[] { transform.position, controlPoint, toPosition };

      _moveTween = _tweenFactory.CreatePathTween(transform, _animationPath, _animationDuration)
        .OnComplete(() =>
        {
          transform.position = toPosition;
          _viewModel.OnMovementCompleted();
        });

      _moveTween.Play();
    }
  }
}