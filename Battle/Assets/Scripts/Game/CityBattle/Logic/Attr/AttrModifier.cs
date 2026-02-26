namespace Game.CityBattle.Logic
{
    public enum AttrModType
{
    Flat = 100,          // 固定值 (+10)
    PercentAdd = 200,    // 加算百分比 (+10%)
    PercentMult = 300    // 乘算百分比 (x1.1)
}

public class AttrModifier
{
    public float value;
    public AttrModType type;
    public int order;       // 排序用，保证计算顺序
    public object source;   // 来源（比如来自哪把剑，方便移除时查找）

    public AttrModifier(float value, AttrModType type, int order, object source)
    {
        this.value = value;
        this.type = type;
        this.order = order;
        this.source = source;
    }
}
}