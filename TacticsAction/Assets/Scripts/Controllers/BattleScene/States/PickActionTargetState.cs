using System.Collections;
using System.Collections.Generic;
using System.Linq;
using API.Events;
using Model;
using UnityEngine;

namespace Controllers.BattleScene.States
{
    public class PickActionTargetState : BattleSceneState
    {
        private ActionType _actionType;
        public PickActionTargetState(BattleSceneController ctrl, ActionType actionType) : base(ctrl)
        {
            _actionType = actionType;
        }

        public override IEnumerator Enter()
        {
            ctrl.grid.selectionMode = GridSelectionMode.ActMelee;
            ctrl.grid.CreateTargetTracker(ctrl.grid.SelectedCharacter.GetComponent<GridCharacterController>().GridPosition, new List<GridCellDir>{GridCellDir.N, GridCellDir.S, GridCellDir.E, GridCellDir.W},0,2);
            
            yield break;
        }
        
        private AttackWith _attackWith;
        private int _skillId;
        private List<GameObject> _targets = new List<GameObject>();
        
        public override IEnumerator OnGridTargetsTargeted(GridTargetsTargetedData data){
            //TODO: Deal with previewing the damage
            
            _attackWith = AttackWith.Fist;
            _skillId = 0;
           
            
            if(_actionType == ActionType.Melee){
                //CleanUp Existing Targets
                _targets.ForEach(t=>{
                    var gridCharCtlr = t.GetComponent<GridCharacterController>(); //t.GetComponent<GridTargetObject>
                    if( gridCharCtlr != null)
                        gridCharCtlr.HideDamagePreview();
                });
                _targets.Clear();
                
                //Perform on new Selected Targets
                var targets = data.GridCells.Select(x=>x.GetComponent<GridCellController>().OccupiedBy).ToList();
                targets.ForEach(t=>{
                    _targets.Add(t);
                    var gridCharCtlr = t.GetComponent<GridCharacterController>(); //t.GetComponent<GridTargetObject>
                    if( gridCharCtlr != null){
                        var targetC = gridCharCtlr.Character;
                        var currC   = ctrl.turnManager.CurrentCharacter;
                        var result = this.ctrl.battleManager.PreviewActResult(_actionType, _attackWith, _skillId, currC, targetC);
                        var minDamage = result.Damages.Sum(x=>x.Min);
                        var maxDamage = result.Damages.Sum(x=>x.Max);
                        
                        gridCharCtlr.ShowDamagePreview(minDamage, maxDamage);
                    }
                });
            }
            
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