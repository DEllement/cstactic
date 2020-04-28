using System.Collections;
using API.Events;
using Model;
using UnityEngine;

namespace Controllers.BattleScene.States
{
    public class CharacterSelectedState: BattleSceneState{
        public CharacterSelectedState(BattleSceneController ctrl) : base(ctrl)
        {
        }
        
        public override IEnumerator Enter(){
            ctrl.grid.selectionMode = GridSelectionMode.Cell;
            yield break;        
        }

        public override IEnumerator OnGridCharacterClicked(GridCharacterClickedData data)
        {
            ctrl.actionMenu.target = ctrl.grid.SelectedCharacter;
            
            yield return new WaitForEndOfFrame();
            
            ctrl.SetState(new ActionMenuOpenState(ctrl));
        }

        public override IEnumerator OnGridCellClicked(GridCellClickedData data)
        {
            if(data.GameObject.GetComponent<GridCellController>().OccupiedBy != ctrl.grid.SelectedCharacter){
                yield return new WaitForEndOfFrame();
                
                ctrl.grid.HideGridCellAsReachable();
                ctrl.grid.DeSelectSelectedCharacter();
                ctrl.grid.selectionMode = GridSelectionMode.Cell;
                ctrl.SetState(new NothingSelectedState(ctrl));
            }
        }
    }
}