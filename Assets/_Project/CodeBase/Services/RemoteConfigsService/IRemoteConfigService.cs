using _Project.CodeBase.Infrastructure.Services.Interfaces;

namespace _Project.CodeBase.Services.RemoteConfigsService
{
  public interface IRemoteConfigService : IServiceReadyAwaiter, IRemoteConfigServiceInternal
  {
  }
}