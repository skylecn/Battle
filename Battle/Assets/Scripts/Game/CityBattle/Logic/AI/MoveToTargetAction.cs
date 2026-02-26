using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPBehave;
using Game.CityBattle.Logic;


namespace Game.CityBattle.AI
{
    public class MoveToTargetAction : Task
    {
        protected Character character => Blackboard.Get<Character>(CharBlackboard.Character);

        private Character _target;
        private float _targetRadius;

        public MoveToTargetAction() : base("MoveToTargetAction")
        {
        }

        protected override void DoStart()
        {
            RootNode.Clock.AddUpdateObserver(Update);
            _target = Blackboard.Get<Character>(CharBlackboard.Target);
            _targetRadius = Blackboard.Get<float>(CharBlackboard.TargetRadius);
            character.MoveToTarget(_target, _targetRadius);
        }

        private void StopAndCleanup(bool result)
        {
            //character.StopMove();
            RootNode.Clock.RemoveUpdateObserver(Update);
            Stopped(result);
        }

        void Update()
        {
            if(character.IsMoving) return;

            if(character.IsIdle)
            {
                StopAndCleanup(true);
                return;
            }
            else
            {
                StopAndCleanup(false);
                return;
            }

            // if (_target == null)
            // {
            //     StopAndCleanup(false);
            //     return;
            // }

            // float dis = Vector2.Distance(character.position, _target.position);
            // if (dis <= _targetRadius)
            // {
            //     StopAndCleanup(true);
            //     return;
            // }

            //float frameDistance = Time.deltaTime * character.moveSpeed;
            //float interpolationValue = frameDistance / dis;
            //character.position = Vector3.Lerp(character.position, _target.position, interpolationValue);
            //character._rigidbodyPosition = Vector3.Lerp(character.position, _target.position, interpolationValue);
            //var dir = (_target.position - character.position).normalized;
            //character.SetDirection(dir);
            //character.SetRigidbodyPosition(_target.position);
        }

        protected override void DoStop()
        {
            StopAndCleanup(false);
        }
    }
}