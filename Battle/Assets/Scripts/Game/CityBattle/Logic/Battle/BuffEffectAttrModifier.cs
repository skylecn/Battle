using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg;

namespace Game.CityBattle.Logic
{   
    public class BuffEffectAttrModifier : BuffEffect
    {
        private AttrModifier _modifier;
        public BuffEffectAttrModifier(BuffEffectData data) : base(data)
        {
        }

        public override void OnApply(Character owner, BuffInstance instance)
        {
            var attrType = (AttrType)_data.Param1;
            var modType = (AttrModType)_data.Param2;
            var value = _data.Param3;

            _modifier = new AttrModifier(value, modType, 0, instance);
            owner.attrs.AddAttrModifier(attrType, _modifier);
        }

        public override void OnRemove(Character owner, BuffInstance instance)
        {
            var attrType = (AttrType)_data.Param1;
            owner.attrs.RemoveAttrModifier(attrType, _modifier);
        }
    }
}
