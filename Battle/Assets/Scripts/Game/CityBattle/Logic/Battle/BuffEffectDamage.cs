using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg;
namespace Game.CityBattle.Logic
{
    public class BuffEffectDamage : BuffEffect
    {   
        public BuffEffectDamage(BuffEffectData data) : base(data)
        {
        }

        public override void OnTick(Character owner, BuffInstance instance)
        {
            float damage = BattleEngine.CalculateDamage(instance.casterAttack, owner);
            owner.TakeDamage(new DamageInfo(instance.caster, damage, false));
        }
    }
}
