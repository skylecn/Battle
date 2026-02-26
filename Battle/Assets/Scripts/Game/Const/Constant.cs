using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constant
{
    #region Animation

    public static int TrackIndex0 = 0;
    public static int TrackIndex1 = 1;

    public static string AnimName_Idle = "idle";
    public static string AnimName_Move = "move";
    public static string AnimName_Dead = "dead";
    public static string AnimName_Skill1 = "skill1";
    public static string AnimName_Skill2 = "skill2";
    public static string AnimName_Skill3 = "skill3";

    public static int Anim_Idle = 1;
    public static int Anim_Move = 2;
    public static int Anim_Dead = 3;
    public static int Anim_Skill1 = 4;
    public static int Anim_Skill2 = 5;
    public static int Anim_Skill3 = 6;

    public static string[] AnimNameList = new string[] {"", AnimName_Idle, AnimName_Move, AnimName_Dead, AnimName_Skill1, AnimName_Skill2, AnimName_Skill3 };

    public static string GetAnimName(int animIndex)
    {
        return AnimNameList[animIndex];
    }

    #endregion
}
