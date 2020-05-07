using System;

namespace Model
{
    public class CharacterStatus{}

    public class Aura{}
    public class Curse{}

    public class Stats{
        
        public int Level; //Level
        public int Xp;
        
        public int HP;
        public int MP;
        
        public int STR; //Affects melee PHYSICAL ATTACK & HP
        public int INT; //Affects the strength of OFFENSIVE MAGIC & MP
        public int DEX; //Affects melee and ranged ACCURACY and ATTACK. Affect MVP & Ranged ATK 
        
        public int MVP; //Affects the ability to move pts
        public int VIT; //Affects DEFENSE against attacks
        public int AGI; //Affects the ACCURACY of attacks.
        public int AVD; //Affects the ability to AVOID ATTACKS while defending.
        public int MND; //Affects the success of SPECIAL SKILLS and offensive and defensive MAGIC.
        public int RES; //Affects the ability to RESIST MAGIC.
        public int LUC; //Affects the chance to DROP ITEMS.
        
        public int ATK;
        public int DEF;
        
        public Aura[] Auras;
        public Curse[] Curses;
        
        public void AddXp(int xp, int foeLevel){
            Xp += xp;
            
            var xpMultiplier = GetXpMultiplier(foeLevel);
            
            //TODO: Check if ready to levelup
        }
        public int XpUntilNextLevel{get;set;}
        public double GetXpMultiplier(int foeLevel){
            int safeZone = (int)Math.Floor( (3+ (decimal)Level/16));
            double effDiff = Math.Max( Math.Abs(Level-foeLevel)-safeZone, 0);
            var xpMultiplier = 0.0;
            if(Level > 95)
                xpMultiplier = Math.Max( Math.Pow( (Level+5)/(Level+5+ Math.Pow(effDiff,2.5)),1.5 ), 0.01);
            else //Math.round(((Math.pow((lvl+5),1.5)) / (Math.pow((lvl+5+(Math.pow(effectivediff,2.5))),1.5)))*10000);
                xpMultiplier = Math.Round(((Math.Pow((Level+5),1.5)) / (Math.Pow((Level+5+(Math.Pow(effDiff,2.5))),1.5)))*10000);
            return xpMultiplier;
        }

        public void ComputeStats(){
            
            
        }


        public float GetBaseDamageRessistance(DamageType damageType)
        {
            return 0;
        }
    }
}