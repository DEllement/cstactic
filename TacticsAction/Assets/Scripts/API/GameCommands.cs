using API.Commands;

namespace API{
    public static class GameCommands
    {
        public static AssignCharacterToGrid AssignCharacterToGrid = new AssignCharacterToGrid();
        public static MoveGridCharacter MoveGridCharacter = new MoveGridCharacter();
        public static SelectCharacter SelectCharacter = new SelectCharacter();
        public static DeSelectCharacter DeSelectCharacter = new DeSelectCharacter();
        public static ExecuteCharacterTurn ExecuteCharacterTurn = new ExecuteCharacterTurn();
    }
}