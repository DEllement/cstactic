using System.Collections;
using System.Collections.Generic;
using API.Events;
using Model;
using UnityEngine;

namespace Controllers.BattleScene.States
{
    public class NothingSelectedState : BattleSceneState{
        public NothingSelectedState(BattleSceneController ctrl) : base(ctrl)
        {
            
        }

        public override IEnumerator Enter()
        {
            ctrl.grid.selectionMode = GridSelectionMode.Cell;
            ctrl.grid.HideGridCellAsReachable();
            ctrl.grid.CancelTargetTracker();
            ctrl.healthsBar.HideHealthStatus();
            ctrl.leftCharacterStatusBar.HideStatus();
            yield break;
        }

        //TODO: OnGridCellMouseOver

        public override IEnumerator OnGridCharacterClicked(GridCharacterClickedData data)
        {
            yield return new WaitForEndOfFrame();
            
            ctrl.grid.SelectCharacter(data);
            ctrl.grid.BuildPossibleGroundMoveGraph(2);
            ctrl.grid.ShowGridCellAsReachable();
            ctrl.grid.selectionMode = GridSelectionMode.ActMove;
            ctrl.leftCharacterStatusBar.ShowStatus( data.GameObject.GetComponent<GridCharacterController>().Character );
            ctrl.SetState(new PickMoveLocationState(this.ctrl));
            
    
            
            yield break;
        }
    }
}