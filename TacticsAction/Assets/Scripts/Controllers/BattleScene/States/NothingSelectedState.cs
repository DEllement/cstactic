using System.Collections;
using API.Events;

namespace Controllers.BattleScene.States
{
    public class NothingSelectedState : BattleSceneState{
        public NothingSelectedState(BattleSceneController ctrl) : base(ctrl)
        {
        }
        
        //TODO: OnGridCellMouseOver

        public override IEnumerator OnGridCharacterClicked(GridCharacterClickedData data)
        {
            ctrl.grid.SelectCharacter(data);
            ctrl.SetState(new CharacterSelectedState(this.ctrl));
            
            yield break;
        }
    }
}