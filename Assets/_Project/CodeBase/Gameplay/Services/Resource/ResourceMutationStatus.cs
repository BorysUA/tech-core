namespace _Project.CodeBase.Gameplay.Services.Resource
{
  public enum ResourceMutationStatus
  {
    None = 0,
    Success = 1,
    NotEnoughResources = 2,
    NoCapacity = 3,
    InvalidResource = 4,
    InvalidAmount = 5,
    BufferOverflow = 6
  }
}