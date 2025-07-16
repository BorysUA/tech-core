using System;
using _Project.CodeBase.Gameplay.Building.Modules;
using _Project.CodeBase.Gameplay.Building.Modules.EnergyShield;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using R3;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Building.VFX.Module
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

      _energyShieldModule.IsModuleWorking
        .Subscribe(OnModuleActivityChanged)
        .AddTo(this);

      _energyShieldModule.DamageIntercepted
        .Subscribe(_ => PlayHitEffect())
        .AddTo(this);
    }

    private void PlayHitEffect()
    {
      _shieldHitTween?.Kill();

      _shieldHitTween = DOTween.To(() => 0f, t =>
      {
        _shieldMpb.SetFloat(ShieldVisibility, Mathf.Lerp(_onHitVisibility, _originalVisibility, t));
        _shieldMpb.SetColor(ShieldColor, Color.Lerp(_onHitColor, _originalColor, t));
        _shieldRenderer.SetPropertyBlock(_shieldMpb);
      }, 1f, _hitAnimationDuration);
    }

    private void OnModuleActivityChanged(bool isActive)
    {
      if (isActive)
        Show();
      else
        Hide();
    }

    private void Show()
    {
      _shieldModel.SetActive(true);
      _shieldCollider.enabled = true;

      _shieldOnTween?.Kill();

      _shieldOnTween = DOTween.To(() => _cutoffYInvisible, currentCutoffY =>
        {
          _shieldMpb.SetFloat(ShieldCutOffY, currentCutoffY);
          _shieldRenderer.SetPropertyBlock(_shieldMpb);
        }, _cutoffYVisible, _fadeAnimationDuration)
        .SetEase(Ease.OutQuad);
    }

    private void Hide()
    {
      _shieldCollider.enabled = false;

      _shieldOnTween?.Kill();

      _shieldOnTween = DOTween.To(() => _cutoffYVisible, currentCutoffY =>
        {
          _shieldMpb.SetFloat(ShieldCutOffY, currentCutoffY);
          _shieldRenderer.SetPropertyBlock(_shieldMpb);
        }, _cutoffYInvisible, _fadeAnimationDuration)
        .SetEase(Ease.InQuad)
        .OnComplete(() => _shieldModel.SetActive(false));
    }
  }
}