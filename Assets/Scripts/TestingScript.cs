using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Threading.Tasks;

public class TestingScript : MonoBehaviour
{
    private Vector3 prevPos;
    private float breadNext = 0.0f;
    public float breadRate = 0.25f; //Increase # to decrease drop rate
    protected GameObject breadCrumbs;


    string filename; 
    public bool playback; 
    public bool recordinginprogress = false; 
    bool isRewind = false;
    public bool inspect = false;
    public bool record = true; 
    
    [SerializeField] private GameObject movingCam;

    int line = 0;
    public bool start;
    //  list of Cleaning Object/ script variables 
    GameObject[] cleaningObjects;
    string[] lines;
    int movementPointer;
    public bool paused;

    Rigidbody rb;
    private List<script1> objectScripts = new List<script1>();

    private List<float[]> locations = new List<float[]>();

    private List<Color32[]> colormaps = new List<Color32[]>();
    private List<script1> colormapparent = new List<script1>();

    List<script1> objectsHit = new List<script1>();
// Robot Variables 
    public bool UserControl;
    public URDFRobot robot;

    public float light_back_height = 0.0f;
    public float light_back_roll = 0.0f;
    public float light_back_pitch = 0.0f;
    public float light_left_height = 0.0f;
    public float light_left_roll = 0.0f;
    public float light_left_pitch = 0.0f;
    public float light_right_height = 0.0f;
    public float light_right_roll = 0.0f;
    public float light_right_pitch = 0.0f;
    public float robot_x = 0.0f;
    public float robot_z = 0.0f;
    public float robot_theta = 0.0f;
    

// game objects
    private GameObject back_height, back_pitch, back_roll, left_height, left_pitch, left_roll, right_height, right_pitch, right_roll, robot_base, lightsource, lightsource2, lightsource3, roboCamera, inputcontroller,head;
    // Start is called before the first frame update
    
    void Start()
    {
        start = false;
        breadCrumbs = new GameObject("BreadCrumbs");
        Debug.Log(Application.dataPath);
        Dictionary<string, string> packages = new Dictionary<string, string>();
        packages["bishop_ros"] = "cleanbot/src/bishop_ros/urdf";
        packages["crosswing_ros"] = "cleanbot/src/crosswing_ros/urdf";

        string path = "cleanbot/cleanbot.urdf";
        string full = Path.Combine(Application.dataPath, path);


        // using .Load
        URDFLoader.Options options = new URDFLoader.Options();
        options.workingPath = Path.Combine(Application.dataPath, "cleanbot");
        Debug.Log("before " + full);
        robot = URDFLoader.Load(full, packages, options);

        // very fragile. if >1 robot or ...
        back_height = GameObject.Find("light_back_height");
        back_pitch = GameObject.Find("light_back"); // not pitch!
        back_roll = GameObject.Find("light_back_roll");
        left_height = GameObject.Find("light_left_height");
        left_pitch = GameObject.Find("light_left"); // not pitch!
        left_roll = GameObject.Find("light_left_roll");
        right_height = GameObject.Find("light_right_height");
        right_pitch = GameObject.Find("light_right"); // not pitch!
        right_roll = GameObject.Find("light_right_roll");
        robot_base = GameObject.Find("base_link");

        cleaningObjects = GameObject.FindGameObjectsWithTag("CleaningObject");
        foreach(GameObject obj in cleaningObjects){
        objectScripts.Add(obj.GetComponent<script1>());   
        }
       
        lightsource = GameObject.Find("Light playing");

        lightsource2 = GameObject.Find("Light playing 2");

        lightsource3 = GameObject.Find("Light playing 3");

        roboCamera = GameObject.Find("Camera");
        inputcontroller = GameObject.Find("Inputcontroller");

        lightsource2.transform.parent = GameObject.Find("light_back geometry box").transform;
        lightsource2.transform.localPosition = new Vector3(0f, 0f, 0f);
        lightsource2.transform.Rotate(180.0f, 0, 0);


        lightsource3.transform.parent = GameObject.Find("light_left geometry box").transform;
        lightsource3.transform.localPosition = new Vector3(0f, 0f, 0f);
        lightsource3.transform.Rotate(180.0f, 0, 0);

        lightsource.transform.parent = GameObject.Find("light_right geometry box").transform;
        lightsource.transform.localPosition = new Vector3(0f, 0f, 0f);
        lightsource.transform.Rotate(180.0f, 0, 0);
        

  



        //robot eyes
        head = GameObject.Find("head geometry cylinder");
        roboCamera.transform.parent = head.transform;
        roboCamera.transform.localPosition = new Vector3(0.2509f, 9.531f, 4.84f);
        roboCamera.transform.Rotate(27.6f, -176.848f, 0.796f);

        // colliders
        BoxCollider bc = GameObject.Find("Cylinder").AddComponent(typeof(BoxCollider)) as BoxCollider;
        CapsuleCollider cc = GameObject.Find("body geometry 33").AddComponent(typeof(CapsuleCollider)) as CapsuleCollider;
        cc.radius = 0.137f;
        cc.center = new Vector3(0, 0.426f, 0);

        rb = GameObject.Find("base_link").AddComponent(typeof(Rigidbody)) as Rigidbody;

    }

    float timeLastUpdated;

    // Update is called once per frame
    void Update()
    {

    // if(Input.GetKeyDown(KeyCode.Return)){
    //         startRewind();
    //     }

    //     if(Input.GetKeyUp(KeyCode.Return)){
    //         stopRewind();
    //     }

        // return object clicked
        if(start){
        if(!inspect){
        if(Input.GetKeyDown("space") ){
            if(Input.GetMouseButtonDown(0)){
                if( !movingCam.GetComponent<FlyCamera>().started){
            Ray ray = roboCamera.GetComponent<Camera>().ScreenPointToRay( Input.mousePosition );
            RaycastHit hit;
            if (Physics.Raycast(ray ,out hit,100)){
                Debug.Log(hit.transform.gameObject.name);
                inputcontroller.GetComponent<objectInspecter>().movetolocation(hit.transform.gameObject);
                Camera inspector = GameObject.Find("InspectorCam").GetComponent<Camera>();
                inspector.targetDisplay = 0;
                roboCamera.GetComponent<Camera>().targetDisplay = 1;
                inspect = true;
            }

            }
            else{
               Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
               RaycastHit hit;
            if (Physics.Raycast(ray ,out hit,100)){
                Debug.Log(hit.transform.gameObject.name);
                inputcontroller.GetComponent<objectInspecter>().movetolocation(hit.transform.gameObject);
                Camera inspector = GameObject.Find("InspectorCam").GetComponent<Camera>();
                inspector.targetDisplay = 0;
                movingCam.GetComponent<Camera>().targetDisplay = 1;
                inspect = true;
            }
            }
        }
        }
        }


        if(Input.GetKeyDown(KeyCode.R)){
            
        }
        //Raycast 
    
        //if ((Time.realtimeSinceStartup - timeLastUpdated) > 0.01666667)
        //{
            
        
            for (int i = 0; i < 250; i++)
            {
           // Quaternion upOffset = Quaternion.AngleAxis(-45, new Vector3(1, 0, 0));
            Vector3 trueUp =  transform.TransformDirection(GameObject.Find("light_back_light geometry box").transform.up);
            Quaternion spreadAngle_x = Quaternion.AngleAxis(Random.Range(-20f, 20f), new Vector3(1, 0, 0));
            Quaternion spreadAngle_z = Quaternion.AngleAxis(Random.Range(-20f, 20f), new Vector3(0, 0, 1));
            Quaternion spreadAngle_y = Quaternion.AngleAxis(Random.Range(-20f, 20f), new Vector3(0, 1, 0));
            Vector3 Upward = spreadAngle_x* spreadAngle_y * spreadAngle_z * trueUp;
             
            Vector3 start = GameObject.Find("light_back_light geometry box").transform.TransformVector(Random.Range(-0.5f, 0.5f), 0f, Random.Range(-0.5f, 0.5f));
                RaycastHit hit_light_back;
                //Debug.DrawRay(GameObject.Find("light_back_light geometry box").transform.position + start, Upward, Color.red, 1f);
                if (Physics.Raycast(GameObject.Find("light_back_light geometry box").transform.position + start, Upward, out hit_light_back,5))
                {
                    if (hit_light_back.collider.gameObject.tag == "CleaningObject")
                    {
                        int location = System.Array.IndexOf (cleaningObjects, hit_light_back.collider.gameObject);
                        if(!objectsHit.Contains(objectScripts[location])){
                        objectsHit.Add(objectScripts[location]);
                    }
                    objectScripts[location].onHIT(hit_light_back);
                        
                        //Debug.DrawRay(GameObject.Find("light_back geometry box").transform.position + start, Upward, Color.green);
                        //hit_light_back.collider.gameObject.GetComponent<script1>().onHIT(hit_light_back);
                        //hit_light_back.collider.gameObject.GetComponent<script1>().Stroke(pixelCoords);
                        //hit_light_back.collider.gameObject.GetComponent<script1>().Hitcount++;
                    }

                }



                Quaternion upOffset2 = Quaternion.AngleAxis(-45, new Vector3(1, 0, 0));

                Vector3 trueUp2 =   transform.TransformDirection(GameObject.Find("light_left_light geometry box").transform.up);
                //  Quaternion spreadAngle_x2 = Quaternion.AngleAxis(Random.Range(-20f, 20f), new Vector3(1, 0, 0));
                // Quaternion spreadAngle_z2 = Quaternion.AngleAxis(Random.Range(-20f, 20f), new Vector3(0, 0, 1));
                // Quaternion spreadAngle_y2 = Quaternion.AngleAxis(Random.Range(-20f, 20f), new Vector3(0, 1, 0));

                Vector3 Upward2 = spreadAngle_x*spreadAngle_y * spreadAngle_z * trueUp2;



                Vector3 start2 = GameObject.Find("light_left_light geometry box").transform.TransformVector(Random.Range(-0.5f, 0.5f), 0f, Random.Range(-0.5f, 0.5f));
                Quaternion angleOffset = Quaternion.Euler(0, 0, 0);
                RaycastHit hit_light_left;
                //todo fix the angle issue 

                //Debug.DrawRay(GameObject.Find("light_left_light geometry box").transform.position+start2, Upward2, Color.red, 0.01f);
                if (Physics.Raycast(GameObject.Find("light_left_light geometry box").transform.position + start2, Upward2, out hit_light_left,5))
                {
                    if (hit_light_left.collider.gameObject.tag == "CleaningObject")
                    {
                    //Debug.DrawRay(GameObject.Find("light_left geometry box").transform.position + start2, Upward2, Color.green);
                    //hit_light_left.collider.gameObject.GetComponent<script1>().onHIT(hit_light_left); OLD METHOD NOT EFFECIENT
                    int location = System.Array.IndexOf (cleaningObjects, hit_light_left.collider.gameObject);
                    if(!objectsHit.Contains(objectScripts[location])){
                        objectsHit.Add(objectScripts[location]);
                    }
                    objectScripts[location].onHIT(hit_light_left);
                }

                }



                Quaternion upOffset3 = Quaternion.AngleAxis(-45, new Vector3(1, 0, 0));

                Vector3 trueUp3 =  transform.TransformDirection(GameObject.Find("light_right geometry box").transform.up);
                // Quaternion spreadAngle_x3 = Quaternion.AngleAxis(Random.Range(-20f, 20f), new Vector3(1, 0, 0));
                // Quaternion spreadAngle_z3 = Quaternion.AngleAxis(Random.Range(-20f, 20f), new Vector3(0, 0, 1));
                // Quaternion spreadAngle_y3 = Quaternion.AngleAxis(Random.Range(-20f, 20f), new Vector3(0, 1, 0));

                Vector3 Upward3 = spreadAngle_x* spreadAngle_y * spreadAngle_z * trueUp3;

                Vector3 start3 = GameObject.Find("light_right geometry box").transform.TransformVector(Random.Range(-0.5f, 0.5f), 0f, Random.Range(-0.5f, 0.5f));
                RaycastHit hit_light_right;
                // Debug.DrawRay(GameObject.Find("light_right geometry box").transform.position+ start3 , Upward3*10, Color.red, 1f);
                if (Physics.Raycast(GameObject.Find("light_right geometry box").transform.position + start3, Upward3, out hit_light_right,5))
                {
                    if (hit_light_right.collider.gameObject.tag == "CleaningObject")
                    {
                    int location = System.Array.IndexOf (cleaningObjects, hit_light_right.collider.gameObject);
                    if(!objectsHit.Contains(objectScripts[location])){
                        objectsHit.Add(objectScripts[location]);
                    }
                    objectScripts[location].onHIT(hit_light_right);
                   
                    //Debug.DrawRay(GameObject.Find("light_right geometry box").transform.position + start3, Upward3, Color.green);
                    // hit_light_right.collider.gameObject.GetComponent<script1>().onHIT(hit_light_right);// OLD METHOD NOT EFFECIENT
                }
                }


            }
            if(Time.frameCount%2 == 0){
            foreach(script1 texture in objectsHit){
                texture.texture.Apply();
            }
            objectsHit.Clear();
            }
        //    timeLastUpdated = Time.realtimeSinceStartup;
        //}

        

    if(UserControl){
        robot_z = Input.GetAxis("Vertical");
        robot_x = Input.GetAxis("Horizontal");

        if (Input.GetKey("e")) // checks if 'e' is pressed
        {
            robot_theta += 50 * Time.deltaTime; //increment theta by 1 if it is rotate clockwise
        }

        if (Input.GetKey("q")) // checks if 'q' is pressed
        {
            robot_theta -= 50f * Time.deltaTime; //decrement theta by 1 if it is rotate clockwise
        }





        //Control for arms 

        if (Input.GetKey("y")) // movement in the upward direction
        {
            light_back_height += 0.1f * Time.deltaTime;

            if (light_back_height > 0.1f)
            {
                light_back_height = 0.1f;
            }
            light_left_height += 0.1f * Time.deltaTime;

            if (light_left_height > 0.1f)
            {
                light_left_height = 0.1f;
            }

            light_right_height += 0.1f * Time.deltaTime;

            if (light_right_height > 0.1f)
            {
                light_right_height = 0.1f;
            }

        }
        if (Input.GetKey("h")) // movement in the downward direction
        {
            light_back_height -= 0.1f * Time.deltaTime;

            if (light_back_height < -0.1f)
            {
                light_back_height = -0.1f;
            }
            light_left_height -= 0.1f * Time.deltaTime;

            if (light_left_height < -0.1f)
            {
                light_left_height = -0.1f;
            }

            light_right_height -= 0.1f * Time.deltaTime;

            if (light_right_height < -0.1f)
            {
                light_right_height = -0.1f;
            }

        }


        // Control for roll 

        if (Input.GetKey("j")) //  clockwise rotation 
        {
            light_back_roll -= 40f * Time.deltaTime;

            light_left_roll -= 40f * Time.deltaTime;

            light_right_roll -= 40f * Time.deltaTime;


        }

        if (Input.GetKey("g")) //counter clockwise rotation 
        {
            light_back_roll += 40f * Time.deltaTime;

            light_left_roll += 40f * Time.deltaTime;

            light_right_roll += 40f * Time.deltaTime;

        }


        // Control for Pitch

        if (Input.GetKey("t"))
        {
            light_back_pitch -= 10f * Time.deltaTime;

            light_left_pitch -= 10f * Time.deltaTime;

            light_right_pitch -= 10f * Time.deltaTime;


        }
        if (Input.GetKey("u"))
        {
            light_back_pitch += 10f * Time.deltaTime;

            light_left_pitch += 10f * Time.deltaTime;

            light_right_pitch += 10f * Time.deltaTime;


        }

        back_height.transform.localPosition = new Vector3(0.0f, light_back_height, 0.0f);
        back_roll.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, light_back_roll);
        back_pitch.transform.localRotation = Quaternion.Euler(light_back_pitch, 0.0f, 0.0f);
        left_height.transform.localPosition = new Vector3(0.0f, light_left_height, 0.0f);
        left_roll.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, light_left_roll);
        left_pitch.transform.localRotation = Quaternion.Euler(light_left_pitch, 0.0f, 0.0f);
        right_height.transform.localPosition = new Vector3(0.0f, light_right_height, 0.0f);
        right_roll.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, light_right_roll);
        right_pitch.transform.localRotation = Quaternion.Euler(light_right_pitch, 0.0f, 0.0f);
        robot_base.transform.Translate(new Vector3(robot_x, 0.0f, robot_z) * -1 * Time.deltaTime); // allows for movement of the robot in the x/z plane

        robot_base.transform.localRotation = Quaternion.Euler(0.0f, robot_theta, 0.0f); // rotates the robot 
         PlaceCrumb();
    }
            
        //to do update robot x,y position wds  
    }
    }

    private int skip = 0;
    void FixedUpdate(){
        if(start){

        if(playback && !paused){
            
            string[] movement = lines[movementPointer].Split(',');
            float result;
            float.TryParse(movement[0].Trim(), out result);
            light_back_height = result;
            back_height.transform.localPosition = new Vector3(0.0f, light_back_height, 0.0f);
            float.TryParse(movement[1].Trim(), out result);
            light_back_roll = result;
             back_roll.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, light_back_roll);
            float.TryParse(movement[2].Trim(), out result);
            light_back_pitch = result;
            back_pitch.transform.localRotation = Quaternion.Euler(light_back_pitch, 0.0f, 0.0f);
            float.TryParse(movement[3].Trim(), out result);
            light_left_height = result;
            left_height.transform.localPosition = new Vector3(0.0f, light_left_height, 0.0f);
            float.TryParse(movement[4].Trim(), out result);
            light_left_roll = result;
             left_roll.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, light_left_roll);
            float.TryParse(movement[5].Trim(), out result);
            light_left_pitch = result;
             left_pitch.transform.localRotation = Quaternion.Euler(light_left_pitch, 0.0f, 0.0f);
            float.TryParse(movement[6].Trim(), out result);
            light_right_height = result;
            right_height.transform.localPosition = new Vector3(0.0f, light_right_height, 0.0f);
            float.TryParse(movement[7].Trim(), out result);
            light_right_roll = result;
            right_roll.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, light_right_roll);
            float.TryParse(movement[8].Trim(), out result);
            light_right_pitch = result;
            right_pitch.transform.localRotation = Quaternion.Euler(light_right_pitch, 0.0f, 0.0f);
            float x_;
            float.TryParse(movement[9].Trim(), out x_);
            float y_;
            float.TryParse(movement[11].Trim(), out y_);
            float z_;
            float.TryParse(movement[10].Trim(), out z_);

            Vector3 basetras = new Vector3(x_, 0.0f, z_)*1*Time.deltaTime;
            robot_base.transform.Translate(basetras);
            robot_base.transform.localRotation = Quaternion.Euler(0.0f, y_, 0.0f);
             PlaceCrumb();

            movementPointer++;
        }
        

        if(record){
            float[] data = {
                light_back_height, 
                light_back_roll,
                light_back_pitch,
                light_left_height,
                light_left_roll,
                light_left_pitch,
                light_right_height,
                light_right_roll,
                light_right_pitch,
                robot_x,
                robot_z,
                robot_theta};
   
        locations.Add(data);

        }
        
        if(recordinginprogress){
            if(line<locations.Count){
            writeData(locations[line],filename);
            print("recording line "+ line);
            line += 1;
            
            }
            else{
                GameObject.Find("DoneRecordingPanel").GetComponent<Canvas>().enabled = true; 
                recordinginprogress = false;
                print("finished recording");
            }
            
        }

        
        }
    }

public void recordData(string file){
    record = false;
    filename = file;
    recordinginprogress = true;
    print("startint recording");
}   

public static async Task writeData(float[] data, string fileName){
    
    using (StreamWriter file = new StreamWriter(fileName +".txt", append: true)){
    foreach (float number in data){
         await file.WriteAsync(number.ToString()+", ");
    }
    await file.WriteAsync("\n");
  } 
}

public void PausePlayback(){
    paused = true;
}

public void ResumePlayback(){
    paused = false;
}

public void Playingback(){
    movementPointer = 0;
            record = false;
            UserControl = false;
            paused = false;
            // TODO file selector
            lines = System.IO.File.ReadAllLines(@"C:\Users\yasse\OneDrive\Desktop\STEM PROJECT\bishop_2\test.txt");
}

void PlaceCrumb()
    {
        if (Time.time > breadNext)
        {
            breadNext = Time.time + breadRate;
            if (transform.position != prevPos)
            {

                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = prevPos;
                cube.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f); //Increase to enlarge cube
                GameObject Parent = GameObject.Find("BreadCrumbs");
                cube.transform.parent = Parent.transform;
            }

        }
        prevPos = transform.position;
    }

}