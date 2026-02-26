using UnityEngine;

namespace Collision2d
{
    public static class PhysicsCore
    {
        // ================== 1. 静态碰撞检测 (Collision) ==================
        public static bool CheckCircleVsCircle(Circle a, Circle b)
        {
            return (a.Center - b.Center).sqrMagnitude < (a.Radius + b.Radius) * (a.Radius + b.Radius);
        }

        public static bool CheckAABBVsAABB(AABB a, AABB b)
        {
            return !(a.MaxX < b.MinX || a.MinX > b.MaxX || a.MaxY < b.MinY || a.MinY > b.MaxY);
        }
        
        public static bool CheckOBBVsOBB(OBB a, OBB b)
        {
            Vector2[] cA = GetOBBCorners(a);
            Vector2[] cB = GetOBBCorners(b);
            Vector2[] axes =
                { GetAxis(cA[0], cA[1]), GetAxis(cA[1], cA[2]), GetAxis(cB[0], cB[1]), GetAxis(cB[1], cB[2]) };
            foreach (var axis in axes)
                if (!IsOverlapOnAxis(axis, cA, cB))
                    return false;
            return true;
        }

        public static bool CheckOBBVsCircle(OBB obb, Circle circle)
        {
            Vector2 localPos = TransformPointInverse(circle.Center, obb.Center, obb.Rotation);
            Vector2 half = obb.Size / 2f;
            Vector2 closest = new Vector2(Mathf.Clamp(localPos.x, -half.x, half.x),
                Mathf.Clamp(localPos.y, -half.y, half.y));
            return (localPos - closest).sqrMagnitude < (circle.Radius * circle.Radius);
        }

        // ================== 2. 覆盖检测 (Overlap) ==================
        public static bool PointInCircle(Vector2 p, Circle c) => (p - c.Center).sqrMagnitude < c.Radius * c.Radius;

        public static bool PointInAABB(Vector2 p, AABB a) =>
            p.x >= a.MinX && p.x <= a.MaxX && p.y >= a.MinY && p.y <= a.MaxY;

        public static bool PointInOBB(Vector2 p, OBB obb)
        {
            Vector2 lp = TransformPointInverse(p, obb.Center, obb.Rotation);
            return Mathf.Abs(lp.x) <= obb.Size.x / 2f && Mathf.Abs(lp.y) <= obb.Size.y / 2f;
        }

        // ================== 3. 射线检测 (Raycast) ==================
        public static bool RaycastCircle(Vector2 origin, Vector2 dir, float dist, Circle c, out RaycastHit hit)
        {
            hit = new RaycastHit();
            Vector2 m = origin - c.Center;
            float b = Vector2.Dot(m, dir);
            float k = Vector2.Dot(m, m) - c.Radius * c.Radius;
            if (k > 0f && b > 0f) return false;
            float discr = b * b - k;
            if (discr < 0f) return false;
            float t = -b - Mathf.Sqrt(discr);
            if (t < 0f) t = 0f;
            if (t > dist) return false;

            hit.IsHit = true;
            hit.Distance = t;
            hit.Point = origin + dir * t;
            hit.Normal = (hit.Point - c.Center).normalized;
            return true;
        }

        public static bool RaycastAABB(Vector2 origin, Vector2 dir, float dist, AABB aabb, out RaycastHit hit)
        {
            hit = new RaycastHit();
            float dx = (dir.x == 0) ? 1e-20f : dir.x, dy = (dir.y == 0) ? 1e-20f : dir.y;
            float t1 = (aabb.MinX - origin.x) / dx, t2 = (aabb.MaxX - origin.x) / dx;
            float t3 = (aabb.MinY - origin.y) / dy, t4 = (aabb.MaxY - origin.y) / dy;
            float tmin = Mathf.Max(Mathf.Min(t1, t2), Mathf.Min(t3, t4));
            float tmax = Mathf.Min(Mathf.Max(t1, t2), Mathf.Max(t3, t4));
            if (tmax < 0 || tmin > tmax) return false;
            float t = (tmin < 0) ? 0 : tmin;
            if (t > dist) return false;

            hit.IsHit = true;
            hit.Distance = t;
            hit.Point = origin + dir * t;
            Vector2 p = hit.Point - aabb.Center;
            hit.Normal = (aabb.HalfSize.x - Mathf.Abs(p.x) < aabb.HalfSize.y - Mathf.Abs(p.y))
                ? new Vector2(Mathf.Sign(p.x), 0)
                : new Vector2(0, Mathf.Sign(p.y));
            return true;
        }

        public static bool RaycastOBB(Vector2 origin, Vector2 dir, float dist, OBB obb, out RaycastHit hit)
        {
            float rad = -obb.Rotation * Mathf.Deg2Rad, c = Mathf.Cos(rad), s = Mathf.Sin(rad);
            Vector2 lOrg = TransformPoint(origin, obb.Center, c, s, true);
            Vector2 lDir = TransformVector(dir, c, s);
            if (RaycastAABB(lOrg, lDir, dist, new AABB(Vector2.zero, obb.Size), out RaycastHit lh))
            {
                hit = lh;
                hit.Point = origin + dir * lh.Distance;
                float r2 = obb.Rotation * Mathf.Deg2Rad;
                hit.Normal = TransformVector(lh.Normal, Mathf.Cos(r2), Mathf.Sin(r2));
                return true;
            }

            hit = new RaycastHit();
            return false;
        }

        // ================== 4. 形状扫描 (CircleCast / BoxCast) ==================
        public static bool CircleCastCircle(Vector2 origin, Vector2 dir, float dist, float r, Circle target,
            out RaycastHit hit)
        {
            if (RaycastCircle(origin, dir, dist, new Circle(target.Center, target.Radius + r), out hit))
            {
                hit.Point -= hit.Normal * r;
                return true;
            }

            return false;
        }

        public static bool CircleCastOBB(Vector2 origin, Vector2 dir, float dist, float r, OBB obb, out RaycastHit hit)
        {
            hit = new RaycastHit();
            float rad = -obb.Rotation * Mathf.Deg2Rad, c = Mathf.Cos(rad), s = Mathf.Sin(rad);
            Vector2 lOrg = TransformPoint(origin, obb.Center, c, s, true);
            Vector2 lDir = TransformVector(dir, c, s);
            AABB expanded = new AABB(Vector2.zero, obb.Size + Vector2.one * (r * 2));

            if (!RaycastAABB(lOrg, lDir, dist, expanded, out RaycastHit bh)) return false;

            Vector2 half = obb.Size / 2f;
            if (Mathf.Abs(bh.Point.x) <= half.x + 0.001f || Mathf.Abs(bh.Point.y) <= half.y + 0.001f)
            {
                hit = bh;
                hit.Point = TransformPoint(bh.Point - bh.Normal * r, obb.Center, c, -s, false);
                hit.Normal = TransformVector(bh.Normal, c, -s);
                hit.IsHit = true;
                return true;
            }
            else
            {
                Vector2[] corners =
                {
                    new Vector2(-half.x, -half.y), new Vector2(-half.x, half.y), new Vector2(half.x, half.y),
                    new Vector2(half.x, -half.y)
                };
                float minD = float.MaxValue;
                bool hitAny = false;
                foreach (var corner in corners)
                {
                    if (RaycastCircle(lOrg, lDir, dist, new Circle(corner, r), out RaycastHit ch))
                    {
                        if (ch.Distance < minD)
                        {
                            minD = ch.Distance;
                            hit = ch;
                            hit.Point = TransformPoint(ch.Point - ch.Normal * r, obb.Center, c, -s, false);
                            hit.Normal = TransformVector(ch.Normal, c, -s);
                            hit.IsHit = true;
                            hitAny = true;
                        }
                    }
                }

                return hitAny;
            }
        }

        // Swept SAT Implementation for BoxCast
        public static bool BoxCastOBB(Vector2 origin, Vector2 size, float angle, Vector2 dir, float dist, OBB target,
            out RaycastHit hit)
        {
            hit = new RaycastHit();
            OBB boxA = new OBB(origin, size, angle);
            Vector2[] cA = GetOBBCorners(boxA), cB = GetOBBCorners(target);
            Vector2[] axes =
                { GetAxis(cA[0], cA[1]), GetAxis(cA[1], cA[2]), GetAxis(cB[0], cB[1]), GetAxis(cB[1], cB[2]) };

            float tEnter = 0f, tExit = dist;
            Vector2 normal = Vector2.zero;

            foreach (var axis in axes)
            {
                Project(axis, cA, out float minA, out float maxA);
                Project(axis, cB, out float minB, out float maxB);
                float speedProj = Vector2.Dot(dir, axis);

                if (Mathf.Abs(speedProj) < 1e-6f)
                {
                    if (maxA < minB || maxB < minA) return false;
                }
                else
                {
                    float t0 = (minB - maxA) / speedProj;
                    float t1 = (maxB - minA) / speedProj;
                    if (t0 > t1)
                    {
                        float t = t0;
                        t0 = t1;
                        t1 = t;
                    }

                    if (t0 > tEnter)
                    {
                        tEnter = t0;
                        normal = speedProj < 0 ? axis : -axis;
                    }

                    if (t1 < tExit) tExit = t1;
                    if (tEnter > tExit) return false;
                }
            }

            if (tEnter >= 0 && tEnter <= dist)
            {
                hit.IsHit = true;
                hit.Distance = tEnter;
                hit.Normal = normal;
                hit.Point = origin + dir * tEnter; // Simplified contact point (Center)
                return true;
            }

            return false;
        }

        public static bool BoxCastCircle(Vector2 origin, Vector2 size, float angle, Vector2 dir, float dist,
            Circle target, out RaycastHit hit)
        {
            // 相对运动：Box不动，圆逆向撞Box
            if (CircleCastOBB(target.Center, -dir, dist, target.Radius, new OBB(origin, size, angle), out hit))
            {
                hit.Normal = -hit.Normal;
                Vector2 impact = target.Center + (-dir * hit.Distance);
                hit.Point = impact - (hit.Normal * target.Radius);
                return true;
            }

            return false;
        }

        // ================== 5. 数学辅助 ==================
        private static Vector2 TransformPointInverse(Vector2 p, Vector2 c, float rot)
        {
            float r = -rot * Mathf.Deg2Rad;
            return TransformPoint(p, c, Mathf.Cos(r), Mathf.Sin(r), true);
        }

        private static Vector2 TransformPoint(Vector2 p, Vector2 c, float cos, float sin, bool inv)
        {
            Vector2 d = inv ? (p - c) : p;
            Vector2 r = new Vector2(d.x * cos - d.y * sin, d.x * sin + d.y * cos);
            return inv ? r : (r + c);
        }

        private static Vector2 TransformVector(Vector2 v, float cos, float sin) =>
            new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);

        private static Vector2[] GetOBBCorners(OBB o)
        {
            Vector2[] r = new Vector2[4];
            Vector2 h = o.Size / 2f;
            Vector2[] l =
                { new Vector2(-h.x, -h.y), new Vector2(-h.x, h.y), new Vector2(h.x, h.y), new Vector2(h.x, -h.y) };
            float rad = o.Rotation * Mathf.Deg2Rad, c = Mathf.Cos(rad), s = Mathf.Sin(rad);
            for (int i = 0; i < 4; i++) r[i] = new Vector2(l[i].x * c - l[i].y * s, l[i].x * s + l[i].y * c) + o.Center;
            return r;
        }

        private static Vector2 GetAxis(Vector2 p1, Vector2 p2) => new Vector2(-(p2.y - p1.y), p2.x - p1.x).normalized;

        private static bool IsOverlapOnAxis(Vector2 ax, Vector2[] A, Vector2[] B)
        {
            Project(ax, A, out float minA, out float maxA);
            Project(ax, B, out float minB, out float maxB);
            return !(maxA < minB || minA > maxB);
        }

        private static void Project(Vector2 ax, Vector2[] p, out float min, out float max)
        {
            min = float.MaxValue;
            max = float.MinValue;
            foreach (var v in p)
            {
                float d = Vector2.Dot(v, ax);
                if (d < min) min = d;
                if (d > max) max = d;
            }
        }
    }
}