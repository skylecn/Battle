using UnityEngine;

namespace Game.Base.Utils
{
    public class BezierHelper
    {
        public Vector3 _s;
        public Vector3 _m;
        public Vector3 _e;
        public float _t;

        public BezierHelper(Vector3 startPos, Vector3 middlePos, Vector3 tarPos)
        {
            _s = startPos;
            _m = middlePos;
            _e = tarPos;
        }

        public void ResetEndPos(Vector3 middlePos, Vector3 tarPos)
        {
            _m = middlePos;
            _e = tarPos;
        }
        
        public void ResetEndPos(Vector3 tarPos)
        {
            _e = tarPos;
        }

        public Vector3 GetPoint(float t)
        {
            _t = t;
            float u = 1 - t;
            Vector3 point = u * u * _s + 2 * u * t * _m + t * t * _e;
            return point;
        }
    }
}