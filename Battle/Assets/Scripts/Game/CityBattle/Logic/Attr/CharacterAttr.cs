using System.Collections.Generic;

namespace Game.CityBattle.Logic
{
public enum AttrType
{
    None = 0,
    Hp = 1,
    Attack = 2,
    Def = 3,
    Cri = 4,
    AntiCri = 5,
    CriDamage = 6,
    MoveSpeed = 7,
    LifeSteal = 8,
}

public class CharacterAttrs
{
    // 存储所有属性
    private Dictionary<AttrType, Attr> attrs = new Dictionary<AttrType, Attr>();

    // 初始化
    public CharacterAttrs()
    {
        // 遍历枚举，初始化所有属性
        // foreach (AttrType type in System.Enum.GetValues(typeof(AttrType)))
        // {
        //     attrs.Add(type, new Attr());
        // }
    }
    
    public void AddAttr(AttrType type, float value)
    {
        if (!attrs.ContainsKey(type))
        {
            attrs.Add(type, new Attr());
        }
        attrs[type].BaseValue = value;
    }

    public void AddAttrModifier(AttrType type, AttrModifier modifier)
    {
        if (!attrs.ContainsKey(type))
        {
            attrs.Add(type, new Attr());
        }
        attrs[type].AddModifier(modifier);
    }

    public void RemoveAttrModifier(AttrType type, AttrModifier modifier)
    {
        if (attrs.ContainsKey(type))
        {
            attrs[type].RemoveModifier(modifier);
        }
    }

    public void RemoveAllAttrModifiers(object source)
    {
        foreach (var attr in attrs)
        {
            attr.Value.RemoveAllModifiersFromSource(source);
        }
    }

    public float GetAttrValue(AttrType type)
    {
        if (attrs.ContainsKey(type))
        {
            return attrs[type].Value;
        }
        return 0;
    }
}
}