using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BattleController
{

    #region Singleton
    public static BattleController Instance
    {
        get { return Nested.instance; }
    }

    private class Nested
    {
        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static Nested()
        {
        }

        internal static readonly BattleController instance = new BattleController();
    }

    private BattleController(){

    }
    #endregion

    #region Input Events

    public static Action<int, int> SendUseSkill;

    #endregion

    #region Receive Events
    public static Action RecvBattleStart;
    public static Action RecvBattleEnd;
    public static Action<float, int, int, Vector2, Vector2> RecvCreateCharacter;
    public static Action<int> RecvRemoveCharacter;
    public static Action<int, int> RecvPlayAnimation;
    public static Action<int, Vector2> RecvSetDirection;
    public static Action<float, int, Vector2> RecvCharacterMovePos;
    public static Action<int, int> RecvCastSkill;
    public static Action<int, int, int> RecvCreateEffect;
    public static Action<int, int> RecvRemoveEffect;
    public static Action<int, int> RecvDamage;
    public static Action<int, int> RecvHeal;
    public static Action<float, int, int, Vector2, int, int, Vector2> RecvCreateBullet;
    public static Action<int> RecvRemoveBullet;
    public static Action<float, int, Vector2> RecvBulletMovePos;

    #endregion

}