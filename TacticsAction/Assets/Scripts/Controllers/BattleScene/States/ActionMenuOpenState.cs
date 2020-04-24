using System.Collections;
using API.Events;

namespace Controllers.BattleScene.States
{
    public class ActionMenuOpenState : BattleSceneState{
        public ActionMenuOpenState(BattleSceneController ctrl) : base(ctrl)
        {
        }
    
        public override IEnumerator Enter()
        {
            ctrl.grid.selectionMode = GridSelectionMode.MenuOpen;
            ctrl.grid.HideGridCellAsReachable();
            yield break;
        }
    
        public override IEnumerator OnActionMenuItemClicked(ActionMenuItemClickedData data){
            switch(data.ActionType){
                case ActionType.Move:
                    ctrl.actionMenu.CloseActionMenu();
                    ctrl.grid.BuildPossibleGroundMoveGraph(2); //TODO: depending on unit is flying, use Range Graph instead
                    ctrl.grid.ShowGridCellAsReachable();
                    ctrl.grid.selectionMode = GridSelectionMode.ActMove;
                    
                    ctrl.SetState(new PickMoveLocationState(ctrl));
                    
                    break;
                case ActionType.Melee:
                    ctrl.actionMenu.CloseActionMenu();
                    ctrl.grid.BuildPossibleRangeGraph(1);
                    ctrl.grid.ShowGridCellAsReachable();
                    ctrl.grid.selectionMode = GridSelectionMode.ActMelee;
                    
                    ctrl.SetState(new PickMeleeTargetState(ctrl));
                    break;
            }
            yield break;
        }

        public override IEnumerator Exit()
        {
            ctrl.grid.selectionMode = GridSelectionMode.Cell;
            yield break;
        }
    }
}