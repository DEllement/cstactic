using System.Collections;
using API.Events;
using Model;
using UnityEngine;

namespace Controllers.BattleScene.States
{
    public class PickMoveLocationState : BattleSceneState
    {
        public PickMoveLocationState(BattleSceneController ctrl) : base(ctrl)
        {
        }

        public override IEnumerator Enter()
        {
            ctrl.grid.selectionMode = GridSelectionMode.ActMove;
            ctrl.grid.BuildPossibleGroundMoveGraph(2); //TODO: depending on unit is flying, use Range Graph instead
            ctrl.grid.ShowGridCellAsReachable();
            
            yield break;
        }
        public override IEnumerator OnGridCellClicked(GridCellClickedData data){
            var gridCellCtrl = data.GameObject.GetComponent<GridCellController>();
            var currGridCharCtrl = ctrl.grid.SelectedCharacter.GetComponent<GridCharacterController>();
            
            //Clicked on already selected Character
            if( data.GridPosition.X == currGridCharCtrl.X && data.GridPosition.Y == currGridCharCtrl.Y){ 
                ctrl.actionMenu.target = ctrl.grid.SelectedCharacter;
                ctrl.SetState(new ActionMenuOpenState(ctrl));
            }
            //OutSide Move Zone Clicked
            else if( ctrl.grid.IsOutSideOfMoveZone(data.GridPosition)){
                if( gridCellCtrl.OccupiedBy != null){
                    var gridCharCtrl = gridCellCtrl.OccupiedBy.GetComponent<GridCharacterController>();
                    if( gridCharCtrl != null){ //Selecting an other character
                        ctrl.grid.HideGridCellAsReachable();
                        if(gridCharCtrl.Character.IsEnnemy)
                             ctrl.SetState(new EnnemySelectedState(ctrl, gridCharCtrl.gameObject));
                        else
                            ctrl.SetState(new CharacterSelectedState(ctrl, gridCharCtrl.gameObject));
                        
                        yield break;
                    }
                }
                //Reselected Character
                ctrl.SetState(new CharacterSelectedState(ctrl, currGridCharCtrl.gameObject)); 
                ctrl.grid.HideGridCellAsReachable();
            }
            //Perform Move If Possible
            else if(ctrl.grid.WalkCharacterToIfPossible(data.GridPosition))
            {
                ctrl.grid.HideGridCellAsReachable();
                ctrl.SetState(new MovingCharacterState(ctrl));
            }
            yield break;
        }
    }
}