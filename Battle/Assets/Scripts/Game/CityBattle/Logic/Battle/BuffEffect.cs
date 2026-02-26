using System.Collections;
using System.Collections.Generic;
using cfg;
using UnityEngine;

namespace Game.CityBattle.Logic
{
    public abstract class BuffEffect
    {
        protected BuffEffectData _data;

        public BuffEffect(BuffEffectData data)
        {
            _data = data;
        }

        // Buff 开始时执行（例如：增加属性，修改标签）
        public virtual void OnApply(Character owner, BuffInstance instance){}
        
        // Buff 结束时执行（例如：还原属性，移除标签）
        public virtual void OnRemove(Character owner, BuffInstance instance){}

        // Buff 定期触发（例如：DOT 伤害）
        public virtual void OnTick(Character owner, BuffInstance instance) { }
    }
}
