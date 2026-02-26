using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using cfg;
using Cysharp.Threading.Tasks;
using FairyGUI;
using Game.CityBattle.Entity;
using Game.CityBattle.Logic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace Game.CityBattle
{
    public partial class CityBattleManager
    {
        public static CityBattleManager Instance
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

            internal static readonly CityBattleManager instance = new CityBattleManager();
        }

        private CityBattleManager()
        {
        }

        BattleInstance _battleInstance;

        public CityBattleCameraController cameraController { get; private set; }

        public static float BattleTime { get; private set; }

        public void Init()
        {
            cameraController = new CityBattleCameraController();
            cameraController.Init(Camera.main);
            cameraController.MAX_Z = 50;
            cameraController.MIN_Z = -100;
            cameraController.Lod_Z = 10;
            //var cameraData = Camera.main.GetUniversalAdditionalCameraData();
            //cameraData.cameraStack.Add(StageCamera.main);

            AddEasyTouchListener();
        }

        public void InitMapObj(int instanceId)
        {
            _battleInstance = CfgData.GetBattleInstance(instanceId);
            EntityManager.Instance.Init();
            EntityManager.Instance.BindReceiveEvents();

            BattleWorld.Instance.Init(_battleInstance);
            UpdateFacade.CallOnce(3, (obj) => {
                BattleWorld.Instance.StartBattle();
#if UNITY_EDITOR
                foreach (var character in EntityManager.Instance.CharacterDict)
                {
                    character.Value.StartDebugger();
                }
                
#endif
            });
        }

        public void Update(float time)
        {
            if (BattleWorld.Instance.State == BattleWorld.GameState.Battle)
            {
                BattleTime += time;
            }

            BattleWorld.Instance.Update(time);
            EntityManager.Instance.Update(time);
        }

        public void Release()
        {
            RemoveEasyTouchListener();

            EntityManager.Instance.UnbindReceiveEvents();
            EntityManager.Instance.Release();
        }

        /*public static float MonsterBornRaidus = 2;

        private cfg.BattleInstance _cityBattleInstanceConf;
        public cfg.BattleInstance CityBattleInstanceConf => _cityBattleInstanceConf;

        private int _instanceId;

        private CityBattleState _state;
        public CityBattleState State => _state;


        private float _battleTime; // 战斗时间

        private float _exitCountDown;

        public static CityBattleManager Instance { get { return Nested.instance; } }
        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly CityBattleManager instance = new CityBattleManager();
        }


        #region 怪物刷新

        private List<Vector3> _monsterBornPoint;
        private List<List<(int, int)>> _monsterRefreshList;
        private List<int> _monsterRefreshIndex;

        private List<GameObject> _monsterBornEffect;
        private List<GameObject> _monsterLineEffect;
        private Material lineMaterial;

        private int _totalMonsterCount;
        private int _deadMonsterCount;

        public int mainBuildingId => 2;

        #endregion

        private Dictionary<int, List<BuildingEntity>> _levelTargetBuilding; // 关卡目标建筑

        private CancellationTokenSource _rebootCts; // 重启

        protected void OnCreated()
        {
            _rebootCts = new CancellationTokenSource();
        }

        protected void OnDestroy()
        {
            _rebootCts.Cancel();
            _rebootCts.Dispose();
        }

        public void Enter(int levelId)
        {
            _cityBattleInstanceConf = CfgData.GetBattleInstance(levelId);

            //UILogicUtils.PlayCloud(_LoadScene);
        }

        public void EnterAtNight(int levelId)
        {
            _cityBattleInstanceConf = CfgData.GetBattleInstance(levelId);
            _LoadScene();
        }

        void _LoadScene()
        {
            //XSceneManager.Instance.LoadSceneAsync<CityBattleScene>();
        }

        public void Exit()
        {
        }

        void _ExitScene()
        {
            //XSceneManager.Instance.LoadSceneAsync<GameScene>();
        }

        // public async UniTask Init()
        // {
        //     _state = CityBattleState.None;

        //     // 怪物刷新数据
        //     _monsterRefreshList = new List<List<(int, int)>>();
        //     _monsterBornPoint = new List<Vector3>();
        //     _monsterRefreshIndex = new List<int>();
        //     _totalMonsterCount = 0;

        //     if (_cityBattleInstanceConf.Enemylist1.Count > 0)
        //     {
        //         var enemyList = new List<(int, int)>();
        //         foreach (var enemy in _cityBattleInstanceConf.Enemylist1)
        //         {
        //             enemyList.Add((enemy.Value[0], enemy.Value[1]));
        //         }

        //         enemyList.Sort(SortEnemy);
        //         _monsterRefreshList.Add(enemyList);
        //         _monsterBornPoint.Add(new Vector3(_cityBattleInstanceConf.EnemyBornPoint[0].Value[0],
        //             _cityBattleInstanceConf.EnemyBornPoint[0].Value[1],
        //             _cityBattleInstanceConf.EnemyBornPoint[0].Value[2]));
        //         _monsterRefreshIndex.Add(0);
        //     }

        //     if (_cityBattleInstanceConf.Enemylist2.Count > 0)
        //     {
        //         var enemyList = new List<(int, int)>();
        //         foreach (var enemy in _cityBattleInstanceConf.Enemylist2)
        //         {
        //             enemyList.Add((enemy.Value[0], enemy.Value[1]));
        //         }

        //         enemyList.Sort(SortEnemy);
        //         _monsterRefreshList.Add(enemyList);
        //         _monsterBornPoint.Add(new Vector3(_cityBattleInstanceConf.EnemyBornPoint[1].Value[0],
        //             _cityBattleInstanceConf.EnemyBornPoint[1].Value[1],
        //             _cityBattleInstanceConf.EnemyBornPoint[1].Value[2]));
        //         _monsterRefreshIndex.Add(0);
        //     }

        //     if (_cityBattleInstanceConf.Enemylist3.Count > 0)
        //     {
        //         var enemyList = new List<(int, int)>();
        //         foreach (var enemy in _cityBattleInstanceConf.Enemylist3)
        //         {
        //             enemyList.Add((enemy.Value[0], enemy.Value[1]));
        //         }

        //         enemyList.Sort(SortEnemy);
        //         _monsterRefreshList.Add(enemyList);
        //         _monsterBornPoint.Add(new Vector3(_cityBattleInstanceConf.EnemyBornPoint[2].Value[0],
        //             _cityBattleInstanceConf.EnemyBornPoint[2].Value[1],
        //             _cityBattleInstanceConf.EnemyBornPoint[2].Value[2]));
        //         _monsterRefreshIndex.Add(0);
        //     }

        //     for (int i = 0; i < _monsterRefreshList.Count; i++)
        //     {
        //         _totalMonsterCount += _monsterRefreshList[i].Count;
        //     }

        //     _levelTargetBuilding = new Dictionary<int, List<BuildingEntity>>();

        //     lineMaterial = await AssetManager.LoadAssetAsyncWithRefCount<Material>("Line02.mat", default);
        // }

        // public async UniTask InitMap()
        // {
        //     try
        //     {
        //         //var timeGo = GameObject.Find("Timeline");
        //         //var timeLine = timeGo?.GetComponent<PlayableDirector>();

        //         // 创建建筑
        //         var buildingList = CfgData.Instance.Player.Buildings;
        //         foreach (var buildingInfo in buildingList)
        //         {
        //             var conf = Config.GetCityBattleBuilding(buildingInfo.Key);
        //             if (conf == null) continue;

        //             // 云已解锁跳出
        //             var buildingCfg = Config.GetBuilding(buildingInfo.Key);
        //             if (buildingCfg.LogicType == (int)BuildingType.Fog && buildingInfo.Value.Level > 0)
        //                 continue;

        //             await CreateMapEntity(conf, buildingInfo.Value.Level);
        //         }

        //         // 创建能被攻击的建筑
        //         foreach (var buildingId in _cityBattleInstanceConf.TargetBuildings)
        //         {
        //             var buildingInfo = CfgData.Instance.GetBuildingInfo(buildingId);
        //             if (buildingInfo == null || buildingInfo.Level == 0) continue;

        //             var conf = Config.GetCityBattleBuilding(buildingId);
        //             if (conf == null) continue;

        //             var mapEntity = GetMapEntity(buildingId);
        //             if (mapEntity == null) continue;

        //             foreach (var content in conf.ContentIds)
        //             {
        //                 var contentConf = Config.GetCityBattleBuildingContent(content);
        //                 if (contentConf == null) continue;

        //                 var building = CreateBuilding(mapEntity, contentConf);
        //                 if (building == null) continue;

        //                 // 加到关卡目标里
        //                 if (!_levelTargetBuilding.ContainsKey(buildingId))
        //                 {
        //                     _levelTargetBuilding.Add(buildingId, new List<BuildingEntity>());
        //                 }

        //                 _levelTargetBuilding[buildingId].Add(building);
        //             }
        //         }

        //         //创建英雄
        //         for (int i = 0; i < _cityBattleInstanceConf.HeroIds.Count; i++)
        //         {
        //             var heroId = _cityBattleInstanceConf.HeroIds[i];
        //             Vector3 bornPoint = Vector3.zero;
        //             if (i < _cityBattleInstanceConf.NpcBornPoint.Count)
        //             {
        //                 var pos = _cityBattleInstanceConf.NpcBornPoint[i];
        //                 bornPoint = new Vector3(pos.Value[0], pos.Value[1], pos.Value[2]);
        //             }

        //             await CreateHero(heroId, bornPoint, i);
        //         }

        //         // 创建玩家
        //         var playerConf = Config.GetCityBattleCharacter(_cityBattleInstanceConf.PlayerId);
        //         await CreatePlayer(playerConf);
        //         _player.position = new Vector3(_cityBattleInstanceConf.PlayerBornPoint[0],
        //             _cityBattleInstanceConf.PlayerBornPoint[1], _cityBattleInstanceConf.PlayerBornPoint[2]);
        //         CityBattleCameraController.Instance.SetCameraPos(_player.position);
        //         CityBattleCameraController.Instance.Follow(_player.transform);

        //         _player.gameObject.AddComponent<AudioListener>();
        //         var cameraAudioListener = Camera.main.gameObject.GetOrAddComponent<AudioListener>();
        //         cameraAudioListener.enabled = false;

        //         // 创建怪物出生特效
        //         _monsterBornEffect = new List<GameObject>();
        //         _monsterLineEffect = new List<GameObject>();
        //         for (int i = 0; i < _monsterBornPoint.Count; i++)
        //         {
        //             var bornEffect = await CreateEffect("fx_gc_guaiwu_birth.prefab", _monsterBornPoint[i]);
        //             if (bornEffect != null)
        //             {
        //                 _monsterBornEffect.Add(bornEffect);

        //                 // 增加线
        //                 if (i < _cityBattleInstanceConf.EnemyTarget.Count)
        //                 {
        //                     var buildingId = _cityBattleInstanceConf.EnemyTarget[i];
        //                     var b = GetBuildingByBuildingId(buildingId);
        //                     if (b != null)
        //                     {
        //                         var go = new GameObject();
        //                         var lineRender = go.AddComponent<LineRenderer>();
        //                         lineRender.material = lineMaterial;
        //                         lineRender.SetPosition(0, _monsterBornPoint[i] + new Vector3(0, 0.2f, 0));
        //                         lineRender.SetPosition(1, b.position + new Vector3(0, 0.2f, 0));
        //                         var distance = (b.position - _monsterBornPoint[i]).magnitude;
        //                         var mpb = new MaterialPropertyBlock();
        //                         lineRender.GetPropertyBlock(mpb);
        //                         mpb.SetFloat("_LineLength", distance);
        //                         lineRender.SetPropertyBlock(mpb);
        //                         _monsterLineEffect.Add(go);
        //                     }
        //                     else
        //                     {
        //                         _monsterLineEffect.Add(null);
        //                     }
        //                 }
        //             }
        //         }

        //         _deadMonsterCount = 0;


        //         //AudioController.PlayMusic(_cityBattleInstanceConf.Bgm);

        //         EventManager.TriggerEvent(GameEventType.LoadingCloudEnd);
        //         await UniTask.Delay(500);
        //         EventManager.TriggerEvent(new EventInt3(GameEventType.CityBattleMonsterDead, _deadMonsterCount,
        //             _totalMonsterCount));

        //         UIManager.Instance.ShowView<UIJoyStickTipCtrl>(false).Forget();


        //         /*if (timeLine != null)
        //         {
        //             timeLine.Play();
        //             await UniTask.Delay(TimeSpan.FromSeconds(timeLine.duration));
        //         }#1#

        //         /*foreach (var buildingId in _cityBattleInstanceConf.TargetBuildings)
        //         {
        //             EventManager.PostEvent(new EventInt3(GameEventType.CityBattleCreateBuilding, buildingId));
        //         }#1#

        //         foreach (var hero in _heroList)
        //         {
        //             hero.Value.StartAI();
        //         }

        //         _player?.StartAI();

        //         _state = CityBattleState.Battle;
        //         _battleTime = 0;

        //         _heroDamage = new Dictionary<int, int>();
        //     }
        //     catch (Exception e)
        //     {
        //         XLog.Error(e);
        //     }
        // }

        public void Release()
        {
            CityBattleCameraController.Instance.StopToFollow();


            _monsterRefreshList = null;
            _monsterBornPoint = null;
            _monsterRefreshIndex = null;

            ClearObj();

            foreach (var mapEntity in _mapEntityList)
            {
                mapEntity.Value.Release();
            }

            _mapEntityList.Clear();

            var cameraAudioListener = Camera.main.gameObject.GetComponent<AudioListener>();
            cameraAudioListener.enabled = true;

            InstancePoolManager.Instance.Clear();
            GameObjectPool.Instance.Clear();
        }

        void ClearObj()
        {
            // 清除怪物路线
            for (int i = 0; i < _monsterBornEffect.Count; i++)
            {
                if (_monsterBornEffect[i] != null)
                {
                    RemoveEffect(_monsterBornEffect[i]);
                }
            }

            _monsterBornEffect.Clear();

            for (int i = 0; i < _monsterLineEffect.Count; i++)
            {
                if (_monsterLineEffect[i] != null)
                {
                    GameObject.Destroy(_monsterLineEffect[i]);
                }
            }

            _monsterLineEffect.Clear();

            // 清理资源
            foreach (var building in _buildingList)
            {
                building.Value.Release();
            }

            _buildingList.Clear();

            foreach (var mapEntity in _mapEntityList)
            {
                mapEntity.Value.Release();
            }

            _mapEntityList.Clear();

            _levelTargetBuilding.Clear();

            foreach (var monster in _monsterList)
            {
                monster.Value.Release();
            }

            _monsterList.Clear();

            foreach (var hero in _heroList)
            {
                hero.Value.Release();
            }

            _heroList.Clear();

            if (_player != null)
            {
                var audio = _player.gameObject.GetComponent<AudioListener>();
                UnityEngine.Object.Destroy(audio);
                _player.Release();
                _player = null;
            }

            _colliderList.Clear();

            ClearBullets();
            ClearEffects();
        }

        public void Pause()
        {
            if (_state == CityBattleState.None) return;

            if (_state == CityBattleState.Battle)
            {
                foreach (var hero in _heroList)
                {
                    if (hero.Value.alive)
                        hero.Value.StopAI();
                }

                foreach (var monster in _monsterList)
                {
                    if (monster.Value.alive)
                        monster.Value.StopAI();
                }

                if (_player is { alive: true })
                    _player.StopAI();

                _state = CityBattleState.Pause;
            }

            //UIManager.Instance.ShowView<UICityBattlePauseCtrl>(false).Forget();
        }

        public void Resume()
        {
            if (_state == CityBattleState.Pause)
            {
                _state = CityBattleState.Battle;
                foreach (var hero in _heroList)
                {
                    if (hero.Value.alive)
                        hero.Value.ContinueAI();
                }

                foreach (var monster in _monsterList)
                {
                    if (monster.Value.alive)
                        monster.Value.ContinueAI();
                }

                if (_player is { alive: true })
                    _player.ContinueAI();
            }
        }

        public void ReStart()
        {
            if (_state == CityBattleState.None) return;

            ClearObj();

            // 重置怪物刷新
            for (int i = 0; i < _monsterRefreshIndex.Count; i++)
            {
                _monsterRefreshIndex[i] = 0;
            }

            //InitMap();

            //EventManager.PostEvent(GameEventType.CityBattleReStart);
        }

        // 关卡结束
        void End()
        {
            if (_player.alive)
            {
                var usingSkill = _player.GetUsingSkill();
                if (usingSkill != null)
                {
                    usingSkill.End();
                }

                _player.StopAI();
                _player.StopMove();
            }

            foreach (var hero in _heroList)
            {
                if (hero.Value.alive)
                    hero.Value.StopAI();
            }

            foreach (var monster in _monsterList)
            {
                if (monster.Value.alive)
                    monster.Value.StopAI();
            }
        }

        // 胜利动画
        void PlayWinAnim()
        {
            // foreach (var heroEntity in _heroList)
            // {
            //     if (heroEntity.Value.alive)
            //         heroEntity.Value.PlaySpineAnimation("win", true);
            // }

            // if (_player.alive)
            //     _player.PlaySpineAnimation("win", true);
        }

        int SortEnemy((int, int) a, (int, int) b)
        {
            return a.Item2.CompareTo(b.Item2);
        }

        int GetInstanceID()
        {
            return ++_instanceId;
        }

        // public void SetMove(Vector2 move)
        // {
        //     if (_state == CityBattleState.Battle)
        //         _player?.SetMove(move);
        // }

        private int _tempDebugNum = 0;

        List<BattleEntity> _deadList = new List<BattleEntity>();

        public void Update()
        {
            switch (_state)
            {
                case CityBattleState.Battle:
                {
                    _battleTime += Time.deltaTime;

                    // if (!_player.alive && (Time.time - _player.deadTime) > _player.cityBattleCharacter.RebornTime)
                    // {
                    //     _player.Reborn();
                    // }
                    // else
                    // {
                    //     _player?.Update(Time.deltaTime);
                    // }

                    _deadList.Clear();
                    foreach (var heroEntity in _heroList)
                    {
                        if (heroEntity.Value.alive)
                        {
                            heroEntity.Value.Update(Time.deltaTime);
                        }
                        // else if (Time.time - heroEntity.Value.deadTime > 2.5 && !heroEntity.Value.dissolve)
                        // {
                        //     heroEntity.Value.Dissolve();
                        // }
                        else if (Time.time - heroEntity.Value.deadTime > 5)
                        {
                            _deadList.Add(heroEntity.Value);
                        }
                    }

                    foreach (var deadEntity in _deadList)
                    {
                        RemoveHero(deadEntity as HeroEntity);
                    }

                    _deadList.Clear();
                    foreach (var monsterEntity in _monsterList)
                    {
                        if (monsterEntity.Value.alive)
                        {
                            monsterEntity.Value.Update(Time.deltaTime);
                        }
                        // else if (Time.time - monsterEntity.Value.deadTime > 2.5 && !monsterEntity.Value.dissolve)
                        // {
                        //     monsterEntity.Value.Dissolve();
                        // }
                        else if (Time.time - monsterEntity.Value.deadTime > 5)
                        {
                            _deadList.Add(monsterEntity.Value);
                        }
                    }

                    foreach (var deadEntity in _deadList)
                    {
                        RemoveMonster(deadEntity as MonsterEntity);
                        //AddDeadMonsterCount();
                    }


                    UpdateBullet(Time.deltaTime);
                    UpdateEffect(Time.deltaTime);

                    for (int i = 0; i < _monsterRefreshList.Count; i++)
                    {
                        var monsterList = _monsterRefreshList[i];
                        var monsterRefreshIndex = _monsterRefreshIndex[i];
                        if (monsterRefreshIndex < monsterList.Count &&
                            _battleTime >= monsterList[monsterRefreshIndex].Item2)
                        {
                            // 刷怪
                            //if (_tempDebugNum >= 1) continue;

                            Vector3 offset = new Vector3(Random.Range(0, MonsterBornRaidus), 0,
                                Random.Range(0, MonsterBornRaidus));
                            var bornPos = _monsterBornPoint[i] + offset;
                            CreateMonster(monsterList[monsterRefreshIndex].Item1, bornPos, i)
                                .Forget(e => Debug.LogError(e));

                            //CreateEffect("fx_gc_guaiwu_birth_glow.prefab", _monsterBornPoint[i], 4);
                            _monsterRefreshIndex[i]++;
                            _tempDebugNum++;

                            continue;
                        }
                    }

                    // for (int i = 0; i < _cityBattleInstanceConf.EnemyTarget.Count; i++)
                    // {
                    //     if (i < _monsterLineEffect.Count && _monsterLineEffect[i] != null)
                    //     {
                    //         // 对应建筑没了
                    //         var building = GetBuildingByBuildingId(_cityBattleInstanceConf.EnemyTarget[i]);
                    //         if (building == null)
                    //         {
                    //             GameObject.Destroy(_monsterLineEffect[i]);
                    //             _monsterLineEffect[i] = null;
                    //             continue;
                    //         }

                    //         // 对应怪没了
                    //         if (_monsterRefreshIndex[i] >= _monsterRefreshList[i].Count)
                    //         {
                    //             bool hasMonster = false;
                    //             foreach (var monsterEntity in _monsterList)
                    //             {
                    //                 if (monsterEntity.Value.alive && monsterEntity.Value.group == i)
                    //                 {
                    //                     hasMonster = true;
                    //                 }
                    //             }

                    //             if (!hasMonster)
                    //             {
                    //                 GameObject.Destroy(_monsterLineEffect[i]);
                    //                 _monsterLineEffect[i] = null;
                    //             }
                    //         }
                    //     }
                    // }


                    bool lose = true;
                    var mainBuilding = GetBuildingByBuildingId(mainBuildingId);
                    if (mainBuilding != null)
                    {
                        lose = false;
                    }

                    if (lose)
                    {
                        _state = CityBattleState.Lose;
                        _exitCountDown = 3;
                        End();
                        return;
                    }

                    if (_deadMonsterCount >= _totalMonsterCount)
                    {
                        _state = CityBattleState.Win;

                        _exitCountDown = 5;

                        End();
                        PlayWinAnim();
                    }
                }
                    break;
                case CityBattleState.Win:
                case CityBattleState.Lose:
                    _exitCountDown -= Time.deltaTime;
                    if (_exitCountDown <= 0)
                    {
                        _exitCountDown = float.MaxValue; // 防止重复退出
                        Exit();
                    }

                    break;
            }
        }

        public void FiexdUpdate()
        {
            foreach (var entity in _heroList)
            {
                entity.Value?.FixedUpdate();
            }

            foreach (var entity in _monsterList)
            {
                entity.Value?.FixedUpdate();
            }

            //_player?.FixedUpdate();
        }

        public void AddDeadMonsterCount()
        {
            _deadMonsterCount++;
            //EventManager.PostEvent(new EventInt3(GameEventType.CityBattleMonsterDead, _deadMonsterCount,
            //    _totalMonsterCount));
        }

        public BuildingEntity GetBuildingByBuildingId(int buildingId)
        {
            _levelTargetBuilding.TryGetValue(buildingId, out List<BuildingEntity> list);
            if (list != null)
            {
                foreach (var entity in list)
                {
                    if (entity.alive)
                        return entity;
                }
            }

            return null;
        }

        public BuildingEntity GetNearestBuildingByBuildingId(int buildingId, Vector3 position)
        {
            BuildingEntity building = null;
            _levelTargetBuilding.TryGetValue(buildingId, out List<BuildingEntity> list);
            if (list != null)
            {
                float distance = float.MaxValue;
                foreach (var buildingEntity in list)
                {
                    if (buildingEntity.alive)
                    {
                        var newDistance = Vector3.Distance(position, buildingEntity.position);
                        if (newDistance < distance)
                        {
                            distance = newDistance;
                            building = buildingEntity;
                        }
                    }
                }
            }

            return building;
        }

        // 怪物关卡目标
        public BattleEntity GetMonsterLevelTarget(int group, Vector3 pos)
        {
            // if (group >= _cityBattleInstanceConf.EnemyTarget.Count)
            // {
            //     return null;
            // }

            // var target = _cityBattleInstanceConf.EnemyTarget[group];
            // var building = GetNearestBuildingByBuildingId(target, pos);

            // if (building != null)
            // {
            //     return building;
            // }

            // foreach (var buildingId in _cityBattleInstanceConf.TargetBuildings)
            // {
            //     building = GetNearestBuildingByBuildingId(buildingId, pos);
            //     if (building != null)
            //     {
            //         return building;
            //     }
            // }

            return null;
        }

        // 英雄关卡目标
        public Vector3 GetHeroLevelTargetPos(int group)
        {
            // if (group >= _cityBattleInstanceConf.NpcTarget.Count)
            // {
            //     return Vector3.zero;
            // }

            // var target = _cityBattleInstanceConf.NpcTarget[group];
            // if (target > _monsterBornPoint.Count)
            // {
            //     return Vector3.zero;
            // }

            //var index = target - 1;
            var index = 0;
            if (_monsterRefreshIndex[index] < _monsterRefreshList[index].Count)
            {
                return _monsterBornPoint[index];
            }

            // 目标点没怪了，找离当前出生点最近的怪物
            var currentPoint = _monsterBornPoint[index];
            var minDistance = float.MaxValue;
            BattleEntity minMonster = null;
            foreach (var monster in _monsterList)
            {
                if (!monster.Value.alive) continue;
                var distance = Vector3.Distance(monster.Value.position, currentPoint);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    minMonster = monster.Value;
                }
            }

            if (minMonster != null)
            {
                return minMonster.position;
            }

            return Vector3.zero;
        }

        public (int, MonsterEntity) GetMonsterOutOfScreen(int group)
        {
            int num = 0;
            float minDistance = float.MaxValue;
            MonsterEntity monsterEntity = null;
            var camera = CityBattleCameraController.Instance.Camera;
            foreach (var entity in _monsterList)
            {
                var monster = entity.Value;
                if (monster.alive && monster.group == group && !IsPointVisible(camera, monster.position))
                {
                    num++;
                    var distance = Vector3.Distance(monster.position, _player.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        monsterEntity = monster;
                    }
                }
            }

            return (num, monsterEntity);
        }

        public bool IsPointVisible(Camera cam, Vector3 worldPoint)
        {
            Vector3 viewportPoint = cam.WorldToViewportPoint(worldPoint);

            // 判断是否在摄像机前方
            bool inFront = viewportPoint.z > 0;

            // 判断是否在视口范围内（0~1表示屏幕范围）
            bool onScreen = viewportPoint.x >= 0 && viewportPoint.x <= 1 &&
                            viewportPoint.y >= 0 && viewportPoint.y <= 1;

            return inFront && onScreen;
        }

        public (int, int) GetAliveBuildingCount()
        {
            _levelTargetBuilding.TryGetValue(mainBuildingId, out List<BuildingEntity> list);
            if (list != null)
            {
                int aliveCount = 0;
                foreach (var entity in list)
                {
                    if (entity.alive)
                        aliveCount++;
                }

                return (aliveCount, list.Count);
            }

            return (0, 0);
        }

        public (int, int) GetAliveBuildingHp()
        {
            _levelTargetBuilding.TryGetValue(mainBuildingId, out List<BuildingEntity> list);
            if (list != null)
            {
                int curHp = 0;
                int totalHp = 0;
                foreach (var entity in list)
                {
                    curHp += entity.hp;
                    totalHp += entity.hpMax;
                }

                return (curHp, totalHp);
            }

            return (0, 0);
        }*/
    }
}