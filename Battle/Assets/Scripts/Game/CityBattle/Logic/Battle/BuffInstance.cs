using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg;

namespace Game.CityBattle.Logic
{
    public partial class BuffInstance
    {
        public BuffData Data { get; private set; }
        
        public Character caster { get; private set; }
        public float casterAttack { get; private set; }
        
        public float duration;
        private float _nextTickTime;
        public int CurrentStacks;
        private List<BuffEffect> _effects = new List<BuffEffect>();
        public BuffInstance(BuffData data, Character _caster)
        {
            Data = data;
            caster = _caster;
            casterAttack = _caster.attack;
            duration = 0f;
            CurrentStacks = 1;
            _nextTickTime = 0f;
            foreach (var effect in Data.Effects){
                _effects.Add(CreateEffect(effect));
            }
        }

        BuffEffect CreateEffect(int effectId)
        {
            var effectData = CfgData.GetBuffEffect(effectId);
            switch (effectData.EffectType)
            {
                case (int)BuffEffectType.Attr:
                    return new BuffEffectAttrModifier(effectData);
                case (int)BuffEffectType.Damage:
                    return new BuffEffectDamage(effectData);
                case (int)BuffEffectType.Uncontrolled:
                    return new BuffEffectUncontrolled(effectData);
                case (int)BuffEffectType.DamageImmunity:
                    return new BuffEffectDamageImmunity(effectData);
                case (int)BuffEffectType.ControlImmunity:
                    return new BuffEffectControlImmunity(effectData);
                default:
                    OutputLogger.Error($"Unknown buff({Data.Id}) effect type: {effectData.EffectType}");
                    return null;
            }
        }

        public void OnApply(Character owner)
        {
            foreach (var effect in _effects)
                effect.OnApply(owner, this);
        }

        public void OnRemove(Character owner)
        {
            foreach (var effect in _effects)
                effect.OnRemove(owner, this);
        }

        public void OnTick(Character owner)
        {
            if (Data.TickInterval > 0 && duration >= _nextTickTime)
            {
                foreach (var effect in _effects)
                    effect.OnTick(owner, this);
                _nextTickTime += Data.TickInterval;
            }
        }
    }
}