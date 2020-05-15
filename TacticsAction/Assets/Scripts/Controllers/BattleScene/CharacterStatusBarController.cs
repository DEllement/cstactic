using System.Collections;
using System.Collections.Generic;
using Model;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStatusBarController : MonoBehaviour
{
    private Text _hpPts;
    private Text _mpPts;
    private Text _name;
    private Text _lvl;
    private Text _class;
    private Image _hpBar;
    private Image _mpBar;
    
    private bool _initialized;
    // Start is called before the first frame update
    void Init()
    {
        _hpPts = transform.Find("HpPts").GetComponent<Text>();    
        _mpPts = transform.Find("MpPts").GetComponent<Text>();    
        _name  = transform.Find("Name").GetComponent<Text>();    
        _lvl   = transform.Find("Lvl").GetComponent<Text>();    
        _class = transform.Find("Class").GetComponent<Text>();    
        _hpBar = transform.Find("HpBar").GetComponent<Image>();    
        _mpBar = transform.Find("MpBar").GetComponent<Image>();    
        _initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Character _character;
    public void ShowStatus(Character character)
    {
        if(!_initialized)
            Init();
        
        _character = character;
        
        _hpPts.text = _character.Stats.HP + " / " + _character.Stats.HPMax;
        _mpPts.text = _character.Stats.MP + " / " + _character.Stats.MPMax;
        _name.text = _character.Name;
        _lvl.text = _character.Level.ToString();
        _class.text = _character.Class.ToString();
        
        gameObject.SetActive(true);
    }
    public void HideStatus(){
        _character = null;
        gameObject.SetActive(false);
    }
}
