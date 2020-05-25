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
            var character = _gridCharacter.GetComponent<GridCharacterController>().Character;
            ctrl.grid.selectionMode = GridSelectionMode.Cell;
            ctrl.grid.SelectCharacter(_gridCharacter);
            ctrl.leftCharacterStatusBar.ShowStatus( character );
            
            //TODO: Generate Character Menu
            //TODO: If can move: Need to find whats the default move per character
            ctrl.SetState(new PickMoveLocationState(ctrl, _gridCharacter, character.DefaultMoveRange, character.DefaultMove ));
            yield break;        
        }
        
        public override IEnumerator OnActionMenuItemClicked(ActionMenuItemClickedData data){
            this.ctrl.ExecuteBattleAction(data.ActionItem.Command());
            yield break;
        }

        public override IEnumerator OnGridCharacterClicked(GridCharacterClickedData data)
        {
            //New Character Selected
            if(data.GameObject != _gridCharacter){
                Debug.Log("CharacterSelectedState::OnGridCharacterClicked * New Character Selected");
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
               
                ctrl.SetState(new NothingSelectedState(ctrl));
            }
        }
    }
}