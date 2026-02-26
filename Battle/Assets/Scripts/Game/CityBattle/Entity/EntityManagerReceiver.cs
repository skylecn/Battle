using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CityBattle.Entity
{
    public partial class EntityManager
    {
        public void BindReceiveEvents()
        {
            BattleController.RecvCreateCharacter += RecvCreateCharacter;
            BattleController.RecvRemoveCharacter += RecvRemoveCharacter;
            BattleController.RecvPlayAnimation += RecvPlayAnimation;
            BattleController.RecvSetDirection += RecvSetDirection;
            BattleController.RecvCharacterMovePos += RecvCharacterMovePos;
            BattleController.RecvCastSkill += RecvCastSkill;
            BattleController.RecvCreateEffect += RecvCreateEffect;
            BattleController.RecvRemoveEffect += RecvRemoveEffect;
            BattleController.RecvDamage += RecvDamage;
            BattleController.RecvHeal += RecvHeal;
            BattleController.RecvCreateBullet += RecvCreateBullet;
            BattleController.RecvRemoveBullet += RecvRemoveBullet;
            BattleController.RecvBulletMovePos += RecvBulletMovePos;
        }

        public void UnbindReceiveEvents()
        {
            BattleController.RecvCreateCharacter -= RecvCreateCharacter;
            BattleController.RecvRemoveCharacter -= RecvRemoveCharacter;
            BattleController.RecvPlayAnimation -= RecvPlayAnimation;
            BattleController.RecvSetDirection -= RecvSetDirection;
            BattleController.RecvCharacterMovePos -= RecvCharacterMovePos;
            BattleController.RecvCastSkill -= RecvCastSkill;
            BattleController.RecvCreateEffect -= RecvCreateEffect;
            BattleController.RecvRemoveEffect -= RecvRemoveEffect;
            BattleController.RecvDamage -= RecvDamage;
            BattleController.RecvHeal -= RecvHeal;
            BattleController.RecvCreateBullet -= RecvCreateBullet;
            BattleController.RecvRemoveBullet -= RecvRemoveBullet;
            BattleController.RecvBulletMovePos -= RecvBulletMovePos;
        }

        public void RecvCreateCharacter(float logicTime, int instanceId, int characterId, Vector2 position, Vector2 direction)
        {
            var character = CreateCharacter(instanceId);
            character.Init(logicTime, instanceId, characterId, position, direction);
        }

        public void RecvRemoveCharacter(int instanceId)
        {
            RemoveCharacter(instanceId);
        }

        public void RecvPlayAnimation(int instanceId, int animationId)
        {
            GetCharacter(instanceId).PlayAnimation(animationId);
        }

        public void RecvSetDirection(int instanceId, Vector2 direction)
        {
            GetCharacter(instanceId).SetDirection(direction);
        }

        public void RecvCharacterMovePos(float logicTime, int instanceId, Vector2 position)
        {
            GetCharacter(instanceId).MovePos(logicTime, position);
        }

        public void RecvCastSkill(int instanceId, int skillId)
        {
        }

        public void RecvCreateEffect(int instanceId, int effectId, int effectInstanceId)
        {
            GetCharacter(instanceId).OnEffect(effectId, effectInstanceId).Forget();
        }

        public void RecvRemoveEffect(int instanceId, int effectInstanceId)
        {
            GetCharacter(instanceId).OnRemoveEffect(effectInstanceId);
        }

        public void RecvDamage(int instanceId, int damage)
        {
            GetCharacter(instanceId).OnDamage(damage);
        }

        public void RecvHeal(int instanceId, int heal)
        {
            GetCharacter(instanceId).OnDamage(-heal);
        }

        public void RecvCreateBullet(float logicTime, int instanceId, int bulletId, Vector2 position, int sourceId, int targetId, Vector2 targetPosition)
        {
            var bullet = CreateBullet(instanceId);
            bullet.Init(logicTime, instanceId, bulletId, position, sourceId, targetId, targetPosition);
        }

        public void RecvRemoveBullet(int instanceId)
        {
            RemoveBullet(instanceId);
        }

        public void RecvBulletMovePos(float logicTime, int instanceId, Vector2 position)
        {
            GetBullet(instanceId).MovePos(logicTime, position);
        }
    }
}