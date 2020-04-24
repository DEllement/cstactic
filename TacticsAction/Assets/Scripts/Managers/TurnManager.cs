
using System.Collections.Generic;
using System.Linq;
using API;
using API.Commands;
using API.Events;
using Model;

public class TurnManager {
    public Character CurrentCharacter;
    public Queue<Character> lineUp;
    public int Round;
    
    //List<Character> friendly;
    //List<Character> ennemies;
    
    public List<Character> GetActiveLineUp(){
        return lineUp.ToList();
    }
    
    public TurnManager()
    {
        this.lineUp = new Queue<Character>();
        
        GameCommands.ForceNextTurn.AddListener(Execute);
    }
    
    public void Execute(ForceNextTurnData data){
        Next();
    }

    public void Init(List<Character> friendly, List<Character> ennemies){
        this.lineUp = new Queue<Character>();
        
        var i_f = 0;
        var i_e = 0;
        
        var totalFriendlyAlive = friendly.Count;
        var totalEnnemiesAlive = ennemies.Count;
        var totalCharactersAlive = totalFriendlyAlive+totalEnnemiesAlive;
        
        for(var i=0; i < totalCharactersAlive; i++){
            //Friendly
            if(i%2==0){
                if(i_f>=totalFriendlyAlive)
                    continue;
                lineUp.Enqueue(friendly[i_f++]);  
            }
            //Ennemy
            else{
                if(i_e>=totalEnnemiesAlive)
                    continue;
                lineUp.Enqueue(ennemies[i_e++]);    
            }
        }
        
        GameEvents.TurnManagerInitialized.Invoke(new TurnManagerInitializedData(lineUp.ToList()));
        //CurrentCharacter = lineUp.Peek();
    }
    
    public void Next(){
        if (lineUp.Peek().Stats.HP <= 0){
            lineUp.Dequeue();
            Next();
            return;
        }
        if(CurrentCharacter != null)
            lineUp.Enqueue(CurrentCharacter);
        GameEvents.TurnManagerLineUpChanged.Invoke(new TurnManagerLineUpChangedData(lineUp.ToList()));
        CurrentCharacter = lineUp.Dequeue();
        
        if(CurrentCharacter.IsEnnemy || CurrentCharacter.IsGuest || CurrentCharacter.AutoCombat)
            GameCommands.ExecuteCharacterTurn.Invoke(null);
        else
            GameCommands.SelectCharacter.Invoke(null);
        
        GameEvents.CharacterTurnStarted.Invoke(new CharacterTurnStartedData(CurrentCharacter));
    }
    
    public void OnClick(){
        Next();      
    }
}

