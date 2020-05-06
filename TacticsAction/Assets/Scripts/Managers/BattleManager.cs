using System.Collections;
using System.Collections.Generic;
using Model;
using UnityEngine;
using Random = System.Random;


public class CharacterActionResult{
    public List<DamageResult> Damages;
}
public class GridCellInfo{
    public int X;
    public int Y;
    public object Status;
    public Character OccupiedBy;
}

public class DamageResult{
    public DamageType DamageType;
    public int Total;
    public string AttackDetails;
}

public interface ITargetable{
    int HP {get;}
    List<DamageResult> PreviewDamage(List<DamageDice> damageDices);
}

public enum Hands{
    Left, Right
}
public class Dice{
    public int min;
    public int max;
    private Random random;

    public Dice(int min, int max)
    {
        this.min = min;
        this.max = max;
        this.random = new Random();
    }

    public int Roll(){
        return random.Next(1, max-min + 1);
    }
}

public class BattleManager : MonoBehaviour
{
    private List<Character> FriendlyCharacters;
    private List<Ennemy> EnnemyCharacters;
    
    public Character CurrentCharacter;
    
    public void Init(List<Character> friends, List<Ennemy> ennemies){
        FriendlyCharacters = friends;
        EnnemyCharacters = ennemies;
    }
    
    public void StartCharacterTurn(Character character){
                
    }
    public void EndCharacterTurn(){
        
    }
    
    public CharacterActionResult Atc(ActionType actionType, List<(int x, int y)> targets){
    
        
        return null;
    }
    public List<CharacterActionResult> PreviewActResult(ActionType actionType, AttackWith attackWith, int skillId, Character actor, List<ITargetable> targets){
    
        var damageDices = actor.GetDamageDices(attackWith, skillId);
        targets.ForEach( t=>{
            var damageResults = t.PreviewDamage(damageDices);
            //TODO: Need to send that somewhere to diplay
        });
        
        return null;
    }
    // Start is called before the first frame update
    void Start()
    {
                
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
