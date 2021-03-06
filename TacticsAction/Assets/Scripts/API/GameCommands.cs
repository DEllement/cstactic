using API.Commands;

namespace API{
    public static class GameCommands
    {
        public static AssignCharacterToGrid AssignCharacterToGrid = new AssignCharacterToGrid();
        public static MoveGridCharacter MoveGridCharacter = new MoveGridCharacter();
        public static SelectCharacter SelectCharacter = new SelectCharacter();
        //public static DeSelectCharacter DeSelectCharacter = new DeSelectCharacter();
        public static ExecuteCharacterTurn ExecuteCharacterTurn = new ExecuteCharacterTurn();
        
        public static ShowActionsMenu ShowActionsMenu = new ShowActionsMenu();
        public static HideActionsMenu HideActionsMenu = new HideActionsMenu();
        
        public static ShowPossibleMove ShowPossibleMove = new ShowPossibleMove(); 
        public static ShowHealthStatus ShowHealthStatus = new ShowHealthStatus();
        public static HideHealthStatus HideHealthStatus = new HideHealthStatus();
        
        
        //Tests Actions
        public static ForceNextTurn ForceNextTurn = new ForceNextTurn();
    }
}