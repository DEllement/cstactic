using System.Collections;
using API.Events;
using Model;
using UnityEngine;

namespace Controllers.BattleScene.States
{
    public class EnnemySelectedState: BattleSceneState{
        private GameObject _gridCharacter;
        public EnnemySelectedState(BattleSceneController ctrl, GameObject gridCharacter) : base(ctrl)
        {
            _gridCharacter = gridCharacter;
        }
        
        public override IEnumerator Enter(){
            ctrl.rightCharacterStatusBar.ShowStatus( _gridCharacter.GetComponent<GridCharacterController>().Character );
            
            //TODO: Generate ennemy selection action menu options
            
            yield break;        
        }
        public override IEnumerator OnActionMenuItemClicked(ActionMenuItemClickedData data){
            this.ctrl.ExecuteBattleAction(data.ActionItem.Command());
            yield break;
        }
        public override IEnumerator OnGridCellClicked(GridCellClickedData data){
            
            var gridCharCtrl = _gridCharacter.gameObject.GetComponent<GridCharacterController>();
            //Clicked on already selected Character
            if( data.GridPosition.X == gridCharCtrl.X && data.GridPosition.Y == gridCharCtrl.Y){ 
                //ctrl.actionMenu.target = gridCharCtrl.gameObject;
                //ctrl.SetState(new ActionMenuOpenState(ctrl));
                //TODO: Could Show Menu like "Info, Attack" etc... 
                
            }else{
                ctrl.SetState(new NothingSelectedState(ctrl)); 
            }
            
            yield break;
        }
        public override IEnumerator OnGridCharacterClicked(GridCharacterClickedData data)
        {
            //New Character Selected
            if( data.GameObject != _gridCharacter ){
                ctrl.grid.DeSelectSelectedCharacter();
                ctrl.SetState(new CharacterSelectedState(ctrl, data.GameObject));
                yield break;
            }
            
            yield return new WaitForEndOfFrame();
            
            //ctrl.SetState(new ActionMenuOpenState(ctrl));
        }

        public override IEnumerator Exit(){
            ctrl.rightCharacterStatusBar.HideStatus();
            yield break;        
        }
    }
}