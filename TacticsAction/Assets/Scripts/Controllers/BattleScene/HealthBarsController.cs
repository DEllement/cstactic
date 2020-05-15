using System.Collections;
using System.Collections.Generic;
using Model;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarsController : MonoBehaviour
{
    public class HealthBarInfo{
        public IDamageableController damageableCtrl;
        public GameObject healthBar;
        
        public int potentialMinDamage;
        public int potentialMaxDamage;

        public HealthBarInfo(GameObject healthBar,IDamageableController damageableCtrl, int potentialMinDamage, int potentialMaxDamage)
        {
            this.damageableCtrl = damageableCtrl;
            this.healthBar = healthBar;
            this.potentialMinDamage = potentialMinDamage;
            this.potentialMaxDamage = potentialMaxDamage;
        }
    }
    public class DamageableTargetInfo{
        public IDamageableController damageableCtrl;
        public int minDamage;
        public int maxDamage;
    }
    
    [SerializeField] public GameObject healthBar;
    [SerializeField] public Vector3 positionOffset = Vector3.zero;
    private Camera _mainCam;
    
    // Start is called before the first frame update
    void Start()
    {
        _mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void LateUpdate()
    {
        _healthBars?.ForEach(d=>{
            d.healthBar.transform.position = _mainCam.WorldToScreenPoint(d.damageableCtrl.GameObject.transform.position+positionOffset);
        });
    }
    
    private List<HealthBarInfo> _healthBars;
    
    public void ShowHealthStatus(List<DamageableTargetInfo> items){
        HideHealthStatus();
        _healthBars = new List<HealthBarInfo>();
        items.ForEach( d=>{
            var hb = Instantiate(healthBar, gameObject.transform, true);
            hb.transform.GetChild(0).GetComponent<Image>().fillAmount = (d.damageableCtrl.Damageable.HP/d.damageableCtrl.Damageable.HPMax );
            hb.transform.GetChild(1).GetComponent<Text>().text = d.damageableCtrl.Damageable.HP + "/" + d.damageableCtrl.Damageable.HPMax;
            _healthBars.Add(new HealthBarInfo(hb, d.damageableCtrl, d.minDamage, d.maxDamage ));
        });
    }
    public void HideHealthStatus()
    {
        _playPotentialDamageAnimation = false;
        _healthBars?.ForEach(d=>{
            Destroy(d.healthBar);
        });
        _healthBars = null;
    }
    
    public void ShowDamagePreviews(List<DamageableTargetInfo> items){
        HideHealthStatus();
        ShowHealthStatus(items);
        
        _playPotentialDamageAnimation = true;
        StartCoroutine( PlayPotentialDamageAnimation() );
    }
    
    private bool _playPotentialDamageAnimation;
    private float updateSpeed = 0.5f;
    private IEnumerator PlayPotentialDamageAnimation(){
        
        float elapsed = 0f;
        bool invert = false;
        
        while(_playPotentialDamageAnimation){
            if(invert)
                elapsed -= Time.deltaTime;
            else
                elapsed += Time.deltaTime;
            
            _healthBars?.ForEach(d=>{
                float currPerc = (float)d.damageableCtrl.Damageable.HP/d.damageableCtrl.Damageable.HPMax;
                float nextPerc = (float)(d.damageableCtrl.Damageable.HP-d.potentialMaxDamage)/d.damageableCtrl.Damageable.HPMax;
                
                var bloodSprite = d.healthBar.transform.GetChild(0).GetComponent<Image>();
                bloodSprite.fillAmount = Mathf.Lerp(currPerc, nextPerc, elapsed /updateSpeed );
            });
            
            if(elapsed >= updateSpeed)
                invert = true;
            if(elapsed <= 0)
                invert = false;
            
            yield return null;
        }
    
    }
}
