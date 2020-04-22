using System.Collections.Generic;
using System.Linq;

namespace Model
{
    public class InventoryItem{
        public bool IsConsumable;    
    }
    public class Inventory {
    
        public List<InventoryItem> Items;

        public Inventory()
        {
            Items = new List<InventoryItem>();
        }
        public List<InventoryItem> GetConsumables(){
            return Items.Where(i=>i.IsConsumable).ToList();
        }
    }
}