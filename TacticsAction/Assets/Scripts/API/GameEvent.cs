using API.Events;
using UnityEngine.Events;

namespace API{
    public static class GameEvents
    {
        //Grid Events
        public static GridReady GridReady = new GridReady();
        public static GridCellClicked GridCellClicked = new GridCellClicked();
        public static GridCharacterClicked GridCharacterClicked = new GridCharacterClicked();
        public static GridCharacterDeSelected GridCharacterDeSelected = new GridCharacterDeSelected();
        public static GridCharacterLeavingGridCell GridCharacterLeavingGridCell = new GridCharacterLeavingGridCell();
        public static GridCharacterMovingToGridCell GridCharacterMovingToGridCell = new GridCharacterMovingToGridCell();
        public static GridCharacterMovedToGridCell GridCharacterMovedToGridCell = new GridCharacterMovedToGridCell();
        public static GridCharacterDoneMoving GridCharacterDoneMoving = new GridCharacterDoneMoving();
        
        public static CharacterTurnStarted CharacterTurnStarted = new CharacterTurnStarted();
        public static CharacterTurnEnded CharacterTurnEnded = new CharacterTurnEnded();
        public static CharacterDied CharacterDied = new CharacterDied();
        
        public static TurnManagerInitialized TurnManagerInitialized = new TurnManagerInitialized();
        public static TurnManagerLineUpChanged TurnManagerLineUpChanged = new TurnManagerLineUpChanged();
        
        public static ActionMenuItemClicked ActionMenuItemClicked = new ActionMenuItemClicked();
        public static UnityEvent ActionMenuOpened = new UnityEvent();
        public static UnityEvent ActionMenuClosed = new UnityEvent();
        
        public static UnityEvent TurnBarReady = new UnityEvent();
    }
}
