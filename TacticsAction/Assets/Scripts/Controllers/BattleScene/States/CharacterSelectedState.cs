using System.Collections;
using API.Events;
using Model;
using UnityEngine;

namespace Controllers.BattleScene.States
{
    public class CharacterSelectedState: BattleSceneState{
        private GameObject _gridCharacter;
        public CharacterSelectedState(BattleSceneController ctrl, GameObject gridCharacter) : base(ctrl)
        {
            _gridCharacter = gridCharacter;
        }
        
        public override IEnumerator Enter(){
            Debug.Log("CharacterSelectedState::Enter");
            ctrl.grid.selectionMode = GridSelectionMode.Cell;
            ctrl.grid.SelectCharacter(_gridCharacter);
            ctrl.leftCharacterStatusBar.ShowStatus( _gridCharacter.GetComponent<GridCharacterController>().Character );
            //TODO: If can move:
            ctrl.SetState(new PickMoveLocationState(ctrl));
            yield break;        
        }

        public override IEnumerator OnGridCharacterClicked(GridCharacterClickedData data)
        {
            //New Character Selected
            if(data.GameObject != _gridCharacter){
                Debug.Log("CharacterSelectedState::OnGridCharacterClicked * New Character Selected");
                ctrl.grid.HideGridCellAsReachable();
                ctrl.grid.DeSelectSelectedCharacter();
                ctrl.SetState(new CharacterSelectedState(ctrl, data.GameObject));
                yield break;
            }
            
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