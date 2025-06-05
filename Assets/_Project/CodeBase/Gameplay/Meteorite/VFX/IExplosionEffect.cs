namespace _Project.CodeBase.Gameplay.Meteorite.VFX
{
  public interface IExplosionEffect
  {
    public void Play();
    public float Progress { get; }
  }
}