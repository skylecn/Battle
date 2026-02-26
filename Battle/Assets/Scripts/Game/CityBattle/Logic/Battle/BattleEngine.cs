using System.Collections;
using System.Collections.Generic;
using cfg;
using UnityEngine;

namespace Game.CityBattle.Logic
{
public class BattleEngine
{
    public static float CalculateDamage(Character attacker, Character defender, SkillAction skillAction)
    {
        // 1. 获取攻击者基础伤害
        float rawDamage = attacker.attack;

        // 2. 获取技能倍率 (来自数据表)
        float skillMultiplier = skillAction.Ratio; 

        // 3. 计算暴击
        // bool isCrit = Random.value < attacker.critChance;
        // float critMod = isCrit ? attacker.critDamage : 1.0f;

        // 4. 最终伤害公式
        // Total = 基础伤害 * 技能倍率 * 暴击倍率 * 独立增伤 * 随机浮动
        float finalDamage = rawDamage * skillMultiplier /* * critMod */;

        // 5. 扣除防御减免
        // float dr = defender.Attributes.Armor.Value / (defender.Attributes.Armor.Value + 5000f); // 类似D3的减伤公式
        // finalDamage *= (1 - dr);

        return finalDamage;
    }

    public static float CalculateDamage(float attack, Character defender){
        return attack;
    }

    
}
}