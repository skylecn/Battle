using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Base.Utils
{   
public static class CfgUtils
{
    public static Vector2 ToVec2(this cfg.vector2 vec){
        return new Vector2(vec.X, vec.Y);
    }
    public static Vector3 ToVec3(this cfg.vector3 vec)
    {
        return new Vector3(vec.X, vec.Y, vec.Z);
    }
    public static Vector4 ToVec4(this cfg.vector4 vec){
        return new Vector4(vec.X, vec.Y, vec.Z, vec.W);
    }
}
}
