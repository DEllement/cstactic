using System.Collections;
using Model;
using UnityEngine;

namespace Controllers.BattleScene.States
{
    public class CharacterTurnEndingState : BattleSceneState
    {
        public CharacterTurnEndingState(BattleSceneController ctrl) : base(ctrl)
        {
        }

        public override IEnumerator Enter()
        {
            ctrl.battleManager.EndCharacterTurn();
            //TODO: Need to launch what to display here

            yield return new WaitForSeconds(2);
            
            ctrl.turnManager.Next();
            ctrl.battleManager.StartCharacterTurn(ctrl.turnManager.CurrentCharacter);
            
            ctrl.SetState(new CharacterTurnStartingState(ctrl));
            
            yield break;
        }
    }
}