using System.Collections;
using API.Events;

namespace Controllers.BattleScene.States
{
    public class CharacterSelectedState: BattleSceneState{
        public CharacterSelectedState(BattleSceneController ctrl) : base(ctrl)
        {
        }
        
        public override IEnumerator Enter(){

            ctrl.grid.BuildPossibleGroundMoveGraph(2);
            ctrl.grid.ShowGridCellAsReachable();
            ctrl.grid.selectionMode = GridSelectionMode.ActMove;
            
            yield break;        
        }

        public override IEnumerator OnGridCharacterClicked(GridCharacterClickedData data)
        {
            ctrl.actionMenu.ShowActionsMenu();
            ctrl.SetState(new ActionMenuOpenState(this.ctrl));
            yield break;  
        }
    }
}