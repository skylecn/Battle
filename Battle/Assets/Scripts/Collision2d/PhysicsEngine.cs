using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Collision2d
{
    public static class PhysicsCollisionMatrix
    {
        private static int LAYER_COUNT = 32;
        private static int[] masks = new int[LAYER_COUNT];

        public static void SetMask(int layer, int mask)
        {
            if (layer >= 0 && layer < LAYER_COUNT) masks[layer] = mask;
        }

        public static void SetAllTrue()
        {
            for (int i = 0; i < LAYER_COUNT; i++) masks[i] = -1;
        }

        public static void SetAllFalse()
        {
            for (int i = 0; i < LAYER_COUNT; i++) masks[i] = 0;
        }

        public static void SetCollide(int layer1, int layer2, bool collide)
        {
            if (layer1 >= 0 && layer1 < LAYER_COUNT && layer2 >= 0 && layer2 < LAYER_COUNT)
            {
                if (collide) masks[layer1] |= (1 << layer2);
                else masks[layer1] &= ~(1 << layer2);
            }
        }

        public static void SetBothCollide(int layer1, int layer2, bool collide)
        {
            SetCollide(layer1, layer2, collide);
            SetCollide(layer2, layer1, collide);
        }

        public static bool CanCollide(int A, int B) => (masks[A] & (1 << B)) != 0;
    }

    public class PhysicsBody
    {
        public int ID;
        public bool IsStatic;
        public Shape ShapeData;
        public int LayerIndex;
        public object UserData;

        public Vector2 Position
        {
            get => ShapeData.Center;
            set => ShapeData.Center = value;
        }

        public HashSet<PhysicsBody> CurrentCollisions = new HashSet<PhysicsBody>();
        public HashSet<PhysicsBody> PreviousCollisions = new HashSet<PhysicsBody>();
        public Action<PhysicsBody> OnCollisionEnter, OnCollisionStay, OnCollisionExit;

        public PhysicsBody(int id, Shape s, bool stat, int layer, object userData)
        {
            ID = id;
            ShapeData = s;
            IsStatic = stat;
            LayerIndex = layer;
            UserData = userData;
        }

        public void SwapBuffers()
        {
            PreviousCollisions.Clear();
            var t = PreviousCollisions;
            PreviousCollisions = CurrentCollisions;
            CurrentCollisions = t;
        }

        public void DispatchEvents()
        {
            foreach (var o in CurrentCollisions)
                if (PreviousCollisions.Contains(o)) OnCollisionStay?.Invoke(o);
                else OnCollisionEnter?.Invoke(o);
            foreach (var o in PreviousCollisions)
                if (!CurrentCollisions.Contains(o))
                    OnCollisionExit?.Invoke(o);
        }

        public AABB GetWorldAABB()
        {
            if (ShapeData is Circle c) return new AABB(c.Center, Vector2.one * c.Radius * 2);
            if (ShapeData is AABB b) return b;
            if (ShapeData is OBB o) return o.ToAABB();
            return new AABB(Vector2.zero, Vector2.zero);
        }
    }

    public class SpatialHashGrid
    {
        private float size;
        private Dictionary<Vector2Int, List<PhysicsBody>> cells = new Dictionary<Vector2Int, List<PhysicsBody>>();
        public SpatialHashGrid(float s) => size = s;
        public void Clear() => cells.Clear();

        public void Insert(PhysicsBody b)
        {
            AABB bound = b.GetWorldAABB();
            Vector2Int min = GetCell(bound.MinX, bound.MinY), max = GetCell(bound.MaxX, bound.MaxY);
            for (int x = min.x; x <= max.x; x++)
            for (int y = min.y; y <= max.y; y++)
            {
                Vector2Int k = new Vector2Int(x, y);
                if (!cells.ContainsKey(k)) cells[k] = new List<PhysicsBody>();
                cells[k].Add(b);
            }
        }

        public HashSet<PhysicsBody> GetNeighbors(PhysicsBody b)
        {
            HashSet<PhysicsBody> r = new HashSet<PhysicsBody>();
            AABB bound = b.GetWorldAABB();
            Vector2Int min = GetCell(bound.MinX, bound.MinY), max = GetCell(bound.MaxX, bound.MaxY);
            for (int x = min.x; x <= max.x; x++)
            for (int y = min.y; y <= max.y; y++)
                if (cells.TryGetValue(new Vector2Int(x, y), out var l))
                    foreach (var o in l)
                        if (o != b)
                            r.Add(o);
            return r;
        }

        public void QueryRegion(AABB region, HashSet<PhysicsBody> res)
        {
            res.Clear();
            Vector2Int min = GetCell(region.MinX, region.MinY), max = GetCell(region.MaxX, region.MaxY);
            for (int x = min.x; x <= max.x; x++)
            for (int y = min.y; y <= max.y; y++)
                if (cells.TryGetValue(new Vector2Int(x, y), out var l))
                    foreach (var b in l)
                        res.Add(b);
        }

        private Vector2Int GetCell(float x, float y) =>
            new Vector2Int(Mathf.FloorToInt(x / size), Mathf.FloorToInt(y / size));
    }

    public class PhysicsWorld
    {
        public List<PhysicsBody> Bodies = new List<PhysicsBody>();
        private SpatialHashGrid grid;
        private HashSet<PhysicsBody> queryCache = new HashSet<PhysicsBody>();
        public PhysicsWorld(float cs) => grid = new SpatialHashGrid(cs);

        private int nextID = 0;

        public int GetNextID() => nextID++;

        public void Clear()
        {
            Bodies.Clear();
            grid.Clear();
            queryCache.Clear();
        }

        public void AddBody(PhysicsBody b) => Bodies.Add(b);
        public void RemoveBody(PhysicsBody b) => Bodies.Remove(b);

        public PhysicsBody AddCircle(Vector2 pos, float r, int layer, object userData, bool isStatic = false)
        {
            PhysicsBody b = new PhysicsBody(GetNextID(), new Circle(pos, r), isStatic, layer, userData);
            AddBody(b);
            return b;
        }

        public PhysicsBody AddAABB(Vector2 pos, Vector2 size, int layer, object userData, bool isStatic = false)
        {
            PhysicsBody b = new PhysicsBody(GetNextID(), new AABB(pos, size), isStatic, layer, userData);
            AddBody(b);
            return b;
        }

        public PhysicsBody AddOBB(Vector2 pos, Vector2 size, float angle, int layer, object userData,
            bool isStatic = false)
        {
            PhysicsBody b = new PhysicsBody(GetNextID(), new OBB(pos, size, angle), isStatic, layer, userData);
            AddBody(b);
            return b;
        }

        public void Step()
        {
            grid.Clear();
            foreach (var b in Bodies)
            {
                b.SwapBuffers();
                grid.Insert(b);
            }

            foreach (var A in Bodies)
            {
                if (A.IsStatic) continue;
                foreach (var B in grid.GetNeighbors(A))
                {
                    if (B.IsStatic || B.ID > A.ID)
                    {
                        if (!PhysicsCollisionMatrix.CanCollide(A.LayerIndex, B.LayerIndex)) continue;
                        if (Check(A, B))
                        {
                            A.CurrentCollisions.Add(B);
                            B.CurrentCollisions.Add(A);
                        }
                    }
                }
            }

            foreach (var b in Bodies)
                if (b.CurrentCollisions.Count > 0 || b.PreviousCollisions.Count > 0)
                    b.DispatchEvents();
        }

        private bool Check(PhysicsBody b1, PhysicsBody b2)
        {
            Shape s1 = b1.ShapeData, s2 = b2.ShapeData;
            if (s1 is Circle c1 && s2 is Circle c2) return PhysicsCore.CheckCircleVsCircle(c1, c2);
            if (s1 is OBB o1 && s2 is OBB o2) return PhysicsCore.CheckOBBVsOBB(o1, o2);
            if (s1 is AABB a1 && s2 is AABB a2) return PhysicsCore.CheckAABBVsAABB(a1, a2);
            OBB ob = s1 as OBB ?? (s1 is AABB ab ? new OBB(ab.Center, ab.Size, 0) : null);
            Circle ci = s2 as Circle;
            if (ob != null && ci != null) return PhysicsCore.CheckOBBVsCircle(ob, ci);
            ob = s2 as OBB ?? (s2 is AABB ab2 ? new OBB(ab2.Center, ab2.Size, 0) : null);
            ci = s1 as Circle;
            if (ob != null && ci != null) return PhysicsCore.CheckOBBVsCircle(ob, ci);
            return false;
        }

        // Queries
        public RaycastHit Raycast(Vector2 pos, Vector2 dir, float dist, int mask)
        {
            RaycastHit best = new RaycastHit { Distance = float.MaxValue };
            Vector2 nDir = dir.normalized;
            foreach (var b in Bodies)
            {
                if (((1 << b.LayerIndex) & mask) == 0) continue;
                if (!PhysicsCore.RaycastAABB(pos, nDir, dist, b.GetWorldAABB(), out _)) continue;
                RaycastHit h = new RaycastHit();
                bool hit = false;
                if (b.ShapeData is Circle c) hit = PhysicsCore.RaycastCircle(pos, nDir, dist, c, out h);
                else if (b.ShapeData is OBB o) hit = PhysicsCore.RaycastOBB(pos, nDir, dist, o, out h);
                else if (b.ShapeData is AABB a) hit = PhysicsCore.RaycastAABB(pos, nDir, dist, a, out h);
                if (hit && h.Distance < best.Distance)
                {
                    best = h;
                    best.Body = b;
                }
            }

            if (best.Distance == float.MaxValue) best.IsHit = false;
            return best;
        }

        public RaycastHit LineCast(Vector2 start, Vector2 end, int mask)
        {
            return Raycast(start, (end - start).normalized, (end - start).magnitude, mask);
        }

        public RaycastHit ShapeCast(Vector2 pos, Vector2 size, float angle, Vector2 dir, float dist, float r, int mask,
            bool isCircle)
        {
            RaycastHit best = new RaycastHit { Distance = float.MaxValue };
            Vector2 nDir = dir.normalized;
            OBB castOBB = new OBB(pos, size, angle);
            AABB start = isCircle ? new AABB(pos, Vector2.one * r * 2) : castOBB.ToAABB();
            Vector2 endCenter = pos + nDir * dist;
            AABB end = isCircle ? new AABB(endCenter, Vector2.one * r * 2) : new OBB(endCenter, size, angle).ToAABB();
            AABB broad = new AABB((start.Center + end.Center) / 2f,
                new Vector2(Mathf.Max(start.MaxX, end.MaxX) - Mathf.Min(start.MinX, end.MinX),
                    Mathf.Max(start.MaxY, end.MaxY) - Mathf.Min(start.MinY, end.MinY)));

            foreach (var b in Bodies)
            {
                if (((1 << b.LayerIndex) & mask) == 0) continue;
                AABB bAABB = b.GetWorldAABB();
                AABB expanded =
                    new AABB(bAABB.Center,
                        bAABB.Size + (isCircle ? Vector2.one * r * 2 : start.Size)); // very rough approx for broadphase
                if (!PhysicsCore.CheckAABBVsAABB(broad, expanded)) continue;

                RaycastHit h = new RaycastHit();
                bool hit = false;
                // Narrow Phase Dispatch
                Shape target = b.ShapeData;
                if (isCircle)
                {
                    if (target is Circle c) hit = PhysicsCore.CircleCastCircle(pos, nDir, dist, r, c, out h);
                    else if (target is OBB o) hit = PhysicsCore.CircleCastOBB(pos, nDir, dist, r, o, out h);
                    else if (target is AABB a)
                        hit = PhysicsCore.CircleCastOBB(pos, nDir, dist, r, new OBB(a.Center, a.Size, 0), out h);
                }
                else
                {
                    if (target is Circle c) hit = PhysicsCore.BoxCastCircle(pos, size, angle, nDir, dist, c, out h);
                    else if (target is OBB o) hit = PhysicsCore.BoxCastOBB(pos, size, angle, nDir, dist, o, out h);
                    else if (target is AABB a)
                        hit = PhysicsCore.BoxCastOBB(pos, size, angle, nDir, dist, new OBB(a.Center, a.Size, 0), out h);
                }

                if (hit && h.Distance < best.Distance)
                {
                    best = h;
                    best.Body = b;
                }
            }

            if (best.Distance == float.MaxValue) best.IsHit = false;
            return best;
        }

        public RaycastHit CircleCast(Vector2 pos, Vector2 dir, float dist, float r, int mask)
        {
            return ShapeCast(pos, Vector2.zero, 0, dir, dist, r, mask, true);
        }

        public RaycastHit BoxCast(Vector2 pos, Vector2 size, float angle, Vector2 dir, float dist, int mask)
        {
            return ShapeCast(pos, size, angle, dir, dist, 0, mask, false);
        }

        public List<PhysicsBody> Overlap(Vector2 p, Vector2 sz, float ang, float r, int mask, int type)
        {
            // 0:Point, 1:Circle, 2:Box
            List<PhysicsBody> res = new List<PhysicsBody>();
            AABB region;
            if (type == 0) region = new AABB(p, Vector2.zero);
            else if (type == 1) region = new AABB(p, Vector2.one * r * 2);
            else region = new OBB(p, sz, ang).ToAABB();

            grid.QueryRegion(region, queryCache);
            foreach (var b in queryCache)
            {
                if (((1 << b.LayerIndex) & mask) == 0) continue;
                bool hit = false;
                if (type == 0)
                {
                    // Point
                    if (b.ShapeData is Circle c) hit = PhysicsCore.PointInCircle(p, c);
                    else if (b.ShapeData is OBB o) hit = PhysicsCore.PointInOBB(p, o);
                    else if (b.ShapeData is AABB a) hit = PhysicsCore.PointInAABB(p, a);
                }
                else if (type == 1)
                {
                    // Circle
                    Circle q = new Circle(p, r);
                    if (b.ShapeData is Circle c) hit = PhysicsCore.CheckCircleVsCircle(q, c);
                    else if (b.ShapeData is OBB o) hit = PhysicsCore.CheckOBBVsCircle(o, q);
                    else if (b.ShapeData is AABB a) hit = PhysicsCore.CheckOBBVsCircle(new OBB(a.Center, a.Size, 0), q);
                }
                else
                {
                    // Box
                    OBB q = new OBB(p, sz, ang);
                    if (b.ShapeData is Circle c) hit = PhysicsCore.CheckOBBVsCircle(q, c);
                    else if (b.ShapeData is OBB o) hit = PhysicsCore.CheckOBBVsOBB(q, o);
                    else if (b.ShapeData is AABB a) hit = PhysicsCore.CheckOBBVsOBB(q, new OBB(a.Center, a.Size, 0));
                }

                if (hit) res.Add(b);
            }

            return res;
        }

        public List<PhysicsBody> OverlapPoint(Vector2 p, int mask)
        {
            return Overlap(p, Vector2.zero, 0, 0, mask, 0);
        }

        public List<PhysicsBody> OverlapCircle(Vector2 p, float r, int mask)
        {
            return Overlap(p, Vector2.zero, 0, r, mask, 1);
        }

        public List<PhysicsBody> OverlapBox(Vector2 p, Vector2 size, float angle, int mask)
        {
            return Overlap(p, size, angle, 0, mask, 2);
        }
    }
}