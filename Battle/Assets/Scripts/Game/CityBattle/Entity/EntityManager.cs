using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CityBattle.Entity
{
    public partial class EntityManager
    {
        Dictionary<int, BulletEntity> _bulletDict = new Dictionary<int, BulletEntity>();

        Dictionary<int, CharacterEntity> _characterDict = new Dictionary<int, CharacterEntity>();

        public Dictionary<int, CharacterEntity> CharacterDict => _characterDict;

        #region Singleton

        public static EntityManager Instance
        {
            get { return Nested.instance; }
        }


        private class Nested
        {
            static Nested()
            {
            }

            internal static readonly EntityManager instance = new EntityManager();
        }

        private EntityManager()
        {
        }

        #endregion


        public void Init()
        {
        }

        public void Update(float time)
        {
            UpdateBullet(time);
            UpdateCharacter(time);
        }

        public void Release()
        {
        }

        #region Bullet

        BulletEntity GetBullet(int instanceId)
        {
            _bulletDict.TryGetValue(instanceId, out var bullet);
            return bullet;
        }

        BulletEntity CreateBullet(int instanceId)
        {
            var bullet = new BulletEntity();
            bullet.Id = instanceId;
            _bulletDict.Add(instanceId, bullet);
            return bullet;
        }

        void RemoveBullet(int instanceId)
        {
            if (_bulletDict.TryGetValue(instanceId, out var bullet))
            {
                _bulletDict.Remove(instanceId);
                bullet.Release();
            }
        }

        void UpdateBullet(float time)
        {
            foreach (var bullet in _bulletDict)
            {
                bullet.Value.Update(time);
            }
        }

        #endregion

        #region Character

        public CharacterEntity GetCharacter(int instanceId)
        {
            _characterDict.TryGetValue(instanceId, out var character);
            return character;
        }

        CharacterEntity CreateCharacter(int instanceId)
        {
            var character = new CharacterEntity();
            character.Id = instanceId;
            _characterDict.Add(instanceId, character);
            return character;
        }

        void RemoveCharacter(int instanceId)
        {
            if (_characterDict.TryGetValue(instanceId, out var character))
            {
                _characterDict.Remove(instanceId);
                character.Release();
            }
        }

        void UpdateCharacter(float time)
        {
            foreach (var character in _characterDict)
            {
                character.Value.Update(time);
            }
        }

        #endregion
    }
}