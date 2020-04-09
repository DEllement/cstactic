using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCharacterController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameEvents.GridCharacterSelected.AddListener(OnSelected);
        GameEvents.GridCharacterDeSelected.AddListener(OnDeSelected);
    }

    private void OnSelected(GridCharacterSelectedData data)
    {
        if( data.GameObject != this.gameObject )
            return;
        
        this.GetComponent<Renderer>().material.color = Color.blue;
    }
    private void OnDeSelected(GridCharacterDeSelectedData data)
    {
        if( data.GameObject != this.gameObject )
            return;
        
        this.GetComponent<Renderer>().material.color = Color.green;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
