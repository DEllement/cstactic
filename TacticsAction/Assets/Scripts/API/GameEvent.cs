using API.Events;

namespace API{
    public static class GameEvents
    {
        public static GridReady GridReady = new GridReady();
        public static GridCellSelected GridCellSelected = new GridCellSelected();
        public static GridCharacterSelected GridCharacterSelected = new GridCharacterSelected();
        public static GridCharacterDeSelected GridCharacterDeSelected = new GridCharacterDeSelected();
    }
}
