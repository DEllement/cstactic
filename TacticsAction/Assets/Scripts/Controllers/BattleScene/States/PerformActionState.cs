using System;
using System.Collections;
using System.Collections.Generic;
using API.Events;

namespace Controllers.BattleScene.States
{
    public class PerformActionState : BattleSceneState{
        public Queue<Action> _commandToRuns;
        
        public PerformActionState(BattleSceneController ctrl, List<Action> _actions) : base(ctrl)
        {
            _commandToRuns = new Queue<Action>();
            _actions.ForEach(a=>{
                _commandToRuns.Enqueue(a);
            });
        }
        
        public override IEnumerator Enter(){
        
            
            
            yield break;
        }
    }
}