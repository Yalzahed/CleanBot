using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class reset : MonoBehaviour
{
    // Start is called before the first frame updatevoid
    public void ResetScene(){
        SceneManager.LoadScene("Example");
    }
}
