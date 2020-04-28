using System.Collections;

namespace Controllers.BattleScene.States
{
    public class BattleIntroState: BattleSceneState{
        public BattleIntroState(BattleSceneController ctrl) : base(ctrl)
        {
        }

        public override IEnumerator Enter()
        {
            //TODO: show intro anim
            ctrl.SetState(new LevelObjectiveOverviewState(ctrl));
            
            yield break;
        }
    }
}