using UnityEngine;

namespace Collision2d.Example
{
    public class DemoSpawner : MonoBehaviour
    {
        public int count = 100;
        public float spawnRadius = 20f;

        void Start()
        {
            // 确保物理管理器存在
            if (FindObjectOfType<MyPhysicsManager>() == null)
            {
                GameObject manager = new GameObject("PhysicsManager");
                manager.AddComponent<MyPhysicsManager>();
            }

            for (int i = 0; i < count; i++)
            {
                SpawnRandomUnit(i);
            }
        }

        void SpawnRandomUnit(int index)
        {
            // 创建空物体
            GameObject go = new GameObject($"Unit_{index}");
            go.transform.position = new Vector2(Random.Range(-spawnRadius, spawnRadius),
                Random.Range(-spawnRadius, spawnRadius));
            go.transform.parent = this.transform;

            // 添加移动脚本
            go.AddComponent<RandomUnit>();

            // 添加物理身体
            MyPhysicsBody body = go.AddComponent<MyPhysicsBody>();

            // 随机形状：50% 是圆，50% 是旋转矩形
            if (Random.value > 0.5f)
            {
                body.type = CustomColliderType.Circle;
                body.radius = Random.Range(0.5f, 1.0f);
            }
            else
            {
                body.type = CustomColliderType.OBB;
                body.size = new Vector2(Random.Range(0.8f, 2f), Random.Range(0.8f, 2f));
                // 随机初始角度
                go.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
            }
        }

        // 画出边界框
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(50, 50, 0));
        }
    }
}