using System.Collections.Generic;
using Model;
using UnityEngine;
using UnityEngine.Events;

namespace API.Events
{
    public class TurnManagerInitializedData{
        public List<Character> LineUp;
        public TurnManagerInitializedData(List<Character> lineUp)
        {
            this.LineUp = lineUp;
        }
    }
    public class TurnManagerInitialized : UnityEvent<TurnManagerInitializedData>{}
}