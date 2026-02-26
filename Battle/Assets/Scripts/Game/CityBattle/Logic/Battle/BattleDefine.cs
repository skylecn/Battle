using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CityBattle.Logic
{
    /// <summary>
    /// 状态标签
    /// </summary>
    public enum StateTag
    {
        DamageImmunity = 1,   // 免疫伤害
        ControlImmunity = 2,   // 免疫控制
    }


    // 角色类型
    public enum CharacterType
    {
        Character = 1,
        Building = 2,
    }

    // 阵营
    public enum BattleCamp
    {
        Friend = 1,
        Enemy = 2,
    }

    public enum SkillTargetType
    {
        None = 0,
        Caster = 1,   // 自己
        Friend = 2, // 友军
        Enemy = 3, // 敌军
        Pos = 4,   // 位置
    }

    public enum SkillActionType
    {
        Damage = 1,    // 伤害
        Heal = 2,    // 治疗
        AddBuff = 3,      // 添加Buff
        ClearPositiveBuffs = 4,  // 清除正面Buff
        ClearNegativeBuffs = 5,  // 清除负面Buff
        LifeSteal = 6,  // 生命偷取

        Effect = 100,    // 特效
        Audio = 101,     // 音效
    }

    public enum SkillActionTargetType
    {
        One = 1,
        Multiple = 2,
    }

    public enum SkillDamageAreaCenter
    {
        Caster = 1,
        Target = 2,
    }

    public enum BulletTrackType{
        Line = 1,
        Parabola = 2,
    }


    public enum BuffType
    {
        Positive = 1, // Buff
        Negative = 2, // Debuff
    }

    /// <summary>
    /// Buff 叠加类型
    /// </summary>
    public enum BuffStackingType
    {
        Override = 1,    // 覆盖：产生新 Buff 时，替换旧的，重置时间
        Stack = 2,       // 叠加层数：增加层数，重置时间（如：受击增伤，最高10层）
        Independent = 3, // 独立：每个 Buff 独立计时（如：多个不同的流血效果）
    }

    /// <summary>
    /// Buff 效果类型
    /// </summary>
    public enum BuffEffectType
    {
        Attr = 1,              // 属性
        Damage = 2,            // 伤害
        Heal = 3,              // 治疗

        Uncontrolled = 10,     // 无法控制
        DamageImmunity = 11,   // 免疫伤害
        ControlImmunity = 12,  // 免疫控制
    }

    public enum ProcEventType
    {
        OnHit = 1,           // 命中后
        OnDamage = 2,        // 伤害后
        OnTakeDamage = 3,    // 受到伤害后
        OnKill = 4,          // 击杀后
    }


    public struct DamageInfo
    {
        public Character Source;       // 谁干的
        public float damage;           // 伤害数值
        public bool IsCritical;        // 是否暴击

        public DamageInfo(Character source, float damage, bool isCritical)
        {
            this.Source = source;
            this.damage = damage;
            this.IsCritical = isCritical;
        }
    }

    public class BattleConfig{
        public const int BattleFrameRate = 10;
        public const float BattleTickTime = 1f / BattleFrameRate;

        public static BattleCamp EnemyCamp(BattleCamp camp)
        {
            return camp == BattleCamp.Friend ? BattleCamp.Enemy : BattleCamp.Friend;
        }

        public static LayerType CharacterLayer(CharacterType type, BattleCamp camp){
            switch(type){
                case CharacterType.Character:
                    return camp == BattleCamp.Friend ? LayerType.Friend : LayerType.Enemy;
                case CharacterType.Building:
                    return LayerType.Building;
                default:
                    return LayerType.Default;
            }
        }
    }
}