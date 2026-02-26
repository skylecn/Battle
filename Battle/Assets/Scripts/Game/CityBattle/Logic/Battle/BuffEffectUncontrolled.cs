using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg;

namespace Game.CityBattle.Logic
{
    public class BuffEffectUncontrolled : BuffEffect
    {
        public BuffEffectUncontrolled(BuffEffectData data) : base(data)
        {
        }

        public override void OnApply(Character owner, BuffInstance instance)
        {
            owner.StartUncontrolled();
        }

        public override void OnRemove(Character owner, BuffInstance instance)
        {
            owner.EndUncontrolled();
        }
    }
}
