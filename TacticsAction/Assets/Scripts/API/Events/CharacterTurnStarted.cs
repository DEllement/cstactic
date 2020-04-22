using Model;
using UnityEngine;
using UnityEngine.Events;

namespace API.Events
{
    public class CharacterTurnStartedData{
        public Character Character;
        public CharacterTurnStartedData(Character character)
        {
            this.Character = character;
        }
    }
    public class CharacterTurnStarted : UnityEvent<CharacterTurnStartedData>{}
}