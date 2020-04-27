namespace Model
{
    public class Character
    {
        public int Id;
        public string Name;
        public int Level;
        public bool IsEnnemy;
        public bool IsGuest;
        public bool AutoCombat;
        public GridCellDir FacingDir;
        
        //Stats
        public Stats Stats;
        public Inventory Inventory;
    }
    public class Ennemy : Character{
        public Ennemy()
        {
            IsEnnemy = true;
        }
    }
}