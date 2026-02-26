
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using YooAsset;

    public class InstancePool
    {
        public string name { protected set; get; }

        protected Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();
        protected Queue<GameObject> _frees = new Queue<GameObject>();

        public AssetHandle resHandle { protected set; get; }

        public GameObject prefab { get { return resHandle.AssetObject as GameObject; } }

        public InstancePool(string name)
        {
            this.name = name;
        }

        public async UniTask<GameObject> InstantiateAsync()
        {
            GameObject obj;

            if (_frees.Count > 0)
            {
                obj = _frees.Dequeue();
            }
            else
            {
                resHandle = YooAssets.LoadAssetAsync(name);
                await resHandle;
                obj = GameObject.Instantiate(prefab);
                /*var op = Addressables.InstantiateAsync(name);
                await op;
                obj = op.Result;*/
                _objects.Add(obj.GetInstanceID(), obj);
            }

            return obj;
        }

        /*public GameObject Instantiate()
        {
            GameObject obj;

            if (_frees.Count > 0)
            {
                obj = _frees.Dequeue();
            }
            else
            {
                var op = Addressables.InstantiateAsync(name);
                op.WaitForCompletion();
                obj = op.Result;
                _objects.Add(obj.GetInstanceID(), obj);
            }

            return obj;
        }*/

        public void Release(GameObject obj)
        {
            if (_objects.ContainsKey(obj.GetInstanceID()))
            {
                _frees.Enqueue(obj);
            }
        }

        public void Clear()
        {
            foreach (var obj in _objects.Values)
            {
                GameObject.Destroy(obj);
            }

            _objects.Clear();
            _frees.Clear();

            resHandle.Release();
            resHandle = null;
        }
    }