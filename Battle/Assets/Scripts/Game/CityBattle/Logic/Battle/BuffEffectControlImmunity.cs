using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg;
namespace Game.CityBattle.Logic
{
    public class BuffEffectControlImmunity : BuffEffect
        {   
        public BuffEffectControlImmunity(BuffEffectData data) : base(data)
        {
        }

        public override void OnApply(Character owner, BuffInstance instance)
        {
            owner.AddTag(StateTag.ControlImmunity);
        }

        public override void OnRemove(Character owner, BuffInstance instance)
        {
            owner.RemoveTag(StateTag.ControlImmunity);
        }
    }
}
