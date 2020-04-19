using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TurnManagerTests
    {
        // A Test behaves as an ordinary method
        [Test]
        public void ItShouldInitializeTheTurnManager()
        {
            // Use the Assert class to test conditions
            var turnManager = new TurnManager();

            var c01 = new Character{Id=1};
            var c02 = new Character{Id=2};
            var c03 = new Character{Id=3};

            var e01 = new Ennemy{Id=4};
            var e02 = new Ennemy{Id=5};
            var e03 = new Ennemy{Id=6};

            turnManager.Init(new List<Character>{ c01,c02,c03 },
                             new List<Character>{ e01,e02,e03 });
            
            var lineupList = turnManager.lineUp.ToList();
            
            Assert.AreEqual(c01.Id, lineupList[0].Id);
            Assert.AreEqual(e01.Id, lineupList[1].Id);
            Assert.AreEqual(c02.Id, lineupList[2].Id);
            Assert.AreEqual(e02.Id, lineupList[3].Id);
            Assert.AreEqual(c03.Id, lineupList[4].Id);
            Assert.AreEqual(e03.Id, lineupList[5].Id);
            
            Assert.AreEqual(6,turnManager.lineUp.Count);
        }

        [Test]
        public void ItShouldSkipTheDead(){
            var turnManager = new TurnManager();

            var c01 = new Character{Id=1, Stats=new Stats {HP=10}};
            var c02 = new Character{Id=2, Stats=new Stats {HP=10}};
            var c03 = new Character{Id=3, Stats=new Stats {HP=10}};
            var e01 = new Ennemy{Id=4, Stats=new Stats {HP=10}};
            var e02 = new Ennemy{Id=5, Stats=new Stats {HP=10}};
            var e03 = new Ennemy{Id=6, Stats=new Stats {HP=10}};

            turnManager.Init(new List<Character>{ c01,c02,c03 },
                             new List<Character>{ e01,e02,e03 });
            
            turnManager.Next();
            Assert.AreEqual(c01, turnManager.CurrentCharacter);
            turnManager.Next();
            Assert.AreEqual(e01, turnManager.CurrentCharacter);
            turnManager.Next();
            Assert.AreEqual(6,turnManager.lineUp.Count);
            Assert.AreEqual(c02, turnManager.CurrentCharacter);
            e02.Stats.HP = 0; //Simulate Kill
            turnManager.Next();
            Assert.AreNotEqual(e02, turnManager.CurrentCharacter);
            Assert.AreEqual(c03, turnManager.CurrentCharacter);
            
            Assert.AreEqual(5,turnManager.lineUp.Count);
        }
        
        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator NewTestScriptWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
