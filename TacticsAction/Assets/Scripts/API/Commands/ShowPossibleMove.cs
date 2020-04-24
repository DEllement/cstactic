using Model;
using UnityEngine;
using UnityEngine.Events;

namespace API.Commands
{
    public class ShowPossibleMoveData{
        public Character Character;
        public bool SetAsChooseLocation;
        public ShowPossibleMoveData(Character character, bool setAsChooseLocation)
        {
            this.Character = character;
            this.SetAsChooseLocation = setAsChooseLocation;
        }
    }
    public class ShowPossibleMove : UnityEvent<ShowPossibleMoveData>{}
}