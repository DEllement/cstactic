using System.Collections;
using API.Events;
using Model;
using UnityEngine;

namespace Controllers.BattleScene.States
{
    public class NothingSelectedState : BattleSceneState{
        public NothingSelectedState(BattleSceneController ctrl) : base(ctrl)
        {
        }
        
        //TODO: OnGridCellMouseOver

        public override IEnumerator OnGridCharacterClicked(GridCharacterClickedData data)
        {
            yield return new WaitForEndOfFrame();
            
            ctrl.grid.SelectCharacter(data);
            ctrl.grid.BuildPossibleGroundMoveGraph(2);
            ctrl.grid.ShowGridCellAsReachable();
            ctrl.grid.selectionMode = GridSelectionMode.ActMove;
            ctrl.SetState(new PickMoveLocationState(this.ctrl));
            
            yield break;
        }
    }
}