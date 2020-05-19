using System.Collections;
using System.Collections.Generic;
using API.Events;
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
            switch(data.ActionType){
                case ActionType.Move:
                    yield return new WaitForEndOfFrame(); //NOTE: To avoid the mouseup race issue
                    ctrl.SetState(new PickMoveLocationState(ctrl));
                    break;
                case ActionType.Melee:
                    yield return new WaitForEndOfFrame();
                    ctrl.SetState(new PickActionTargetState(ctrl, data.ActionType));
                    break;
                case ActionType.Wait:
                    yield return new WaitForEndOfFrame();
                    ctrl.grid.HideGridCellAsReachable();
                    break;
            }
            yield break;
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