using System;
using System.Collections;
using System.Collections.Generic;
using cfg;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.CityBattle.Logic
{

    public class SkillInstance
    {
        public SkillData conf;
        public Character caster;
        public Character target;
        public Vector2 targetPos;
        public bool isEnd { get; private set; }
        private float duration;
        private int hitCount;
        private Action attackEnd;
        private float _lastTime;

        private List<SkillAction> _skillEventList = new List<SkillAction>();

        private List<SkillAction> _hitEventList = new List<SkillAction>();

        // 技能冷却中
        public bool CD
        {
            get { return BattleWorld.BattleTime - _lastTime < conf.Cooldown; }
            set { _lastTime = BattleWorld.BattleTime; }
        }

        public float CDTime => conf.Cooldown - (BattleWorld.BattleTime - _lastTime);

        public SkillInstance(Character self, SkillData conf)
        {
            this.caster = self;
            this.conf = conf;
            isEnd = true;
            attackEnd = null;

            foreach (var skillEvent in conf.SkillEvent)
            {
                _skillEventList.Add(CfgData.GetSkillAction(skillEvent));
            }

            foreach (var hitEvent in conf.HitEvent)
            {
                _hitEventList.Add(CfgData.GetSkillAction(hitEvent));
            }

            // 初始有CD
            _lastTime = BattleWorld.BattleTime;
        }

        public void Init(Character target, Vector2 targetPos, Action end = null)
        {
            duration = 0;
            hitCount = 0;
            isEnd = false;
            this.target = target;
            this.targetPos = targetPos;
            attackEnd = end;
            _lastTime = BattleWorld.BattleTime;

            caster.PlayAnimation(conf.Animation);
        }

        // 是否在攻击范围
        public bool InAttackRange(Vector3 targetPos, float targetRadius)
        {
            return Vector3.Distance(caster.position, targetPos) <= targetRadius + conf.Distance;
        }

        public void Update(float time)
        {
            if (isEnd) return;

            duration += time * conf.Speed;

            if (hitCount < _skillEventList.Count && duration * 1000 > _skillEventList[hitCount].Time)
            {
                OnEvent(_skillEventList[hitCount]);
                hitCount++;
            }

            if (duration > conf.Duration)
            {
                End();
            }
        }

        public void End(bool force = false)
        {
            isEnd = true;
            if (!force){
                attackEnd?.Invoke();
                caster.OnEndSkill(this);
            }

            attackEnd = null;
            caster.PlayIdleAnim();
        }

        void OnEvent(SkillAction eventId)
        {
            ExcuteAction(eventId);
        }

        void ExecHitAction(Character target)
        {
            foreach (var hitEvent in _hitEventList)
            {
                ExecHitAction(target, hitEvent);
            }
        }

        public void ExcuteAction(SkillAction actionId)
        {
            switch (actionId.ActionType)
            {
                case (int)SkillActionType.Damage:
                {
                    if (actionId.Bullet != 0)
                    {
                        BattleWorld.Instance.CreateBullet(this, target, targetPos, actionId);
                    }
                    else
                    {
                        ExecDamage(actionId);
                    }
                }
                    break;
                case (int)SkillActionType.AddBuff:
                    if (actionId.Bullet != 0)
                    {
                        BattleWorld.Instance.CreateBullet(this, target, targetPos, actionId);
                    }
                    else
                    {
                        ExecAddBuff(actionId);
                    }
                    break;
                case (int)SkillActionType.ClearPositiveBuffs:
                    if (actionId.Bullet != 0)
                    {
                        BattleWorld.Instance.CreateBullet(this, target, targetPos, actionId);
                    }
                    else
                    {
                        ExecClearBuff(actionId, BuffType.Positive);
                    }
                    break;
                case (int)SkillActionType.ClearNegativeBuffs:
                    if (actionId.Bullet != 0)
                    {
                        BattleWorld.Instance.CreateBullet(this, target, targetPos, actionId);
                    }
                    else
                    {
                        ExecClearBuff(actionId, BuffType.Negative);
                    }
                    break;
                case (int)SkillActionType.Effect:
                    caster.OnReleaseEffect(actionId.Effect, conf.Speed);
                    break;
                case (int)SkillActionType.Audio:
                    caster.OnPlayAudio(actionId.Audio);
                    break;
            }
        }

        public void ExecDamage(SkillAction meta)
        {
            switch (meta.TargetType)
            {
                case (int)SkillActionTargetType.One:
                {
                    float damage = BattleEngine.CalculateDamage(caster, target, meta);
                    var damageInfo = new DamageInfo(caster, damage, false);
                    target.TakeDamage(damageInfo);
                    ExecHitAction(target);
                    caster.DoDamage(damageInfo);
                    caster.DoHit(damageInfo);
                    break;
                }
                case (int)SkillActionTargetType.Multiple:
                    {
                        Vector2 center = AreaCenter(meta);
                        var list = BattleWorld.Instance.OverlapSphere(center, meta.AreaRadius,
                            LayerUtil.LayerMaskNot(caster.layerType), caster.camp, false);
                        foreach (var entity in list)
                        {
                            float damage = BattleEngine.CalculateDamage(caster, entity, meta);
                            var damageInfo = new DamageInfo(caster, damage, false);
                            entity.TakeDamage(damageInfo);
                            ExecHitAction(entity, meta);
                            caster.DoDamage(damageInfo);
                            caster.DoHit(damageInfo);
                            //entity.OnEffect(meta.Effect);
                            //entity.OnHitEffect();
                        }

                        break;
                    }
            }
        }



        public void ExecAddBuff(SkillAction meta){
            switch (meta.TargetType)
            {
                case (int)SkillActionTargetType.One:
                {
                    target.AddBuff(meta.BuffId, caster);
                    break;
                }
                case (int)SkillActionTargetType.Multiple:
                {
                    bool sameCamp = conf.TargetCamp == (int)BattleCamp.Friend;
                    int layerMask = sameCamp ? LayerUtil.LayerMask(caster.layerType) : LayerUtil.LayerMaskNot(caster.layerType);
                    Vector2 center = AreaCenter(meta);
                    var list = BattleWorld.Instance.OverlapSphere(center, meta.AreaRadius,
                        layerMask, caster.camp, sameCamp);
                    foreach (var entity in list)
                    {
                        entity.AddBuff(meta.BuffId, caster);
                    }
                    break;
                }
            }
        }

        public void ExecClearBuff(SkillAction meta, BuffType type){
            switch (meta.TargetType)
            {
                case (int)SkillActionTargetType.One:
                {
                    target.ClearBuffs(type);
                    break;
                }
                case (int)SkillActionTargetType.Multiple:
                {
                    bool sameCamp = conf.TargetCamp == (int)BattleCamp.Friend;
                    int layerMask = sameCamp ? LayerUtil.LayerMask(caster.layerType) : LayerUtil.LayerMaskNot(caster.layerType);
                    Vector2 center = AreaCenter(meta);
                    var list = BattleWorld.Instance.OverlapSphere(center, meta.AreaRadius,
                        layerMask, caster.camp, sameCamp);
                    foreach (var entity in list)
                    {
                        entity.ClearBuffs(type);
                    }
                    break;
                }
            }
        }

        void ExecHitAction(Character target, SkillAction meta)
        {
            switch (meta.ActionType)
            {
                case (int)SkillActionType.Damage:
                {
                    float damage = BattleEngine.CalculateDamage(caster, target, meta);
                    var damageInfo = new DamageInfo(caster, damage, false); 
                    target.TakeDamage(damageInfo);
                    caster.DoDamage(damageInfo);
                    break;
                }
                case (int)SkillActionType.AddBuff:
                {
                    target.AddBuff(meta.BuffId, caster);
                    break;
                }
                case (int)SkillActionType.ClearPositiveBuffs:
                {
                    target.ClearBuffs(BuffType.Positive);
                    break;
                }
                case (int)SkillActionType.ClearNegativeBuffs:
                {
                    target.ClearBuffs(BuffType.Negative);
                    break;
                }
            }
        }

        private Vector2 AreaCenter(SkillAction meta)
        {
            return meta.AreaCenter == (int)SkillDamageAreaCenter.Caster
                                    ? caster.position
                                    : target?.position ?? targetPos;
        }

        float GetAttack()
        {
            return caster.attack;
        }
    }
}