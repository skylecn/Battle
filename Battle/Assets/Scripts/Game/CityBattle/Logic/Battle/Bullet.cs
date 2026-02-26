using System.Collections;
using System.Collections.Generic;
using cfg;
using Cysharp.Threading.Tasks;
using Game.Base.Utils;
using Game.CityBattle.Entity;
using Game.CityBattle.Logic;
using UnityEngine;

namespace Game.CityBattle.Logic
{
    public class Bullet
    {
        public int instanceId { get; private set; }
        public BulletData bulletConf { get; private set; }
        private SkillInstance _skillInstance;
        private SkillAction _skillEvent;

        public bool isDestroy { get; private set; }
        private Vector2 targetPos;
        private Character target;
        private float _curTime;
        
        private Vector2 _position;
        public Vector2 position
        {
            get => _position;
            set
            {
                _position = value;
                BattleController.RecvBulletMovePos?.Invoke(BattleWorld.BattleTime, instanceId, _position);
            }
        }

        public void Init(int instanceId, SkillInstance skillInstance, Character target, Vector2 targetPos, SkillAction eventD)
        {
            this.instanceId = instanceId;
            _skillInstance = skillInstance;
            _skillEvent = eventD;
            this.bulletConf = CfgData.GetBullet(_skillEvent.Bullet);
            this.target = target;
            this.targetPos = targetPos;
            _position = skillInstance.caster.position;
            isDestroy = false;
            _curTime = 0;
        }

        public void Update(float time)
        {
            if (isDestroy) return;

            if ((target != null && (target.position-position).sqrMagnitude>target.radius2) ||
                targetPos!=position)
            {
                var tarPos = target?.position ?? targetPos;
                float frameDistance = time * bulletConf.Speed / 100f;
                float dis = (tarPos - position).magnitude;
                if (dis > 0)
                {
                    float interpolationValue = frameDistance / dis;
                    position = Vector3.Lerp(position, tarPos, interpolationValue);
                }
                else
                {
                    OnHit();
                }

                _curTime += time;
            }
            else
            {
                OnHit();
            }
        }

        void OnHit()
        {
            isDestroy = true;
            _skillInstance.ExecDamage(_skillEvent);
        }
    }
}