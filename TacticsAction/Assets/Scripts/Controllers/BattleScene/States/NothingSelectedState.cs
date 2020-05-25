using System.Collections;
using System.Collections.Generic;
using API.Events;
using Model;
using UnityEngine;

namespace Controllers.BattleScene.States
{
    public class NothingSelectedState : BattleSceneState{
        public NothingSelectedState(BattleSceneController ctrl) : base(ctrl)
        {
            
        }

        public override IEnumerator Enter()
        {
            ctrl.grid.selectionMode = GridSelectionMode.Cell;
            ctrl.grid.DeSelectSelectedCharacter();
            ctrl.grid.CancelTargetTracker();
            ctrl.healthsBar.HideHealthStatus();
            ctrl.leftCharacterStatusBar.HideStatus();
            ctrl.rightCharacterStatusBar.HideStatus();
            yield break;
        }

        //TODO: OnGridCellMouseOver

        public override IEnumerator OnGridCharacterClicked(GridCharacterClickedData data)
        {
            yield return new WaitForEndOfFrame();
            
            if(data.GameObject.CompareTag("Ennemy")){
                ctrl.SetState(new EnnemySelectedState(ctrl, data.GameObject));
                yield break;
            }
            ctrl.SetState(new CharacterSelectedState(ctrl, data.GameObject));
            yield break;
        }
    }
}