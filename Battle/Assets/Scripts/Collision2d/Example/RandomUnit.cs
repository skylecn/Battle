using UnityEngine;

namespace Collision2d.Example
{
    public class RandomUnit : MonoBehaviour
    {
        private Vector2 velocity;
        private float rotationSpeed;
        private float boundary = 25f; // 50x50 的空间，半径是 25

        void Start()
        {
            // 随机速度
            velocity = Random.insideUnitCircle.normalized * Random.Range(2f, 5f);
            // 随机旋转速度 (如果是 OBB)
            rotationSpeed = Random.Range(-90f, 90f);
        }

        void Update()
        {
            // 1. 移动
            transform.Translate(velocity * Time.deltaTime, Space.World);

            // 2. 旋转 (仅为了演示 OBB 检测)
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

            // 3. 边界反弹逻辑 (限制在 -25 到 25 之间)
            Vector2 pos = transform.position;

            if (pos.x > boundary)
            {
                pos.x = boundary;
                velocity.x *= -1;
            }
            else if (pos.x < -boundary)
            {
                pos.x = -boundary;
                velocity.x *= -1;
            }

            if (pos.y > boundary)
            {
                pos.y = boundary;
                velocity.y *= -1;
            }
            else if (pos.y < -boundary)
            {
                pos.y = -boundary;
                velocity.y *= -1;
            }

            transform.position = pos;
        }
    }
}