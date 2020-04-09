using API.Commands;

namespace API{
    public static class GameCommands
    {
        public static AssignCharacterToGrid AssignCharacterToGrid = new AssignCharacterToGrid();
        public static MoveGridCharacter MoveGridCharacter = new MoveGridCharacter();
    }
}