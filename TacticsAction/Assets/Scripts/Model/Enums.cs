namespace Model
{
    public enum GridCellDir{
        NW,N,NE,W,E,SW,S,SE
    }
    public enum TileType{
        Default,
    } 
    public enum GridSelectionMode {
        Cell, //Nothing is selected yet
        Disabled, //When player choosing an action
        ActMove, //Need path finding
        ActMelee, //Need path finding+target reachable
        ActRange,//Need target reachable
        ActItem,//Need target reachable
    }
    public enum ActionType
    {
        None,
        Move,
        Act,
            Attack,
                Melee,
                Ranged,
            Magic,
                White,
                Grey,
                Black,
        Inventory,
            Scrolls,
            Consumable,
        Wait,
    }
}