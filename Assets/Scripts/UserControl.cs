using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UserControl : MonoBehaviour
{
    public GameObject robot;
    public Canvas canvas; 
    public Canvas selectcontrol;
    // Start is called before the first frame update
    public void UserControlflags(){
        TestingScript script = robot.GetComponent<TestingScript>();
        script.UserControl = true;
        script.record = true;
        canvas.GetComponent<Canvas>().enabled = true;
        selectcontrol.enabled = false;
        
        script.start = true;
    }

    public void Premadeflags(){
        TestingScript script = robot.GetComponent<TestingScript>();
        script.UserControl = false;
        script.playback = true;
        script.Playingback();
        canvas.GetComponent<Canvas>().enabled = true;
        selectcontrol.enabled = false;

        script.start = true;
    }
}
