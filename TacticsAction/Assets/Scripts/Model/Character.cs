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
        
        public List<IEquipable> ToList(){
            return new List<IEquipable>{
                Head,
                Neck,
                LeftHand,
                RightHand,
                Waist,
                Finger1,
                Finger2,
                Foots
            };
        }
    }
    
    public class Character : IDamageable
    {
        public int Id;
        public string Name;
        public int Level;
        public string Class;
        public bool IsEnnemy;
        public bool IsGuest;
        public bool AutoCombat;
        public GridCellDir FacingDir;
        
        //Stats
        public Stats Stats;
        public Inventory Inventory;
        public Equipments Equipments;
        
        public Character(){
            DefaultMove = ActionType.MoveWalk;
            DefaultMoveRange = (0,2);
        }
        
        public List<DamageDice> GetDamageDices(AttackWith attackWith, int skillId)
        {
            switch(attackWith){
                case AttackWith.Fist:
                    return new List<DamageDice> {new DamageDice(DamageType.Physical, new Dice(1,5))};
                case AttackWith.Foots:
                    return new List<DamageDice> {new DamageDice(DamageType.Physical, new Dice(1,5))};
                case AttackWith.Weapon:
                    //TODO: get weapon stats
                    //TODO: get character stats
                    //TODO: compute stats
                    return new List<DamageDice> {new DamageDice(DamageType.Physical, new Dice(1,5))};
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

        public int HPMax => Stats.HPMax;
        public int HP => Stats.HP;
        
        public (int s, int e) DefaultMoveRange { get; set; }
        public ActionType DefaultMove { get; set; }

        public List<DamageResult> DoDamages(List<DamageDice> damageDices)
        {
            return damageDices.Select(DoDamage).ToList();
        }
        public List<DamageResult> PreviewDamages(List<DamageDice> damageDices)
        {
            return damageDices.Select(PreviewDamage).ToList();
        }

        public DamageResult PreviewDamage(DamageDice damageDice)
        {
            var res = GetDamageResistance(damageDice.DamageType);
            return new DamageResult {
                DamageType = damageDice.DamageType,
                Min = damageDice.Dice.min - (damageDice.Dice.min*res),
                Max = damageDice.Dice.max - (damageDice.Dice.max*res)
            };
        }
        public DamageResult DoDamage(DamageDice damageDice)
        {
            var res = GetDamageResistance(damageDice.DamageType);
            var roll = damageDice.Dice.Roll();
            var damage = roll - (roll*res);
            
            Stats.HP -= (int)damage;
            
            return new DamageResult {
                DamageType = damageDice.DamageType,
                Total = damage,
                TargetHP = Stats.HP,
                AttackDetails = damage + " " +  damageDice.DamageType + " Damage",
            };
        }
        
        public float GetDamageResistance(DamageType damageType)
        {
            var res = Stats.GetBaseDamageRessistance(damageType);
            Equipments.ToList().ForEach(e =>
            {
                //e.StatsModifiers
            });
            return res;
        }
    }
    public class Ennemy : Character{
        public Ennemy()
        {
            IsEnnemy = true;
        }
    }
}