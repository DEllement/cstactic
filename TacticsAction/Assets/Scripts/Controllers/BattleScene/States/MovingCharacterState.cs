using System.Collections;
using API.Events;

namespace Controllers.BattleScene.States
{
    public class MovingCharacterState : BattleSceneState
    {
        public MovingCharacterState(BattleSceneController ctrl) : base(ctrl)
        {
        }
        
        public override IEnumerator OnGridCharacterLeavingGridCell(GridCharacterLeavingGridCellData data){yield break;}
        public override IEnumerator OnGridCharacterMovedToGridCell(GridCharacterMovedToGridCellData data){yield break;}
        public override IEnumerator OnGridCharacterDoneMoving(GridCharacterDoneMovingData data){yield break;}
        public override IEnumerator OnGridCharacterMovingToGridCell(GridCharacterMovingToGridCell data){
            
            //TODO: if character finish turn Next or DoNothing or OpenMenu?
            
            ctrl.grid.ComputeEdges();
            ctrl.SetState(new NothingSelectedState(ctrl));
            
            yield break;
        }
    }
}