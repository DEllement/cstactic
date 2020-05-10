using System.Collections;
using System.Collections.Generic;
using Model;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class BattleTest
    {
        [Test]
        public void ItShouldPreviewTheAttack()
        {
            Character c1 = new Character{Id=1, Stats=new Stats{HP=10}, Inventory=new Inventory()};
            Ennemy e1 = new Ennemy{Id=4, Stats=new Stats{HP=10},Inventory=new Inventory()};
            
            var gameObject = new GameObject();
            var battleManager = gameObject.AddComponent<BattleManager>();
            
            battleManager.Init(new List<Character>{c1}, new List<Ennemy>{e1});
            
            var result = battleManager.PreviewActResult(ActionType.Melee, AttackWith.Weapon, 0 , c1, e1);
            
            
        }
        [Test]
        public void ItShouldDoTheAttackAndLoseHealth(){
        
        }
        [Test]
        public void ItShouldDieAfterTheAttack(){
        
        }
        [Test]
        public void ItShouldHeal(){
        
        }

        // A UnityTest behaves like a coroutine in PlayMode
        // and allows you to yield null to skip a frame in EditMode
        [UnityTest]
        public IEnumerator BattleTestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // yield to skip a frame
            yield return null;
        }
    }
}