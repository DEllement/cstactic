using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using API.Events;
using Controllers.BattleScene.Actions;
using Model;
using UnityEngine;

namespace Controllers.BattleScene.States
{
    public class ActionStrategyFactory{
        private BattleSceneController _ctrl;
        public ActionStrategyFactory(BattleSceneController ctrl){
            _ctrl = ctrl;
        }
        
        public IAttackStrategy Create(BattleAction battleAction){
            switch(battleAction){
                case MeleeAttackAction a:
                    return new MeleeAttack1AttackStrategy(_ctrl, battleAction);
            }
            return null;
        }
    }
    
    public interface IAttackStrategy{
        void OnEnter();
        void OnExist();
        void AttackTarget(IDamageable target);
    }
    
    public abstract class AttackStrategyBase : IAttackStrategy{
        protected BattleSceneController ctrl;
        protected BattleAction battleAction;
        protected AttackStrategyBase(BattleSceneController ctrl, BattleAction battleAction)
        {
            this.ctrl = ctrl;
            this.battleAction = battleAction;
        }

        public virtual void OnEnter()
        {
        }

        public virtual void OnExist()
        {
        }

        public virtual void AttackTarget(IDamageable target)
        {
                        
        }
    }
    
    public class MeleeAttack1AttackStrategy : AttackStrategyBase{
        public MeleeAttack1AttackStrategy(BattleSceneController ctrl, BattleAction action):base(ctrl, action){}
        public override void OnEnter(){
            ctrl.grid.CreateTargetTracker(ctrl.grid.SelectedCharacter.GetComponent<GridCharacterController>().GridPosition, new List<GridCellDir>{GridCellDir.N, GridCellDir.S, GridCellDir.E, GridCellDir.W},0,2);
        }
        public override void AttackTarget(IDamageable target)
        {
            
        }
    }
    
    public class PickActionMeleeTargetState : PickActionTargetState{
        private MeleeAttackAction _action;
        private IAttackStrategy _attackStrategy;
        public PickActionMeleeTargetState(BattleSceneController ctrl, MeleeAttackAction action) : base(ctrl)
        {
            _action = action;
            _attackStrategy = new ActionStrategyFactory(ctrl).Create(action);
        }
        public override IEnumerator Enter(){
            _attackStrategy.OnEnter();
            
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
            
            _attackStrategy.GetActions();
            
            var actions = new List<Action>();
            actions.Add(delegate() {  });
            
            ctrl.SetState(new PerformActionState(ctrl, actions));
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