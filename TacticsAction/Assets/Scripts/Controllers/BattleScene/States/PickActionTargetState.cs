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
            Debug.Log("OnGridTargetsTargeted");
            _attackWith = AttackWith.Fist;
            _skillId = 0;
           
            if(_actionType == ActionType.Melee){
                //Perform on new Selected Targets
                var damagePreviewTargets = new List<HealthBarsController.DamageableTargetInfo>();
                var targets = data.GridCells.Select(x=>x.GetComponent<GridCellController>().OccupiedBy).Where(x=>x != null).ToList();
                targets.ForEach(t=>{
                    var damageableCtrl = t.GetComponent<IDamageableController>();
                    if( damageableCtrl != null){
                        var targetC = damageableCtrl.Damageable;
                        var currC   = ctrl.turnManager.CurrentCharacter;
                        var result = ctrl.battleManager.PreviewActResult(_actionType, _attackWith, _skillId, currC, targetC);
                        
                        damagePreviewTargets.Add(new HealthBarsController.DamageableTargetInfo{
                            damageableCtrl = damageableCtrl,
                            minDamage=(int)result.Damages.Sum(x=>x.Min),
                            maxDamage=(int)result.Damages.Sum(x=>x.Max)
                        });
                    }
                });
                
                ctrl.healthsBar.ShowDamagePreviews(damagePreviewTargets);
            }
            
            yield break;
        }
        public override IEnumerator OnGridTargetsSelected(GridTargetsSelectedData data)
        {
            //TODO: Check what is the target, and if the selection is valid
            Debug.Log("OnGridTargetsSelected");
            if( _actionType == ActionType.Melee ){
                var targets = data.GridCells.Select(x=>x.GetComponent<GridCellController>().OccupiedBy).Where(x=>x != null).ToList();
                targets.ForEach(t=>{
                    var damageableCtrl = t.GetComponent<IDamageableController>();
                    if( damageableCtrl != null){
                        //TODO: Perform actual damage using battle Manager     
                        //Test
                        Object.Destroy(damageableCtrl.GameObject);
                    }
                });
                    
                //TODO: Pass down the selected targets to PerformActionState or assign on ctrl
                ctrl.grid.CancelTargetTracker();
                ctrl.grid.selectionMode = GridSelectionMode.Cell;
                ctrl.SetState(new PerformActionState(ctrl));
            }
            
            yield break;
        }

        public override IEnumerator OnGridOutsideTargetRangeClicked(GridOutsideTargetRangeClickedData data){

            ctrl.healthsBar.HideHealthStatus();
            ctrl.SetState(new NothingSelectedState(ctrl));
            
            yield break;
        }
    }
}