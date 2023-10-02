using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VegStateController : MonoBehaviour {
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private static void checkForPlayerDistance() {
        var pBody = Character_Controller.getPlayerBody();
       /* if (pBody.position) {*/ //todo the carrot should be checking (inside update) if the player is within 10f distance away from the player
                //todo if so, the carrot should move up and stay, otherwise, stay down (if it is not down already)
            
        //}
    }
}
