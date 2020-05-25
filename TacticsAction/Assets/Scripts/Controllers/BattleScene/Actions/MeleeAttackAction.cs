using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers.BattleScene.States;
using Model;
using UnityEngine;

namespace Controllers.BattleScene.Actions
{
    public class MeleeAttackAction : BattleAction
    {
        public AttackWith AttackWith;
        public int SkillId;
        
        public override IEnumerator Execute(){
            ctrl.SetState(new PickActionMeleeTargetState(ctrl, this));
            yield break;
        }
    }
}