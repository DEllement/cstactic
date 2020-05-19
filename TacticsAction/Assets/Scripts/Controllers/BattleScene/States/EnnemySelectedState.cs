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
            yield break;        
        }
        public override IEnumerator OnGridCellClicked(GridCellClickedData data){
            
            var gridCharCtrl = _gridCharacter.gameObject.GetComponent<GridCharacterController>();
            //Clicked on already selected Character
            if( data.GridPosition.X == gridCharCtrl.X && data.GridPosition.Y == gridCharCtrl.Y){ 
                //ctrl.actionMenu.target = gridCharCtrl.gameObject;
                //ctrl.SetState(new ActionMenuOpenState(ctrl));
                //TODO: Could Show Menu like "Info, Attack" etc... 
                yield break;
            }
            
            //Reselected Curr Friend Character
            var currGridCharCtrl = ctrl.grid.SelectedCharacter.GetComponent<GridCharacterController>();
            ctrl.SetState(new CharacterSelectedState(ctrl, currGridCharCtrl.gameObject)); 
            ctrl.grid.HideGridCellAsReachable();
                
        }
        public override IEnumerator Exit(){
            ctrl.rightCharacterStatusBar.HideStatus();
            yield break;        
        }
    }
}