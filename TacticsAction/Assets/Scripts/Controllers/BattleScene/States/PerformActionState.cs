using System.Collections;
using API.Events;

namespace Controllers.BattleScene.States
{
    public class PerformActionState : BattleSceneState{
        public PerformActionState(BattleSceneController ctrl) : base(ctrl)
        {
        }

        public override IEnumerator OnGridTargetsSelected(GridTargetsSelectedData data)
        {
            throw new System.NotImplementedException();
        }
    }
}