using System;
using System.Collections.Generic;
using System.Linq;

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
        public DamageType DamageType;
        public Dice Dice;

        public DamageDice(DamageType damageType, Dice dice)
        {
            this.DamageType = damageType;
            this.Dice = dice;
        }

        public int Roll()
        {
            return Dice.Roll();
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
        public List<DamageResult> PreviewDamages(List<DamageDice> damageDices)
        {
            return damageDices.Select(PreviewDamage).ToList();
        }

        public DamageResult PreviewDamage(DamageDice damageDice)
        {
            var res = Stats.GetDamageResistance(damageDice.DamageType);
            var rolledDamage = damageDice.Roll();
            var finalDamage = rolledDamage - (rolledDamage*res);
            
            var result = new DamageResult();
            result.DamageType = damageDice.DamageType;
            result.Total = finalDamage;
            return result;
        }
        
        public float GetDamageResistance(DamageType damageDiceDamageType)
        {
            var res = Stats.GetBaseDamageRessistance(damageType);
            Equipments.ForEach(e =>
            {
                //TODO: apply each equipment res bonus    
            });
            return res;
        }
        public List<DamageDice> GetDamageDices()
        {
            
        }
    }
    public class Ennemy : Character{
        public Ennemy()
        {
            IsEnnemy = true;
        }
    }
}