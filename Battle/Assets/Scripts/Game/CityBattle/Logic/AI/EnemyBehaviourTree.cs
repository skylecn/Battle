using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPBehave;
using Game.CityBattle.Logic;

namespace Game.CityBattle.AI
{
    public class EnemyBehaviourTree : CharacterBehaviourTree
    {
        protected override Root CreateTree()
        {
            return new Root(
                new Service(0.1f, UpdateBlackboard,
                    new Selector(
                        Fight(),
                        MoveToTarget(),
                        Idle()
                    )
                )
            );
        }

        // 移动到目标
        protected Node MoveToTarget()
        {
            return new Condition(() =>
            {
                var nearestBuilding = character.FindNearestBuilding();
                if (nearestBuilding == null)
                {
                    return false;
                }
                Blackboard.Set(CharBlackboard.Target, nearestBuilding);
                Blackboard.Set(CharBlackboard.TargetRadius, nearestBuilding.radius);
                return true;
            }, new MoveToTargetAction());
        }
    }
}