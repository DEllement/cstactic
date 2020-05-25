using System.Collections;
using API.Events;
using Controllers.BattleScene.Actions;
using Model;
using UnityEngine;

namespace Controllers.BattleScene.States
{
    public class PickMoveLocationState : BattleSceneState
    {
        private GameObject _gridCharacter;
        private (int s, int e) _moveRangeRadius;
        private ActionType _actionMoveType;
        public PickMoveLocationState(BattleSceneController ctrl, GameObject gridCharacter, (int s, int e) moveRangeRadius, ActionType actionMoveType) : base(ctrl)
        {
            _gridCharacter = gridCharacter;
            _moveRangeRadius = moveRangeRadius;
            _actionMoveType = actionMoveType;
        }

        public override IEnumerator Enter()
        {
            ctrl.grid.selectionMode = GridSelectionMode.ActMove;
            
            switch(_actionMoveType){
                case ActionType.MoveWalk: ctrl.grid.BuildPossibleGroundMoveGraph(_moveRangeRadius.e); break;
                case ActionType.MoveFly: break;
                case ActionType.MoveTeleport: break;
            }
            ctrl.grid.ShowGridCellAsReachable();
            
            yield break;
        }
        public override IEnumerator OnGridCellClicked(GridCellClickedData data){
            var gridCellCtrl = data.GameObject.GetComponent<GridCellController>();
            var currGridCharCtrl = _gridCharacter.GetComponent<GridCharacterController>();
            
            //Clicked on already selected Character
            if( data.GridPosition.X == currGridCharCtrl.X && data.GridPosition.Y == currGridCharCtrl.Y){ 
                ctrl.actionMenu.target = _gridCharacter;
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
                    }
                }
                ctrl.SetState(new NothingSelectedState(ctrl));
                //Reselected Character
                //ctrl.SetState(new CharacterSelectedState(ctrl, currGridCharCtrl.gameObject)); 
                //ctrl.grid.HideGridCellAsReachable();
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