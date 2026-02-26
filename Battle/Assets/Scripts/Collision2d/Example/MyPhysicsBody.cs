using UnityEngine;
using UnityEngine.Events;

namespace Collision2d.Example
{
    public enum CustomColliderType
    {
        Circle,
        OBB,
        AABB
    }

    public class MyPhysicsBody : MonoBehaviour
    {
        [Header("Settings")] public CustomColliderType type = CustomColliderType.OBB;
        public bool isStatic = false;
        public Vector2 size = Vector2.one;
        public float radius = 0.5f;

        [Header("Events")] public UnityEvent<MyPhysicsBody> onEnter;
        public UnityEvent<MyPhysicsBody> onStay;
        public UnityEvent<MyPhysicsBody> onExit;

        public PhysicsBody InternalBody { get; private set; }

        private void Start() => MyPhysicsManager.Instance.RegisterBody(this);

        private void OnDestroy()
        {
            if (MyPhysicsManager.Instance) MyPhysicsManager.Instance.UnregisterBody(this);
        }

        public void Initialize(int id)
        {
            Shape shape = null;
            if (type == CustomColliderType.Circle) shape = new Circle(transform.position, radius);
            else if (type == CustomColliderType.AABB) shape = new AABB(transform.position, size);
            else shape = new OBB(transform.position, size, transform.eulerAngles.z);

            InternalBody = new PhysicsBody(id, shape, isStatic, gameObject.layer, this);

            InternalBody.OnCollisionEnter += (o) => onEnter?.Invoke(o.UserData as MyPhysicsBody);
            InternalBody.OnCollisionStay += (o) => onStay?.Invoke(o.UserData as MyPhysicsBody);
            InternalBody.OnCollisionExit += (o) => onExit?.Invoke(o.UserData as MyPhysicsBody);
        }

        private void Update()
        {
            if (InternalBody == null) return;
            InternalBody.LayerIndex = gameObject.layer;
            InternalBody.ShapeData.Center = transform.position;
            if (InternalBody.ShapeData is OBB o)
            {
                o.Size = size;
                o.Rotation = transform.eulerAngles.z;
            }
            else if (InternalBody.ShapeData is Circle c)
            {
                c.Radius = radius;
            }
            else if (InternalBody.ShapeData is AABB a)
            {
                a.Size = size;
            }
        }

        private void OnDrawGizmos()
        {
            bool hit = InternalBody != null && InternalBody.CurrentCollisions.Count > 0;
            Gizmos.color = hit ? Color.red : Color.green;
            if (type == CustomColliderType.Circle) Gizmos.DrawWireSphere(transform.position, radius);
            else
            {
                Matrix4x4 old = Gizmos.matrix;
                Quaternion r = type == CustomColliderType.OBB ? transform.rotation : Quaternion.identity;
                Gizmos.matrix = Matrix4x4.TRS(transform.position, r, Vector3.one);
                Gizmos.DrawWireCube(Vector3.zero, size);
                Gizmos.matrix = old;
            }
        }
    }
}