using System.Collections;
using System.Collections.Generic;
using cfg;
using Collision2d;
using UnityEngine;
using Game.Base.Utils;

namespace Game.CityBattle.Logic
{
    public partial class BattleWorld
    {
        #region Singleton

        public static BattleWorld Instance
        {
            get { return Nested.instance; }
        }

        private class Nested
        {
            static Nested()
            {
            }

            internal static readonly BattleWorld instance = new BattleWorld();
        }

        private BattleWorld()
        {
        }

        #endregion

        public enum GameState
        {
            Init,
            Battle,
            Pause,
            Win,
            Lose,
        }

        public GameState State { get; private set; }

        public int Tick { get; private set; }

        public static float BattleTime { get; private set; }
        private float _passedTime;

        public PhysicsWorld physicsWorld { get; private set; }

        BattleInstance _battleInstance;

        int _monsterRefreshIndex;

        public void Init(BattleInstance battleInstance)
        {
            State = GameState.Init;
            _battleInstance = battleInstance;
            physicsWorld = new PhysicsWorld(6.0f);
            InitMapObj();
        }

        void InitMapObj()
        {
            for (int i = 0; i < _battleInstance.Friend.Count; i++)
            {
                var friendConf = CfgData.GetCharacter(_battleInstance.Friend[i]);
                CreateCharacter(friendConf, BattleCamp.Friend,
                    _battleInstance.FriendPos[i].ToVec2(), Vector2.right);
            }

            // for (int i = 0; i < _battleInstance.Enmey.Count; i++)
            // {
            //     var enemyConf = CfgData.GetCharacter(_battleInstance.Enmey[i]);
            //     CreateCharacter(enemyConf, BattleCamp.Enemy,
            //         _battleInstance.EnemyPos[i].ToVec2(), Vector2.left);
            // }
            _monsterRefreshIndex = 0;
        }

        public void StartBattle()
        {
            State = GameState.Battle;
            BattleTime = 0;
            Tick = 0;
            _passedTime = 0;
            foreach (var character in _characterList)
            {
                character.Value.Start();
            }
        }

        public void Update(float deltaTime)
        {
            if (State == GameState.Battle)
            {
                _passedTime += deltaTime;
                while (_passedTime >= BattleConfig.BattleTickTime)
                {
                    TickUpdate();
                    _passedTime -= BattleConfig.BattleTickTime;
                }
            }
        }

        public void TickUpdate()
        {
            Tick++;
            BattleTime += BattleConfig.BattleTickTime;
            physicsWorld.Step();
            FrameUpdate();
            LateFrameUpdate();

        }

        void FrameUpdate()
        {
            UpdateCharacter(BattleConfig.BattleTickTime);
            UpdateBullet(BattleConfig.BattleTickTime);
            RefreshMonster();
        }

        void LateFrameUpdate()
        {
        }

        public void Release()
        {
            physicsWorld.Clear();
            foreach (var character in _characterList)
            {
                character.Value.Release();
            }
            _characterList.Clear();
        }

        public void RefreshMonster()
        {
            if(_monsterRefreshIndex >= _battleInstance.Enmey.Count || BattleTime<_battleInstance.EnemyRefreshTime[_monsterRefreshIndex])
            {
                return;
            }

            var enemyConf = CfgData.GetCharacter(_battleInstance.Enmey[_monsterRefreshIndex]);
            var enemyPos = _battleInstance.EnemyPos[_monsterRefreshIndex].ToVec2();
            var enemyNum = _battleInstance.EnemyRefreshNum[_monsterRefreshIndex];
            float radius = _battleInstance.EnemyRefreshRadius[_monsterRefreshIndex];
            for(int i=0;i<enemyNum;i++)
            {
                float angle = Random.Range(0, 360) * Mathf.Deg2Rad;
                var randomPos = enemyPos + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * Random.Range(0, radius);
                var character = CreateCharacter(enemyConf, BattleCamp.Enemy,
                    randomPos, Vector2.left);
                character.Start();
            }
            _monsterRefreshIndex++;
        }
    }
}