using System.Collections;
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
            ctrl.grid.selectionMode = GridSelectionMode.Disabled;
            ctrl.grid.HideGridCellAsReachable();
            yield break;
        }
    
        public override IEnumerator OnActionMenuItemClicked(ActionMenuItemClickedData data){
            switch(data.ActionType){
                case ActionType.Move:
                    ctrl.actionMenu.CloseActionMenu();
                    ctrl.grid.BuildPossibleGroundMoveGraph(2); //TODO: depending on unit is flying, use Range Graph instead
                    ctrl.grid.ShowGridCellAsReachable();
                    
                    yield return new WaitForEndOfFrame(); //NOTE: To avoid the mouseup race issue
                    
                    ctrl.SetState(new PickMoveLocationState(ctrl));
                    
                    break;
                case ActionType.Melee:
                    ctrl.actionMenu.CloseActionMenu();
                    ctrl.grid.BuildPossibleRangeGraph(1);
                    ctrl.grid.ShowGridCellAsReachable();
                    
                    yield return new WaitForEndOfFrame();
                    
                    ctrl.SetState(new PickMeleeTargetState(ctrl));
                    break;
                case ActionType.Wait:

                    ctrl.actionMenu.CloseActionMenu();
                    
                    yield return new WaitForEndOfFrame();
                    ctrl.grid.HideGridCellAsReachable();
           
                    
                    break;
            }
            yield break;
        }

        public override IEnumerator Exit()
        {
            yield break;
        }
    }
}