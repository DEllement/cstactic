using System.Collections;
using System.Collections.Generic;
using Controllers.BattleScene.States;
using UnityEngine;

namespace Controllers.BattleScene.Actions
{
    public class RangeAttackAction : BattleAction
    {
        public override IEnumerator Execute()
        {
            ctrl.SetState(new PickActionTargetState(ctrl, OnTargetsTargeted, OnTargetsSelected ));
            yield break;
        }

        private void OnTargetsSelected(List<GameObject> targets)
        {
        }

        private void OnTargetsTargeted(List<GameObject> targets)
        {
        }
    }
}