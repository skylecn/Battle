using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg;
namespace Game.CityBattle.Logic
{
    public class BuffEffectDamageImmunity : BuffEffect
    {
        public BuffEffectDamageImmunity(BuffEffectData data) : base(data)
        {
        }

        public override void OnApply(Character owner, BuffInstance instance)
        {
            owner.AddTag(StateTag.DamageImmunity);
        }

        public override void OnRemove(Character owner, BuffInstance instance)
        {
            owner.RemoveTag(StateTag.DamageImmunity);
        }
    }
}

