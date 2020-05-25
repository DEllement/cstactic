using System.Collections;
using System.Collections.Generic;
using API.Events;
using Controllers.BattleScene.Actions;
using Model;
using UnityEngine;

namespace Controllers.BattleScene.States
{
    public class ActionMenuOpenState : BattleSceneState{
        public ActionMenuOpenState(BattleSceneController ctrl) : base(ctrl)
        {
        }
    
        public override IEnumerator Enter()
        {
            ctrl.grid.HideGridCellAsReachable();
            ctrl.grid.selectionMode = GridSelectionMode.Disabled;
            ctrl.actionMenu.target = ctrl.grid.SelectedCharacter;
            ctrl.actionMenu.ShowActionsMenu();
            
            yield break;
        }
    
        public override IEnumerator OnActionMenuItemClicked(ActionMenuItemClickedData data){
            
            yield return new WaitForEndOfFrame(); //NOTE: To avoid the mouseup race issue
            ctrl.ExecuteBattleAction( data.ActionItem.Command() ); 
            
            /*switch(data.ActionType){
                /*case ActionType.Move:
                    ctrl.SetState(new PickMoveLocationState(ctrl, ctrl.grid.SelectedCharacter));
                    break;*
                case ActionType.Melee:
                    ctrl.SetState(new PickActionTargetState(ctrl));
                    break;
                case ActionType.Wait:
                    ctrl.grid.HideGridCellAsReachable();
                    break;
            }*/
        }
        
        public override IEnumerator OnNonUIClicked(){
            ctrl.SetState(new NothingSelectedState(ctrl));
            yield break;
        }

        public override IEnumerator Exit()
        {
            ctrl.actionMenu.CloseActionMenu();
            yield break;
        }
    }
}