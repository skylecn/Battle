using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Game.CityBattle.UI;

namespace Game.CityBattle.Entity
{
    public partial class CharacterEntity : MapEntity
    {
        private cfg.CharacterData _characterData;

        public CharacterHpBar hpBar;

        public void Init(float logicTime, int instanceId, int characterId, Vector2 position, Vector2 direction)
        {
            Id = instanceId;
            _characterData = CfgData.GetCharacter(characterId);
            InitPos(logicTime, position);
            SetDirection(direction);
            LoadRes(_characterData.Res).Forget();
            hpBar = new CharacterHpBar();
            hpBar.initPosition = new Vector3(0,3f, 0);
            hpBar.Init(this);
        }


        protected override void OnRecycle()
        {
            hpBar?.Release();
            base.OnRecycle();
        }

        public void OnDamage(int damage)
        {
            hpBar.Refresh();
        }


    }
}
