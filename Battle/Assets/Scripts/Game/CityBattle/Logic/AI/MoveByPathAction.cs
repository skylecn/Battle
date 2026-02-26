using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPBehave;
using Game.CityBattle.Logic;

namespace Game.CityBattle.AI
{
    public class MoveByPathAction : Task
    {
        protected Character character => Blackboard.Get<Character>(CharBlackboard.Character);

        //private Path _path;
        private int _curIndex = 0;
        private Vector2 _curPoint;

        private Vector2 _targetPos;
        private float _targetRadius;

        public MoveByPathAction() : base("MoveByPathAction")
        {
        }

        protected override void DoStart()
        {
            RootNode.Clock.AddUpdateObserver(Update);
            //_path = null;
            _targetPos = Blackboard.Get<Vector2>(CharBlackboard.MoveTargetPos);
            _targetRadius = Blackboard.Get<float>(CharBlackboard.TargetRadius);
            //RequestPath();
            _curPoint = _targetPos;
            //character.PlaySpineAnimation(GlobalConfig.AnimName_Move1, true);
            //character.StartMove();
        }

        /*void RequestPath()
        {
            var _aBPathFinder = AStarManager.Instance.FindABPath(character.position.GetXZ(),
                new Vector3(_targetPos.x, 0f, _targetPos.z), OnPathComplete);
        }

        private void OnPathComplete(Path p)
        {
            if (currentState != State.ACTIVE)
                return;

            if (p.error)
            {
                StopAndCleanup(false);
                return;
            }

            _path = p;

            _curIndex = 0;
            _curPoint = _path.vectorPath[_curIndex];
            character.PlaySpineAnimation(GlobalConfig.AnimName_Move1, true);
        }*/

        private void StopAndCleanup(bool result)
        {
            //_path = null;
            //character.StopMove();
            RootNode.Clock.RemoveUpdateObserver(Update);
            Blackboard.Unset(CharBlackboard.MoveTargetPos);
            Stopped(result);
        }

        void Update()
        {
            //if (_path == null || _path.vectorPath.Count < 1) return;


            float distance = Vector2.Distance(character.position, _targetPos);
            if (distance <= _targetRadius)
            {
                StopAndCleanup(true);
                return;
            }

            if (character.position != _curPoint)
            {
                //float frameDistance = Time.deltaTime * character.moveSpeed;
                float dis = (_curPoint - character.position).magnitude;
                if (dis > 0)
                {
                    var dir = _curPoint - character.position;
                    //character.SetDirection(dir);
                    //float interpolationValue = frameDistance / dis;
                    //character.position = Vector3.Lerp(character.position, _curPoint, interpolationValue);
                    //character.rigidbodyPosition = Vector3.Lerp(character.position, _curPoint, interpolationValue);
                    //character.SetRigidbodyPosition(_curPoint);
                }
            }
            /*else
            {
                if (_curIndex == _path.vectorPath.Count - 1)
                {
                    if (_targetPos != character.position)
                    {
                        _curPoint = _targetPos;
                        return;
                    }

                    StopAndCleanup(true);
                    return;
                }

                _curIndex++;
                _curPoint = _path.vectorPath[_curIndex];
            }*/
        }

        protected override void DoStop()
        {
            StopAndCleanup(false);
        }
    }
}