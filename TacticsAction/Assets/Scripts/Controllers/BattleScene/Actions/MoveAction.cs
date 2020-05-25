using System.Collections;
using Controllers.BattleScene.States;
using Model;

namespace Controllers.BattleScene.Actions
{
    public class MoveAction : BattleAction
    {
        public (int s, int e) RangeRadius;
        
        public override IEnumerator Execute(){
            ctrl.SetState(new PickMoveLocationState(ctrl, ctrl.grid.SelectedCharacter, RangeRadius, ActionType));
            yield break;
        }
    }
    public class MoveWalkAction : BattleAction{
        public override ActionType ActionType => ActionType.MoveWalk;
    }
}