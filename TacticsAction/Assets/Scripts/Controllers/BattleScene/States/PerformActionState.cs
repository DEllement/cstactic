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
            while(_commandToRuns.Count > 0){
                yield return ctrl.StartCoroutine(ExecuteAction(_commandToRuns.Dequeue()));
            }
            //TODO: On All Action Runned change move to next state or trust the queue last action
        }
        
        
        private IEnumerator ExecuteAction(Action action){
            action();
            yield break;
        }
    }
}