using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace API.Events
{
    public class TurnManagerLineUpChangedData{
        public List<Character> LineUp;
        public TurnManagerLineUpChangedData(List<Character> lineUp)
        {
            this.LineUp = lineUp;
        }
    }
    public class TurnManagerLineUpChanged : UnityEvent<TurnManagerLineUpChangedData>{}
}