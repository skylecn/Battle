using System.Collections;
using System.Collections.Generic;
using cfg;
using UnityEngine;

namespace Game.CityBattle.Logic
{
    public class PassiveSkillInstance
    {
        public PassiveSkillData conf;

        private float _lastTriggerTime;

        public bool CD{
            get { return BattleWorld.BattleTime - _lastTriggerTime < conf.Cooldown; }
            set { _lastTriggerTime = BattleWorld.BattleTime; }
        }

        public PassiveSkillInstance(PassiveSkillData conf)
        {
            this.conf = conf;
            _lastTriggerTime = 0;
        }

        public void ExecuteActions(Character owner, DamageInfo damageInfo){
            if (CD) return;
            if (Random.value >= conf.Ratio) return;   // 概率触发

            foreach (var skillEventId in conf.SkillEvent)
            {
                ExecuteAction(owner, skillEventId, damageInfo);
            }
            CD = true;
        }

        void ExecuteAction(Character owner, int skillEventId, DamageInfo damageInfo){
            var skillEvent = CfgData.GetSkillAction(skillEventId);
            switch (skillEvent.ActionType)
            {
                case (int)SkillActionType.AddBuff:
                    owner.AddBuff(skillEvent.BuffId, owner);
                    break;
                case (int)SkillActionType.LifeSteal:
                    ExecuteLifeStealAction(owner, skillEvent, damageInfo);
                    break;
                default:
                    OutputLogger.Error($"Unknown passive skill {conf.Id} action type: {skillEvent.ActionType}");
                    break;
            }
        }

        void ExecuteLifeStealAction(Character owner, SkillAction skillEvent, DamageInfo damageInfo){
            float lifeSteal = owner.attrs.GetAttrValue(AttrType.LifeSteal);
            if (lifeSteal <= 0) return;
            float damage = damageInfo.damage;
            float lifeStealAmount = damage * lifeSteal*skillEvent.Ratio;
            owner.Heal(lifeStealAmount);
        }
    }
}
