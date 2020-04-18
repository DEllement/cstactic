using API.Events;

namespace API{
    public static class GameEvents
    {
        //Grid Events
        public static GridReady GridReady = new GridReady();
        public static GridCellSelected GridCellSelected = new GridCellSelected();
        public static GridCharacterSelected GridCharacterSelected = new GridCharacterSelected();
        public static GridCharacterDeSelected GridCharacterDeSelected = new GridCharacterDeSelected();
        public static GridCharacterLeavingGridCell GridCharacterLeavingGridCell = new GridCharacterLeavingGridCell();
        public static GridCharacterMovingToGridCell GridCharacterMovingToGridCell = new GridCharacterMovingToGridCell();
        public static GridCharacterMovedToGridCell GridCharacterMovedToGridCell = new GridCharacterMovedToGridCell();
        public static GridCharacterDoneMoving GridCharacterDoneMoving = new GridCharacterDoneMoving();
        
        public static CharacterTurnStarted CharacterTurnStarted = new CharacterTurnStarted();
        public static CharacterTurnEnded CharacterTurnEnded = new CharacterTurnEnded();
    }
}
