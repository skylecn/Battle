
public static class LayerUtil
{
    public static int DefaultAllLayers = -1; // -1 在二进制补码中表示所有位都为1

    public static int AllLayersMask = (1<<(int)LayerType.Count)-1;

    public static int LayerMask(LayerType layer)
    {
        return LayerMask((int)layer);
    }

    public static int LayerMask(int layer)
    {
        return 1 << layer;
    }

    // 取反
    public static int LayerMaskNot(int layer)
    {
        return ~LayerMask(layer);
    }

    public static int LayerMaskNot(LayerType layer)
    {
        return LayerMaskNot((int)layer);
    }

    public static int LayerMask(params LayerType[] layers)
    {
        int mask = 0;
        foreach (var layer in layers)
            mask |= LayerMask(layer);
        return mask;
    }

    public static bool IsLayerValid(int layer, int layerMask)
    {
        return (layerMask & LayerMask(layer)) != 0;
    }
}
