using System.Collections;
using API.Events;
using Model;

namespace Controllers.BattleScene.States
{
    public class PickMoveLocationState : BattleSceneState
    {
        public PickMoveLocationState(BattleSceneController ctrl) : base(ctrl)
        {
        }

        public override IEnumerator Enter()
        {
            ctrl.grid.selectionMode = GridSelectionMode.ActMove;
            yield break;
        }
        public override IEnumerator OnGridCellClicked(GridCellClickedData data){
        
            if( ctrl.grid.IsOutSideOfMoveZone(data.GridPosition)){
                ctrl.grid.HideGridCellAsReachable();
                ctrl.SetState(new CharacterSelectedState(ctrl));
                yield break;
            }
            
            var gridCharCtrl = ctrl.grid.SelectedCharacter.GetComponent<GridCharacterController>();

            //Reselected Character
            if(data.GridPosition.X == gridCharCtrl.X && data.GridPosition.Y == gridCharCtrl.Y){
                ctrl.actionMenu.target = ctrl.grid.SelectedCharacter;
                ctrl.actionMenu.ShowActionsMenu();
                ctrl.SetState(new ActionMenuOpenState(ctrl));
                yield break;
            }
            //Perform Move If Possible
            if(ctrl.grid.WalkCharacterToIfPossible(data.GridPosition))
            {
                ctrl.grid.HideGridCellAsReachable();
                ctrl.SetState(new MovingCharacterState(ctrl));
            }
            
            //TODO : Handle not moved case
            
            yield break;
        }
    }
}