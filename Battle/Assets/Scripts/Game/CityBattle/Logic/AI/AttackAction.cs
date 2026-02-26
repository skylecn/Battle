using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPBehave;
using Game.CityBattle.Logic;

namespace Game.CityBattle.AI
{
    public class AttackAction : Task
    {
        protected Character character => Blackboard.Get<Character>(CharBlackboard.Character);

        protected Character target => Blackboard.Get<Character>(CharBlackboard.Target);

        public AttackAction() : base("AttackAction")
        {
        }

        protected override void DoStart()
        {
            bool success = character.CastSkill(Blackboard.Get<int>(CharBlackboard.SkillIndex), target, target.position, AttackEnd);
            if(!success)
            {
                StopAndCleanup(false);
                return;
            }
        }

        void AttackEnd()
        {
            if (currentState != State.ACTIVE)
                return;
            StopAndCleanup(true);
        }

        protected override void DoStop()
        {
            StopAndCleanup(false);
        }

        private void StopAndCleanup(bool result)
        {
            Stopped(result);
        }
    }
}