namespace _Project.CodeBase.Gameplay.Services.Resource
{
  public readonly struct ResourceMutationResult
  {
    public static readonly ResourceMutationResult Success = new(ResourceMutationStatus.Success);

    public ResourceMutationStatus Code { get; }
    public bool IsSuccess => Code == ResourceMutationStatus.Success;
    public bool IsFailure => !IsSuccess;

    public ResourceMutationResult(ResourceMutationStatus code) =>
      Code = code;
  }
}