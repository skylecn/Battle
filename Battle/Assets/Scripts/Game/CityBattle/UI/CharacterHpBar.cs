using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using Game.CityBattle.Logic;
using UnityEngine;

namespace Game.CityBattle.UI
{
    public class CharacterHpBar : MapEntityUIComponent
    {
        private GProgressBar hpBar;

        public CharacterHpBar() : base("Common", "HpBar")
        {
        }

        public override void OnCreateView()
        {
            hpBar = panel.GetChild("hp").asProgress;

            var character = BattleWorld.Instance.GetCharacter(instanceId);
            if (character != null)
            {
                hpBar.GetChild("bar").asGraph.color = character.camp == BattleCamp.Friend ? Color.green : Color.red;
            }
            Refresh();
        }

        public void Refresh(){
            var character = BattleWorld.Instance.GetCharacter(instanceId);
            if (character == null)
            {
                return;
            }
            hpBar.value = character.hp / character.hpMax;
        }
    }
}