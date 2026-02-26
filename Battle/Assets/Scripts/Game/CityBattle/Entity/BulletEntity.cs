using System.Collections;
using System.Collections.Generic;
using cfg;
using Cysharp.Threading.Tasks;
using Game.Base.Utils;
using UnityEngine;

namespace Game.CityBattle.Entity
{
    public class BulletEntity : MapEntity
    {
        BulletData _bulletData;
        private int _targetId;
        private Vector2 _targetPos;

        private Vector3 _startOffset;
        private Vector3 _targetOffset;

        public void Init(float logicTime, int instanceId, int bulletId, Vector2 pos, int sourceId, int targetId,
            Vector2 targetPosition)
        {
            Id = instanceId;
            _bulletData = CfgData.GetBullet(bulletId);
            var source = EntityManager.Instance.GetCharacter(sourceId);
            if (source != null)
            {
                _startOffset = source.GetSkeletonPoint(_bulletData.SourceNode).position - source.transform.position + _bulletData.SourceOffset.ToVec3();
            }
            var target = EntityManager.Instance.GetCharacter(targetId);
            if (target != null)
            {
                _targetOffset = target.GetSkeletonPoint(_bulletData.TargetNode).position - target.transform.position + _bulletData.TargetOffset.ToVec3();
            }

            _targetId = targetId;
            _targetPos = targetPosition;
            InitPos(logicTime, pos);

            LoadRes(_bulletData.Res).Forget();
        }

        protected override void SetPos(Vector2 position)
        {
            this.position = new Vector3(position.x, 0, position.y) + _startOffset;
        }
    }
}