using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class GameObjectPool
    {
        protected Queue<GameObject> _frees = new Queue<GameObject>();
        protected Transform _cached;
        
        public static GameObjectPool Instance { get { return Nested.instance; } }
        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly GameObjectPool instance = new GameObjectPool();
        }

        private GameObjectPool()
        {
            _cached = new GameObject().transform;
            _cached.gameObject.SetActive(false);

            _cached.name = "Cached Instantiated Objects";
        }

        public void Clear()
        {
            foreach (var obj in _frees)
            {
                GameObject.Destroy(obj);
            }

            _frees.Clear();
        }

        public GameObject Get()
        {
            if (_frees.Count == 0)
            {
                var obj = new GameObject();
                return obj;
            }
            else
            {
                GameObject gameObject = _frees.Dequeue();
                if (gameObject == null)
                {
                    return null;
                }

                gameObject.transform.SetParent(null);
                gameObject.transform.localPosition = Vector3.zero;
                gameObject.transform.localRotation = Quaternion.identity;
                gameObject.transform.localScale = Vector3.one;

                return gameObject;
            }
        }

        public void Recycle(GameObject obj)
        {
            obj.transform.SetParent(_cached);
            _frees.Enqueue(obj);
        }
    }
