using System.Collections;

namespace Controllers.BattleScene.States
{
    public class CharacterTurnStartingState : BattleSceneState
    {
        public CharacterTurnStartingState(BattleSceneController ctrl) : base(ctrl)
        {
        }

        public override IEnumerator Enter()
        {
            //TODO: Show or do what ever here
            
            ctrl.battleManager.StartCharacterTurn( ctrl.turnManager.CurrentCharacter );
            ctrl.grid.SelectCharacter( ctrl.turnManager.CurrentCharacter.Id );
            
            yield break;
        }
    }
}