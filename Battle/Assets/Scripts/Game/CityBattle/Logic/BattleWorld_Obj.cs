using System.Collections;
using System.Collections.Generic;
using cfg;
using UnityEngine;

namespace Game.CityBattle.Logic
{
    public partial class BattleWorld
    {
        private int _objId;

        private Dictionary<int, Character> _characterList = new Dictionary<int, Character>();

        Dictionary<int, Bullet> _bulletList = new Dictionary<int, Bullet>();

        int GetObjID()
        {
            return ++_objId;
        }

        #region Character

        public Character GetCharacter(int instanceId)
        {
            _characterList.TryGetValue(instanceId, out var character);
            return character;
        }

        private void AddCharacter(Character character)
        {
            _characterList.Add(character.instanceId, character);
        }

        private void RemoveCharacter(Character character)
        {
            _characterList.Remove(character.instanceId);
        }

        Character CreateCharacter(CharacterData characterConf, BattleCamp camp, Vector2 position, Vector2 direction)
        {
            var character = new Character();
            character.Init(GetObjID(), characterConf, camp, position, direction);
            AddCharacter(character);
            BattleController.RecvCreateCharacter?.Invoke(BattleTime, character.instanceId, characterConf.Id, position,
                direction);
            return character;
        }

        List<Character> _removeCharacterList = new List<Character>();

        void UpdateCharacter(float time)
        {
            foreach (var character in _characterList)
            {
                character.Value.Update(time);
                if (character.Value.ShouldDisappear)
                {
                    _removeCharacterList.Add(character.Value);
                }
            }

            foreach (var character in _removeCharacterList)
            {
                character.Release();
                _characterList.Remove(character.instanceId);
                BattleController.RecvRemoveCharacter?.Invoke(character.instanceId);
            }

            _removeCharacterList.Clear();
        }

        #endregion

        #region Bullet

        public Bullet CreateBullet(SkillInstance skillInstance, Character target, Vector2 targetPosition,
            SkillAction skillAction)
        {
            var bullet = new Bullet();
            bullet.Init(GetObjID(), skillInstance, target, targetPosition, skillAction);
            _bulletList.Add(bullet.instanceId, bullet);
            BattleController.RecvCreateBullet?.Invoke(BattleTime, bullet.instanceId, bullet.bulletConf.Id,
                bullet.position, skillInstance.caster.instanceId, target.instanceId, targetPosition);
            return bullet;
        }

        private List<int> _removeBulletList = new List<int>();

        public void UpdateBullet(float time)
        {
            foreach (var bullet in _bulletList)
            {
                if (!bullet.Value.isDestroy)
                    bullet.Value.Update(time);
                else
                {
                    _removeBulletList.Add(bullet.Key);
                }
            }

            foreach (var bulletId in _removeBulletList)
            {
                _bulletList.Remove(bulletId);
                BattleController.RecvRemoveBullet?.Invoke(bulletId);
            }

            _removeBulletList.Clear();
        }

        void ClearBullets()
        {
            _bulletList.Clear();
        }

        #endregion

        #region ColliderCheck

        List<Character> _characters = new List<Character>();

        public List<Character> OverlapSphere(Vector2 pos, float radius, int layerMask)
        {
            _characters.Clear();
            var list = physicsWorld.OverlapCircle(pos, radius, layerMask);
            foreach (var body in list)
            {
                if (body.UserData is Character character && character.IsAlive)
                {
                    _characters.Add(character);
                }
            }

            return _characters;
        }

        public List<Character> OverlapSphere(Vector2 pos, float radius, int layerMask, BattleCamp camp, bool sameCamp)
        {
            _characters.Clear();
            var list = physicsWorld.OverlapCircle(pos, radius, layerMask);
            foreach (var body in list)
            {
                if (body.UserData is Character character && character.IsAlive)
                {
                    if (sameCamp && character.camp != camp) continue;
                    if (!sameCamp && character.camp == camp) continue;
                    _characters.Add(character);
                }
            }

            return _characters;
        }

        public Character GetNearestCharacter(Vector2 pos, float radius, int layerMask, BattleCamp camp, bool sameCamp)
        {
            var characters = OverlapSphere(pos, radius, layerMask, camp, sameCamp);
            if (characters.Count == 0)
            {
                return null;
            }

            float minDis = float.MaxValue;
            Character nearestChar = null;
            foreach (var character in characters)
            {
                float dis = (character.position - pos).sqrMagnitude;
                if (dis < minDis)
                {
                    minDis = dis;
                    nearestChar = character;
                }
            }

            return nearestChar;
        }

        public Character GetNearestFriend(Character character, float radius)
        {
            return GetNearestCharacter(character.position, radius, LayerUtil.LayerMask(character.layerType),
                character.camp, true);
        }

        public Character GetNearestEnemy(Character character, float radius)
        {
            return GetNearestCharacter(character.position, radius, LayerUtil.LayerMaskNot(character.layerType),
                character.camp, false);
        }

        #endregion

        #region AI

        public Character GetNearestBuilding(Vector2 pos)
        {
            float minDis = float.MaxValue;
            Character nearestBuilding = null;
            foreach (var character in _characterList)
            {
                if (character.Value.type != CharacterType.Building) continue;
                float dis = (character.Value.position - pos).sqrMagnitude;
                if (dis < minDis)
                {
                    minDis = dis;
                    nearestBuilding = character.Value;
                }
            }

            return nearestBuilding;
        }

        #endregion
    }
}