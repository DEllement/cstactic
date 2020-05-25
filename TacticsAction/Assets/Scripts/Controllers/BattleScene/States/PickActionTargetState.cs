using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using API.Events;
using Controllers.BattleScene.Actions;
using Model;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Controllers.BattleScene.States
{
    public class PickActionMeleeTargetState : PickActionTargetState{
        private MeleeAttackAction _action;
        public PickActionMeleeTargetState(BattleSceneController ctrl, MeleeAttackAction action) : base(ctrl)
        {
            _action = action;
        }
        public override IEnumerator Enter(){
            ctrl.grid.CreateTargetTracker(ctrl.grid.SelectedCharacter.GetComponent<GridCharacterController>().GridPosition, new List<GridCellDir>{GridCellDir.N, GridCellDir.S, GridCellDir.E, GridCellDir.W},0,2);
            yield break;
        }
        
        public override IEnumerator OnGridTargetsTargeted(GridTargetsTargetedData data){
            Debug.Log("OnGridTargetsTargeted");
            var targets = data.GridCells.Select(x=>x.GetComponent<GridCellController>().OccupiedBy).Where(x=>x != null).ToList();
            var damagePreviewTargets = new List<HealthBarsController.DamageableTargetInfo>();
            targets.ForEach(t=>{
                var damageableCtrl = t.GetComponent<IDamageableController>();
                if( damageableCtrl != null){
                    var targetC = damageableCtrl.Damageable;
                    var currC   = ctrl.turnManager.CurrentCharacter;
                    var result  = ctrl.battleManager.PreviewActResult(_action.ActionType, _action.AttackWith, _action.SkillId, currC, targetC);
                        
                    damagePreviewTargets.Add(new HealthBarsController.DamageableTargetInfo{
                        damageableCtrl = damageableCtrl,
                        minDamage=(int)result.Damages.Sum(x=>x.Min),
                        maxDamage=(int)result.Damages.Sum(x=>x.Max)
                    });
                }
            });
                
            ctrl.healthsBar.ShowDamagePreviews(damagePreviewTargets);
            yield break;
        }
        public override IEnumerator OnGridTargetsSelected(GridTargetsSelectedData data)
        {
            Debug.Log("OnGridTargetsSelected");
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
            yield break;
        }
    }
    
    public class PickActionTargetState : BattleSceneState
    {
        
        public PickActionTargetState(BattleSceneController ctrl) : base(ctrl)
        {
        }

        public override IEnumerator Enter()
        {
            yield break;
        }
        
        public override IEnumerator OnGridTargetsTargeted(GridTargetsTargetedData data){
            Debug.Log("OnGridTargetsTargeted");
            //var targets = data.GridCells.Select(x=>x.GetComponent<GridCellController>().OccupiedBy).Where(x=>x != null).ToList();
            yield break;
        }
        public override IEnumerator OnGridTargetsSelected(GridTargetsSelectedData data)
        {
            Debug.Log("OnGridTargetsSelected");
            //var targets = data.GridCells.Select(x=>x.GetComponent<GridCellController>().OccupiedBy).Where(x=>x != null).ToList();
            yield break;
        }

        public override IEnumerator OnGridOutsideTargetRangeClicked(GridOutsideTargetRangeClickedData data){
            ctrl.SetState(new NothingSelectedState(ctrl));
            yield break;
        }
    }
}