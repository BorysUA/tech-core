using System;
using _Project.CodeBase.Gameplay.Buildings.Modules;
using _Project.CodeBase.Gameplay.Buildings.Modules.EnergyShield;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using R3;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Buildings.VFX.Module
{
  public class EnergyShieldEffect : ModuleEffect
  {
    private static readonly int ShieldColor = Shader.PropertyToID("_Color");
    private static readonly int ShieldVisibility = Shader.PropertyToID("_CenterVisibility");
    private static readonly int ShieldCutOffY = Shader.PropertyToID("_CutoffY");

    [SerializeField] private GameObject _shieldModel;

    [SerializeField] private Color _onHitColor = Color.red;
    [SerializeField] private float _onHitVisibility = 1;
    [SerializeField] private float _hitAnimationDuration = 1;
    [SerializeField] private float _cutoffYVisible = 1;
    [SerializeField] private float _cutoffYInvisible = 5;
    [SerializeField] private float _fadeAnimationDuration = 1;

    private bool _isVisible;
    private EnergyShieldModule _energyShieldModule;
    private Renderer _shieldRenderer;
    private Collider _shieldCollider;
    private MaterialPropertyBlock _shieldMpb;

    private TweenerCore<float, float, FloatOptions> _shieldHitTween;
    private TweenerCore<float, float, FloatOptions> _shieldOnTween;
    private TweenerCore<float, float, FloatOptions> _shieldOffTween;

    private Color _originalColor;
    private float _originalVisibility;

    public override Type ModuleType => typeof(EnergyShieldModule);
    public IDamageInterceptor DamageInterceptor => _energyShieldModule;

    public override void BindModule(BuildingModule module)
    {
      _energyShieldModule = (EnergyShieldModule)module;

      _shieldModel.transform.localScale = Vector3.one * Mathf.Max(0.01f, _energyShieldModule.Radius);
      _shieldMpb = new MaterialPropertyBlock();
      _shieldRenderer = _shieldModel.GetComponent<Renderer>();
      _shieldCollider = _shieldModel.GetComponent<Collider>();

      _shieldRenderer.GetPropertyBlock(_shieldMpb);

      _originalColor = _shieldRenderer.sharedMaterial.GetColor(ShieldColor);
      _originalVisibility = _shieldRenderer.sharedMaterial.GetFloat(ShieldVisibility);

      bool initial = _energyShieldModule.IsModuleWorking.CurrentValue;
      SetVisibleImmediate(initial);

      _energyShieldModule.IsModuleWorking
        .Subscribe(OnModuleActivityChanged)
        .AddTo(this);

      _energyShieldModule.DamageIntercepted
        .Subscribe(_ => PlayHitEffect())
        .AddTo(this);
    }

    private void OnModuleActivityChanged(bool isActive)
    {
      if (isActive)
        Show();
      else
        Hide();
    }

    private void SetVisibleImmediate(bool visible)
    {
      _isVisible = visible;

      if (visible)
      {
        _shieldModel.SetActive(true);
        _shieldCollider.enabled = true;
        _shieldMpb.SetFloat(ShieldCutOffY, _cutoffYVisible);
      }
      else
      {
        _shieldCollider.enabled = false;
        _shieldModel.SetActive(false);
        _shieldMpb.SetFloat(ShieldCutOffY, _cutoffYInvisible);
      }

      _shieldRenderer.SetPropertyBlock(_shieldMpb);
    }

    private void PlayHitEffect()
    {
      _shieldHitTween?.Kill();

      _shieldHitTween = DOTween.To(() => 0f, t =>
        {
          _shieldMpb.SetFloat(ShieldVisibility, Mathf.Lerp(_onHitVisibility, _originalVisibility, t));
          _shieldMpb.SetColor(ShieldColor, Color.Lerp(_onHitColor, _originalColor, t));
          _shieldRenderer.SetPropertyBlock(_shieldMpb);
        }, 1f, _hitAnimationDuration)
        .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
    }

    private void Show()
    {
      if (_isVisible)
        return;

      _isVisible = true;
      _shieldModel.SetActive(true);
      _shieldCollider.enabled = true;
      _shieldOnTween?.Kill();

      _shieldOnTween = DOTween.To(() => _cutoffYInvisible, currentCutoffY =>
        {
          _shieldMpb.SetFloat(ShieldCutOffY, currentCutoffY);
          _shieldRenderer.SetPropertyBlock(_shieldMpb);
        }, _cutoffYVisible, _fadeAnimationDuration)
        .SetEase(Ease.OutQuad)
        .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
    }

    private void Hide()
    {
      if (!_isVisible)
        return;

      _isVisible = false;
      _shieldCollider.enabled = false;
      _shieldOffTween?.Kill();
      _shieldHitTween?.Kill();

      _shieldOffTween = DOTween.To(() => _cutoffYVisible, currentCutoffY =>
        {
          _shieldMpb.SetFloat(ShieldCutOffY, currentCutoffY);
          _shieldRenderer.SetPropertyBlock(_shieldMpb);
        }, _cutoffYInvisible, _fadeAnimationDuration)
        .SetEase(Ease.InQuad)
        .OnComplete(() => _shieldModel.SetActive(false))
        .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
    }
  }
}