using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TurnManagerTests
    {
        // A Test behaves as an ordinary method
        [Test]
        public void NewTestScriptSimplePasses()
        {
            // Use the Assert class to test conditions
            var turnManager = new TurnManager();

            var c01 = new Character();
            var c02 = new Character();
            var c03 = new Character();

            var e01 = new Ennemy();
            var e02 = new Ennemy();
            var e03 = new Ennemy();

            turnManager.Init(new List<Character>{ c01,c02,c03 },new List<Character>{ e01,e02,e03 });


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
