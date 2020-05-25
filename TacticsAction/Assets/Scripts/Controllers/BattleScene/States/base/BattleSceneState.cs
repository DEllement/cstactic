using System.Collections;
using API.Events;

namespace Controllers.BattleScene.States
{
    public abstract class BattleSceneState{
        protected BattleSceneController ctrl;
        protected BattleSceneState(BattleSceneController ctrl)
        {
            this.ctrl = ctrl;
        }
        public virtual IEnumerator Enter(){ yield break;}
        public virtual IEnumerator Exit(){yield break;}
        public virtual IEnumerator OnActionMenuItemClicked(ActionMenuItemClickedData data){
            yield break;
        }
        public virtual IEnumerator OnActionMenuClosed(){yield break;}
        public virtual IEnumerator OnGridCellClicked(GridCellClickedData data){yield break;}
        public virtual IEnumerator OnGridCharacterClicked(GridCharacterClickedData data){ yield break;}
        public void OnGridCharacterDeSelected(GridCharacterDeSelectedData data){}
        
        public virtual IEnumerator OnGridCharacterLeavingGridCell(GridCharacterLeavingGridCellData data){yield break;}
        public virtual IEnumerator OnGridCharacterMovedToGridCell(GridCharacterMovedToGridCellData data){yield break;}
        public virtual IEnumerator OnGridCharacterDoneMoving(GridCharacterDoneMovingData data){yield break;}
        public virtual IEnumerator OnGridCharacterMovingToGridCell(GridCharacterMovingToGridCell data){yield break;}
        public virtual IEnumerator OnGridTargetsTargeted(GridTargetsTargetedData data){yield break;}
        public virtual IEnumerator OnGridTargetsSelected(GridTargetsSelectedData data){yield break;}
        public virtual IEnumerator OnGridOutsideTargetRangeClicked(GridOutsideTargetRangeClickedData data){yield break;}
        public virtual IEnumerator OnNonUIClicked(){yield break;}
    }
}