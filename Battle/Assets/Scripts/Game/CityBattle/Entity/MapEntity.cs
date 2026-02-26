using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;


namespace Game.CityBattle.Entity
{
    public class MapEntityType
    {
        public const byte Character = 1;
        public const byte Bullet = 2;
    }

    public struct LogicPosStep
    {
        public Vector2 position; // 逻辑帧结束时的位置
        public float timestamp; // 逻辑时间（秒）
    }

    public class MapEntity
    {
        public int Id { get; set; }
        public virtual int type { get; }
        public Transform transform { get; protected set; }

        public Animator animator { get; protected set; }

        public MeshRenderer meshRenderer { get; protected set; }

        public GameObject gameObject { get; protected set; }

        public string name
        {
            get { return gameObject.name; }
            protected set { gameObject.name = value; }
        }

        public virtual Vector3 position
        {
            get { return transform.position; }
            set { transform.position = value; }
        }

        public Vector3 scale
        {
            get { return transform.localScale; }
            set { transform.localScale = value; }
        }

        public bool active
        {
            set { gameObject.SetActive(value); }
            get { return gameObject.activeSelf; }
        }

        protected LogicPosStep _prevStep;
        protected LogicPosStep _curStep;

        // 骨骼挂点
        protected Dictionary<string, Transform> skeletonPoints = new Dictionary<string, Transform>();

        public Transform GetSkeletonPoint(string name)
        {
            if (skeletonPoints.TryGetValue(name, out var point))
            {
                return point;
            }

            return transform;
        }

        public Transform MidSkeleton => GetSkeletonPoint("Middle");

        public Transform TopSkeleton => GetSkeletonPoint("Top");

        public Transform BottomSkeleton => GetSkeletonPoint("Bottom");

        public Transform FxSkeleton => GetSkeletonPoint("fx");

        protected GameObject _resInstance;

        protected Dictionary<int, VFXInstance> _effectInstances = new Dictionary<int, VFXInstance>();

        public MapEntity()
        {
            this.gameObject = GameObjectPool.Instance.Get();
            name = "MapEntity";
            this.transform = this.gameObject.transform;
            this.active = true;
        }

        public void Release()
        {
            OnRecycle();
            if (gameObject != null)
            {
                GameObjectPool.Instance.Recycle(gameObject);
            }
        }

        protected virtual void OnRecycle()
        {
            UnloadRes();
        }

        public async UniTask Init(int instanceId, string resId)
        {
            Id = instanceId;
            await LoadRes(resId);
        }

        protected async UniTask LoadRes(string ResName)
        {
            _resInstance = await InstancePoolManager.Instance.Get(ResName);
            _resInstance.transform.SetParent(transform);
            _resInstance.transform.localPosition = Vector3.zero;
            _resInstance.transform.localRotation = Quaternion.identity;
            _resInstance.transform.localScale = Vector3.one;

            animator = _resInstance.GetComponentInChildren<Animator>();
            meshRenderer = _resInstance.GetComponentInChildren<MeshRenderer>();

            InitSkeleton();
        }

        protected void InitSkeleton()
        {
            var transforms = gameObject.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < transforms.Length; i++)
            {
                skeletonPoints[transforms[i].name] = transforms[i];
            }
        }

        private void UnloadRes()
        {
            if (_resInstance != null)
            {
                InstancePoolManager.Instance.Recycle(_resInstance);
                _resInstance = null;
            }
        }

        protected virtual void SetPos(Vector2 position)
        {
            this.position = new Vector3(position.x, 0, position.y);
        }

        public void InitPos(float time, Vector2 pos)
        {
            this.SetPos(pos);
            _curStep = new LogicPosStep { position = pos, timestamp = time };
        }

        public void MovePos(float time, Vector2 pos)
        {
            _prevStep = _curStep;
            _curStep = new LogicPosStep { position = pos, timestamp = time };
        }

        protected void UpdatePos(float time)
        {
            if (_prevStep.timestamp == 0)
            {
                return;
            }

            float logicDelta = _curStep.timestamp - _prevStep.timestamp;
            if (logicDelta <= 0)
            {
                return;
            }

            float renderTime = CityBattleManager.BattleTime; // 当前时间
            float alpha = (renderTime - _curStep.timestamp) / logicDelta;
            alpha = Mathf.Clamp01(alpha);
            var pos = Vector2.Lerp(_prevStep.position, _curStep.position, alpha);
            SetPos(pos);
        }

        public void SetDirection(float direction)
        {
            transform.rotation = Quaternion.Euler(0, direction, 0);
        }

        public void SetDirection(Vector2 direction)
        {
            // direction = new Vector2(-direction.x, direction.y);
            // spineController.SetDirection(direction);
            transform.forward = new Vector3(direction.x, 0, direction.y);
        }

        public void PlayAnimation(string animationName)
        {
            animator?.Play(animationName);
        }

        public void PlayAnimation(int animationId)
        {
            PlayAnimation(Constant.GetAnimName(animationId));
        }

        public virtual void Update(float time)
        {
            UpdatePos(time);
        }

        public async UniTaskVoid OnEffect(int effectId, int effectInstanceId)
        {
            var inst = await VFXManager.instance.CreateEffect(effectId, transform, GetSkeletonPoint);
            if(inst==null){
                return;
            }
            if(effectInstanceId>0){
                _effectInstances[effectInstanceId] = inst;
            }
        }

        public void OnRemoveEffect(int effectInstanceId)
        {
            if(_effectInstances.TryGetValue(effectInstanceId, out var inst)){
                VFXManager.instance.RemoveEffect(inst);
                _effectInstances.Remove(effectInstanceId);
            }
        }
    }
}