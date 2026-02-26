using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPBehave;
using Game.CityBattle.Entity;

namespace Game.CityBattle.AI
{
    public class BuildingBehaviourTree : CharacterBehaviourTree
    {
        protected override Root CreateTree()
        {
            return new Root(
                new Service(0.1f, UpdateBlackboard,
                    BuildingFight()
                )
            );
        }

        Node BuildingFight()
        {
            return
                new Selector(
                    new Condition(() =>
                        {
                            var target = character.FindNearestEnemy();
                            if (target == null)
                            {
                                return false;
                            }

                            var skillIndex = character.FindAvailableSkill(target.position, target.radius);
                            if (skillIndex == -1) return false;

                            Blackboard.Set(CharBlackboard.SkillIndex, skillIndex);
                            Blackboard.Set(CharBlackboard.Target, target);
                            return true;
                        },
                        new AttackAction()
                    ),
                    // 如果没敌人或可用技能，则等待
                    new Wait(1)
                );
        }
    }
}