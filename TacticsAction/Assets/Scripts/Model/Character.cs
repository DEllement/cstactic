using System;
using System.Collections.Generic;

namespace Model
{
    public enum AttackWith{
        Fist,
        Foots,
        Weapon,
        Spell,
        Item,
    }
    public enum DamageType
    {
        Physical,
        Fire,
        Ice,
        Electric,
        Chaos
    }
    public class DamageDice{
        DamageType DamageType;
        Dice Dice;

        public DamageDice(DamageType damageType, Dice dice)
        {
            this.DamageType = damageType;
            this.Dice = dice;
        }
    }
    
    public interface IEquipable{
        int Id {get;set;}
    }
    
    public class Equipments{
        public IEquipable Head;
        public IEquipable Neck;
        public IEquipable LeftHand;
        public IEquipable RightHand;
        public IEquipable Waist;
        public IEquipable Finger1;
        public IEquipable Finger2;
        public IEquipable Foots;
    }
    
    public class Character : ITargetable
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
        public Equipments Equipments;
        
        public List<DamageDice> GetDamageDices(AttackWith attackWith, int skillId)
        {
            switch(attackWith){
                case AttackWith.Fist:
                    return new List<DamageDice> {new DamageDice(DamageType.Physical, new Dice(1,2))};
                case AttackWith.Foots:
                    return new List<DamageDice> {new DamageDice(DamageType.Physical, new Dice(1,2))};
                case AttackWith.Weapon:
                    //TODO: get weapon stats
                    //TODO: get character stats
                    //TODO: compute stats
                    return new List<DamageDice> {new DamageDice(DamageType.Physical, new Dice(1,2))};
                case AttackWith.Spell:
                    //TODO: get spell stats
                    //TODO: get character stats
                    //TODO: compute stats
                    return new List<DamageDice> {new DamageDice(DamageType.Fire, new Dice(1,2))};
                case AttackWith.Item:
                    //TODO: get item stats
                    return new List<DamageDice> {new DamageDice(DamageType.Chaos, new Dice(1,2))};
            }
            
            return null;
        }

        public int HP => Stats.HP;
    }
    public class Ennemy : Character{
        public Ennemy()
        {
            IsEnnemy = true;
        }
    }
}