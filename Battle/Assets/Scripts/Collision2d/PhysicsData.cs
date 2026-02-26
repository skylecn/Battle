using UnityEngine;

namespace Collision2d
{
// === 基础形状定义 ===
    public abstract class Shape
    {
        public Vector2 Center;
    }

    public class Circle : Shape
    {
        public float Radius;

        public Circle(Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;
        }
    }

    public class AABB : Shape
    {
        public Vector2 Size;
        public Vector2 HalfSize => Size * 0.5f;
        public float MinX => Center.x - HalfSize.x;
        public float MaxX => Center.x + HalfSize.x;
        public float MinY => Center.y - HalfSize.y;
        public float MaxY => Center.y + HalfSize.y;

        public AABB(Vector2 center, Vector2 size)
        {
            Center = center;
            Size = size;
        }
    }

    public class OBB : Shape
    {
        public Vector2 Size;
        public float Rotation; // 角度 (Degrees)

        public OBB(Vector2 center, Vector2 size, float rotation)
        {
            Center = center;
            Size = size;
            Rotation = rotation;
        }

        // 获取 OBB 的外接 AABB (用于 BroadPhase)
        public AABB ToAABB()
        {
            float rad = Rotation * Mathf.Deg2Rad;
            float c = Mathf.Abs(Mathf.Cos(rad));
            float s = Mathf.Abs(Mathf.Sin(rad));
            float w = Size.x * c + Size.y * s;
            float h = Size.x * s + Size.y * c;
            return new AABB(Center, new Vector2(w, h));
        }
    }

// === 射线/扫描 结果结构 ===
    public struct RaycastHit
    {
        public bool IsHit;
        public PhysicsBody Body;
        public Vector2 Point; // 击中点
        public Vector2 Normal; // 法线
        public float Distance; // 距离
    }
}