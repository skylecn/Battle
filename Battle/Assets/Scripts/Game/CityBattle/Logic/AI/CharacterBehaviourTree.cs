using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPBehave;
using Game.CityBattle.Logic;

namespace Game.CityBattle.AI
{
    public class CharacterBehaviourTree
    {
        protected Blackboard _blackboard;
        public Blackboard Blackboard => _blackboard;
        protected Root _root;
        public Root Root => _root;
        public Character character;

        public CharacterBehaviourTree()
        {
            _root = CreateTree();
            _blackboard = _root.Blackboard;
        }

        protected virtual Root CreateTree()
        {
            return new Root(
                new Service(0.1f, UpdateBlackboard,
                    new Selector(
                        Fight(),
                        Idle()
                    )
                )
            );
        }

        protected virtual void UpdateBlackboard()
        {
        }

        public void Start(Character obj)
        {
            character = obj;
            _blackboard.Set(CharBlackboard.Character, character);
            OnReadyStart();

            _root.Start();
        }

        // 准备开始
        protected virtual void OnReadyStart()
        {
        }

        public void Stop()
        {
            if (_root.IsActive)
                _root.Stop();
        }

        public void Continue()
        {
            if (!_root.IsActive)
                _root.Start();
        }

        // 空闲
        protected Node Idle()
        {
            return new Sequence(
                new WaitUntilStopped()
            );
        }


        // 战斗
        protected Node Fight()
        {
            return
                new Condition(() =>
                    {
                        var target = character.FindNearestEnemy();
                        if (target == null)
                        {
                            return false;
                        }

                        Blackboard.Set(CharBlackboard.Target, target);
                        Blackboard.Set(CharBlackboard.TargetRadius, target.radius);
                        return true;
                    }, Stops.LOWER_PRIORITY_IMMEDIATE_RESTART, 1, 0,
                    new Sequence(
                        // 查找可用技能
                        new Action(() =>
                        {
                            var skillIndex = character.FindAvailableSkill();
                            if (skillIndex == -1) return false;
                            Blackboard.Set(CharBlackboard.SkillIndex, skillIndex);
                            return true;
                        }),
                        // 距离不够，移动
                        new Succeeder(
                            new Repeater(
                                new Condition(() =>
                                    {
                                        var target = Blackboard.Get<Character>(CharBlackboard.Target);
                                        return !character.InAttackRange(Blackboard.Get<int>(CharBlackboard.SkillIndex),
                                            target.position, Blackboard.Get<float>(CharBlackboard.TargetRadius));
                                    },
                                    new Sequence(
                                        new Action(() =>
                                        {
                                            var skill = character.GetSkill(
                                                Blackboard.Get<int>(CharBlackboard.SkillIndex));
                                            var radius = Blackboard.Get<float>(CharBlackboard.TargetRadius);
                                            Blackboard.Set(CharBlackboard.TargetRadius,
                                                skill.conf.Distance + radius);
                                        }),
                                        new MoveToTargetAction()
                                    ))
                            )
                        ),
                        new AttackAction()
                    )
                );
        }
    }
}