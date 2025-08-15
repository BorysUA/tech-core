using System;

namespace _Project.CodeBase.Gameplay.Models
{
  public readonly struct Vector3 : IEquatable<Vector3>
  {
    public readonly float X;
    public readonly float Y;
    public readonly float Z;

    public Vector3(float x, float y, float z)
    {
      X = x;
      Y = y;
      Z = z;
    }

    public static Vector3 Zero => default;

    public static Vector3 operator +(Vector3 a, Vector3 b)
      => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    public static Vector3 operator -(Vector3 a, Vector3 b)
      => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    public static Vector3 operator *(Vector3 a, float d)
      => new(a.X * d, a.Y * d, a.Z * d);

    public static Vector3 operator *(float d, Vector3 a)
      => new(a.X * d, a.Y * d, a.Z * d);

    public static bool operator ==(Vector3 a, Vector3 b)
      => a.Equals(b);

    public static bool operator !=(Vector3 a, Vector3 b)
      => !a.Equals(b);

    public static implicit operator UnityEngine.Vector3(Vector3 v) =>
      new(v.X, v.Y, v.Z);

    public static implicit operator Vector3(UnityEngine.Vector3 v) =>
      new(v.x, v.y, v.z);

    public static float Dot(Vector3 a, Vector3 b)
      => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

    public static Vector3 Cross(Vector3 a, Vector3 b) =>
      new(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);

    public static float Distance(Vector3 a, Vector3 b)
    {
      var dx = a.X - b.X;
      var dy = a.Y - b.Y;
      var dz = a.Z - b.Z;
      return MathF.Sqrt(dx * dx + dy * dy + dz * dz);
    }

    public bool Equals(Vector3 other) => X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
    public override bool Equals(object obj) => obj is Vector3 other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(X, Y, Z);
    public override string ToString() => $"({X:0.###}, {Y:0.###}, {Z:0.###})";

    public float Magnitude
      => MathF.Sqrt(X * X + Y * Y + Z * Z);

    public Vector3 Normalized()
    {
      float m = Magnitude;
      return m > 1e-5f ? new Vector3(X / m, Y / m, Z / m) : Zero;
    }
  }
}