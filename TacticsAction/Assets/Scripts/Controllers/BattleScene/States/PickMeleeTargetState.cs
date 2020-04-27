using System.Collections;
using API.Events;
using Model;

namespace Controllers.BattleScene.States
{
    public class PickMeleeTargetState : BattleSceneState
    {
        public PickMeleeTargetState(BattleSceneController ctrl) : base(ctrl)
        {
        }

        public override IEnumerator Enter()
        {
            ctrl.grid.selectionMode = GridSelectionMode.ActMelee;
            yield break;
        }

        public override IEnumerator OnGridCharacterClicked(GridCharacterClickedData data)
        {
            var attacker = ctrl.turnManager.CurrentCharacter; 
                
            yield break;
        }
    }
}