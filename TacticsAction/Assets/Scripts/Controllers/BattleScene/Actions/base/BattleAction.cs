using System.Collections;
using Model;

namespace Controllers.BattleScene.Actions
{
    public abstract class BattleAction : IActionCommand
    {
        public virtual ActionType ActionType => ActionType.None;
        
        //Property Injected
        public BattleSceneController ctrl;
        public virtual IEnumerator Execute(){
            yield break;
        }
    }
}