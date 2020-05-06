using System.Collections;
using System.Collections.Generic;
using System.Linq;
using API.Events;
using Model;
using UnityEngine;

namespace Controllers.BattleScene.States
{
    public class PickMeleeTargetState : BattleSceneState
    {
        private ActionType _actionType;
        public PickMeleeTargetState(BattleSceneController ctrl, ActionType actionType) : base(ctrl)
        {
            _actionType = actionType;
        }

        public override IEnumerator Enter()
        {
            ctrl.grid.selectionMode = GridSelectionMode.ActMelee;
            ctrl.grid.CreateTargetTracker(ctrl.grid.SelectedCharacter.GetComponent<GridCharacterController>().GridPosition, new List<GridCellDir>{GridCellDir.N, GridCellDir.S, GridCellDir.E, GridCellDir.W},0,2);
            
            yield break;
        }

        public override IEnumerator OnGridTargetsSelected(GridTargetsSelectedData data)
        {
            //TODO: Check what is the target, and if the selection is valid
            
            if(_actionType == ActionType.Melee){
                var targets = data.GridCells.Select(x=>x.GetComponent<GridCellController>().OccupiedBy).ToList();
                if( targets.Any(t=>t.CompareTag("Ennemy")))
                {
                    //TODO: Pass down the selected targets to PerformActionState or assign on ctrl
                    ctrl.grid.CancelTargetTracker();
                    ctrl.grid.selectionMode = GridSelectionMode.Cell;
                    ctrl.SetState(new PerformActionState(ctrl));
                    
                    //Test
                    foreach (var gameObject in targets)
                    {
                        Object.Destroy(gameObject);
                    }
                    
                }
            }
            
            yield break;
        }
    }
}