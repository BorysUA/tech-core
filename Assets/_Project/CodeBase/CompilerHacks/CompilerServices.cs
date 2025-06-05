// ReSharper disable once CheckNamespace

namespace System.Runtime.CompilerServices
{
  internal sealed class IsExternalInit
  {
  }

  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
  internal sealed class RequiredMemberAttribute : Attribute
  {
  }

  [AttributeUsage(AttributeTargets.Parameter)]
  internal sealed class CallerArgumentExpressionAttribute : Attribute
  {
    public CallerArgumentExpressionAttribute(string parameterName)
    {
    }
  }
}