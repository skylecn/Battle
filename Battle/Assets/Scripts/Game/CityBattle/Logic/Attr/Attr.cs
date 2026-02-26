using System.Collections.Generic;

namespace Game.CityBattle.Logic
{
    public class Attr
    {
        private float _baseValue; // 基础数值

        // 缓存修改器列表
        private readonly List<AttrModifier> _modifiers = new List<AttrModifier>();

        // 缓存计算结果
        private float _value;
        private bool _isDirty = true; // 脏标记：为true说明需要重新计算

        // 当且仅当需要读取数值时，才触发计算
        public float Value
        {
            get
            {
                if (_isDirty)
                {
                    _value = CalculateFinalValue();
                    _isDirty = false;
                }

                return _value;
            }
        }

        // 设置基础值
        public float BaseValue
        {
            get => _baseValue;
            set
            {
                _baseValue = value;
                _isDirty = true; // 标记为脏，下次读取需重算
            }
        }

        // 添加修改器
        public void AddModifier(AttrModifier mod)
        {
            _isDirty = true;
            _modifiers.Add(mod);
            _modifiers.Sort(CompareModifierOrder); // 确保计算顺序正确
        }

        // 移除修改器
        public bool RemoveModifier(AttrModifier mod)
        {
            if (_modifiers.Remove(mod))
            {
                _isDirty = true;
                return true;
            }

            return false;
        }

        // 移除来自特定来源的所有修改器（比如脱下一件装备）
        public bool RemoveAllModifiersFromSource(object source)
        {
            int numRemoved = _modifiers.RemoveAll(mod => mod.source == source);
            if (numRemoved > 0)
            {
                _isDirty = true;
                return true;
            }

            return false;
        }

        private int CompareModifierOrder(AttrModifier a, AttrModifier b)
        {
            if (a.order < b.order) return -1;
            else if (a.order > b.order) return 1;
            return 0; // Flat First, then Percent Add, then Mult
        }

        // 核心计算逻辑
        private float CalculateFinalValue()
        {
            float finalValue = BaseValue;
            float sumPercentAdd = 0;

            for (int i = 0; i < _modifiers.Count; i++)
            {
                AttrModifier mod = _modifiers[i];

                if (mod.type == AttrModType.Flat)
                {
                    finalValue += mod.value;
                }
                else if (mod.type == AttrModType.PercentAdd)
                {
                    sumPercentAdd += mod.value; // 先累加所有百分比
                    // 比如 0.1 + 0.05 = 0.15
                }
                else if (mod.type == AttrModType.PercentMult)
                {
                    // 等PercentAdd算完后再乘
                    // 这里的逻辑稍微复杂，通常需要多一次循环或特定顺序
                    // 简化写法：假设Mult都在最后
                }
            }

            // 应用加算百分比 (Base + Flat) * (1 + Total%)
            finalValue *= 1 + sumPercentAdd;

            // 应用独立乘区
            for (int i = 0; i < _modifiers.Count; i++)
            {
                if (_modifiers[i].type == AttrModType.PercentMult)
                {
                    finalValue *= (1 + _modifiers[i].value);
                }
            }

            return (float)System.Math.Round(finalValue, 4); // 避免浮点误差
        }
    }
}