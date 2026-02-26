using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

    public class InstancePoolManager
    {
        public static InstancePoolManager Instance { get { return Nested.instance; } }
        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly InstancePoolManager instance = new InstancePoolManager();
        }
        
        private Dictionary<string, InstancePool> _pools = new Dictionary<string, InstancePool>();
        private Dictionary<int, InstancePool> _objects = new Dictionary<int, InstancePool>();

        private Transform _cached;

        private InstancePoolManager()
        {
            _cached = GameObjectPool.Instance.Get().transform;
            _cached.gameObject.SetActive(false);

            _cached.name = "Cached Instantiated Objects";
        }


        public void Clear()
        {
            foreach (var pool in _pools.Values)
            {
                pool.Clear();
            }

            _pools.Clear();
            _objects.Clear();
        }

        public async UniTask<GameObject> Get(string name)
        {
            InstancePool pool;
            if (!_pools.TryGetValue(name, out pool))
            {
                pool = new InstancePool(name);
                _pools.Add(name, pool);
            }

            var obj = await pool.InstantiateAsync();
            _objects.Add(obj.GetInstanceID(), pool);
            return obj;
        }

        /*public GameObject GetSync(string name)
        {
            InstancePool pool;
            if (!_pools.TryGetValue(name, out pool))
            {
                pool = new InstancePool(name);
                _pools.Add(name, pool);
            }

            var obj = pool.Instantiate();
            _objects.Add(obj.GetInstanceID(), pool);
            return obj;
        }*/

        public void Recycle(GameObject obj)
        {
            InstancePool pool;
            if (_objects.TryGetValue(obj.GetInstanceID(), out pool))
            {
                pool.Release(obj);
                _objects.Remove(obj.GetInstanceID());

                obj.transform.SetParent(_cached);
            }
        }

        public async UniTask Cache(string name)
        {
            var obj = await Get(name);
            Recycle(obj);
        }
    }