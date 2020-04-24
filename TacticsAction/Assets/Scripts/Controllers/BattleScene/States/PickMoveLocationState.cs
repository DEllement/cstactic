using System.Collections;
using API.Events;

namespace Controllers.BattleScene.States
{
    public class PickMoveLocationState : BattleSceneState
    {
        public PickMoveLocationState(BattleSceneController ctrl) : base(ctrl)
        {
        }

        public override IEnumerator Enter()
        {
            return base.Enter();
        }
        public override IEnumerator OnGridCellClicked(GridCellClickedData data){
        
            if( ctrl.grid.IsOutSideOfMoveZone(data.GridPosition)){
                ctrl.grid.DeSelectSelectedCharacter();
                ctrl.SetState(new NothingSelectedState(ctrl));
                yield break;
            }
            
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