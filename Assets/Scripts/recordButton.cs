using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class recordButton : MonoBehaviour
{
    // Start is called before the first frame update
   [SerializeField] GameObject main;

    public bool recordData = false;
   
   private string fileName;
void Update(){
    if(recordData){clicked();}
    
}

public void clicked(){
       fileName="test";

       print("buttonclicked");

        main.GetComponent<TestingScript>().recordData(fileName);
    }
}