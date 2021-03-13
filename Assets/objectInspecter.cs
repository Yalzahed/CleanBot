using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectInspecter : MonoBehaviour
{
    public GameObject inspected = null;
    public Vector3 location;
   [SerializeField] private Camera mainCam;
    [SerializeField] private Camera Cam;

    [SerializeField] private Camera roboCam;

    [SerializeField] private GameObject movingCam;
      private Vector3 previousPos;

    // Start is called before the first frame update
    void Start()
    {
 
    }

    // Update is called once per frame
    void Update()
    {  
        // if(Input.GetAxis("Mouse ScrollWheel")>0){
        //      Ray ray = Cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        //     Vector3 direction = ray.direction;
        //     Cam.transform.Translate(previousPos+direction*1.05f);
        // }


        if (Input.GetMouseButtonDown(1)){
            previousPos = Cam.ScreenToViewportPoint(Input.mousePosition);
        }

        if(Input.GetMouseButton(1)){
            Vector3 direction = previousPos - Cam.ScreenToViewportPoint(Input.mousePosition);
            Cam.transform.position = inspected.transform.position;

            Cam.transform.Rotate(new Vector3(1,0,0), direction.y*180);
            Cam.transform.Rotate(new Vector3(0,1,0), -direction.x*180,Space.World);
            
            print(inspected.GetComponent<Renderer>().bounds.size);
            Cam.transform.Translate(new Vector3(0,inspected.GetComponent<Renderer>().bounds.size.y/2,-Mathf.Max(inspected.GetComponent<Renderer>().bounds.size.y,inspected.GetComponent<Renderer>().bounds.size.x,inspected.GetComponent<Renderer>().bounds.size.z)*1.8f));
            
            previousPos = Cam.ScreenToViewportPoint(Input.mousePosition);

        }
        if(Input.GetKeyDown(KeyCode.Escape)){
            inspected.transform.position = location;
            mainCam.GetComponent<TestingScript>().inspect = false;
            Cam.targetDisplay =1;
            if(!movingCam.GetComponent<FlyCamera>().started){
            roboCam.GetComponent<Camera>().targetDisplay =0;
            }
            else{
                movingCam.GetComponent<Camera>().targetDisplay =0;
            }
            inspected = null;
            

        }
    }
    public void movetolocation(GameObject inspected){
       this.inspected = inspected;
        location = inspected.transform.position;
        inspected.transform.position = new Vector3(1000,1000,1000);
        Cam.transform.position = inspected.transform.position;
        Cam.transform.Translate(new Vector3(0,inspected.GetComponent<Renderer>().bounds.size.y/2,-Mathf.Max(inspected.GetComponent<Renderer>().bounds.size.y,inspected.GetComponent<Renderer>().bounds.size.x,inspected.GetComponent<Renderer>().bounds.size.z)*1.8f));
        
    }

}
