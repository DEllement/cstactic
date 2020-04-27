using System.Collections;
using System.Collections.Generic;
using Model;
using UnityEngine;


public class CharacterActionResult{

}
public class GridCellInfo{
    public int X;
    public int Y;
    public object Status;
    public Character OccupiedBy;
}

public class BattleManager : MonoBehaviour
{
    private List<Character> FriendlyCharacters;
    private List<Ennemy> EnnemyCharacters;
    private GridCellInfo[,] Grid; 
    
    public Character CurrentCharacter;
    
    public void Init(List<Character> friends, List<Ennemy> ennemies){
        FriendlyCharacters = friends;
        EnnemyCharacters = ennemies;
    }
    public void InitGrid(){
    
    }
    
    public void StartCharacterTurn(Character character){
                
    }
    public void EndCharacterTurn(){
        
    }
    
    public CharacterActionResult Atc(ActionType actionType, List<(int x, int y)> targets){
    
        return null;
    }
    public List<CharacterActionResult> PreviewActResult(ActionType actionType, List<(int x, int y)> targets){
    
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
