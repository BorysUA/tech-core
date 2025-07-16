using System.Collections;
using _Project.CodeBase.Gameplay.Meteorite.VFX;
using _Project.CodeBase.Gameplay.Services.Factories;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Zenject;

namespace _Project.CodeBase.Gameplay.Meteorite
{
  public class MeteoriteView : MonoBehaviour
  {
    [SerializeField] private Collider _meteoriteCollider;
    [SerializeField] private Transform _model;

    private IVFXFactory _vfxFactory;
    private MeteoriteViewModel _meteoriteViewModel;

    private ITrailEffect _trailEffect;

    [Inject]
    public void Construct(IVFXFactory vfxFactory)
    {
      _vfxFactory = vfxFactory;
    }

    public void Setup(MeteoriteViewModel meteoriteViewModel)
    {
      _meteoriteViewModel = meteoriteViewModel;

      _meteoriteViewModel.Initialized
        .Subscribe(_ => PlayTrailEffect())
        .AddTo(this);

      _meteoriteViewModel.Position
        .Subscribe(toPosition => transform.position = toPosition)
        .AddTo(this);

      _meteoriteViewModel.Rotation
        .Subscribe(toRotation => _model.rotation = toRotation)
        .AddTo(this);

      _meteoriteViewModel.Direction
        .Subscribe(direction => _trailEffect?.SetDirection(-direction))
        .AddTo(this);

      _meteoriteViewModel.Exploded
        .Subscribe(_ => Explode())
        .AddTo(this);

      _meteoriteViewModel.Activated
        .Subscribe(_ => Activate())
        .AddTo(this);
    }

    public void Update() =>
      _meteoriteViewModel.Update(Time.deltaTime);

    public void OnTriggerEnter(Collider other) =>
      _meteoriteViewModel.HandleMeteorCollision(other).Forget();

    private void Activate()
    {
      _meteoriteCollider.enabled = true;
      gameObject.SetActive(true);
    }

    private async void PlayTrailEffect()
    {
      _trailEffect = await _vfxFactory.CreateTrailEffect(_meteoriteViewModel.Type, transform);
      _trailEffect.SetDirection(-_meteoriteViewModel.Direction.CurrentValue);
      _trailEffect.Play();
    }

    private async void Explode()
    {
      _trailEffect.Stop();
      _meteoriteCollider.enabled = false;

      IExplosionEffect effect = await _vfxFactory.CreateExplosionEffect(_meteoriteViewModel.Type, transform.position);

      StartCoroutine(DestroyDelay(effect));
    }

    private IEnumerator DestroyDelay(IExplosionEffect effect)
    {
      effect.Play();

      while (effect.Progress < 0.4f)
        yield return new WaitForEndOfFrame();

      gameObject.SetActive(false);
      _meteoriteViewModel.Deactivate();
    }
  }
}