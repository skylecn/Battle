using System.Collections.Generic;
using UnityEngine;

namespace Collision2d.Example
{
    public class MyPhysicsManager : MonoBehaviour
    {
        public static MyPhysicsManager Instance;
        [Header("Config")] public float broadPhaseCellSize = 2.0f;
        public bool useUnityPhysics2DMatrix = true;
        public PhysicsWorld world;

        private void Awake()
        {
            Instance = this;
            world = new PhysicsWorld(broadPhaseCellSize);
            if (useUnityPhysics2DMatrix)
            {
                for (int i = 0; i < 32; i++) PhysicsCollisionMatrix.SetMask(i, Physics2D.GetLayerCollisionMask(i));
            }
            else
            {
                for (int i = 0; i < 32; i++) PhysicsCollisionMatrix.SetMask(i, -1);
            }
        }

        private void FixedUpdate() => world.Step();

        public void RegisterBody(MyPhysicsBody mb)
        {
            mb.Initialize(world.GetNextID());
            world.AddBody(mb.InternalBody);
        }

        public void UnregisterBody(MyPhysicsBody mb)
        {
            if (mb.InternalBody != null) world.RemoveBody(mb.InternalBody);
        }

        // === Static APIs ===
        public static RaycastHit Raycast(Vector2 pos, Vector2 dir, float dist, int mask = -1)
            => Instance ? Instance.world.Raycast(pos, dir, dist, mask) : new RaycastHit();

        public static RaycastHit CircleCast(Vector2 pos, Vector2 dir, float dist, float r, int mask = -1)
            => Instance ? Instance.world.ShapeCast(pos, Vector2.zero, 0, dir, dist, r, mask, true) : new RaycastHit();

        public static RaycastHit BoxCast(Vector2 pos, Vector2 size, float angle, Vector2 dir, float dist, int mask = -1)
            => Instance ? Instance.world.ShapeCast(pos, size, angle, dir, dist, 0, mask, false) : new RaycastHit();

        public static List<PhysicsBody> OverlapPoint(Vector2 pos, int mask = -1)
            => Instance ? Instance.world.Overlap(pos, Vector2.zero, 0, 0, mask, 0) : new List<PhysicsBody>();

        public static List<PhysicsBody> OverlapCircle(Vector2 pos, float r, int mask = -1)
            => Instance ? Instance.world.Overlap(pos, Vector2.zero, 0, r, mask, 1) : new List<PhysicsBody>();

        public static List<PhysicsBody> OverlapBox(Vector2 pos, Vector2 size, float angle, int mask = -1)
            => Instance ? Instance.world.Overlap(pos, size, angle, 0, mask, 2) : new List<PhysicsBody>();
    }
}