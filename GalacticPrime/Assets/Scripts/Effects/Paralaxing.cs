using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paralaxing : MonoBehaviour 
{

    public Transform[] backgrounds;         //array of background and foregrounds to be paralaxed
    private float[] parralaxScales;         //proportion of camera movement to backgrounds
    public float smoothing = 1f;                 //how smoothe the parralax is going to be, set above 0

    private Transform cam;                  // references main camera transform
    private Vector3 previousCamPos;         // the position of the camera in the previous frame

    //is called before Start()
    private void Awake()
    {
        //set up camera ref
        cam = Camera.main.transform;
    }

    // Use this for initialization
    void Start () 
    {
        //previous frame had the current frames camera pos
        previousCamPos = cam.position;

        //assigning coresponding parallaxScales
        parralaxScales = new float[backgrounds.Length];
        for (int i = 0; i < backgrounds.Length; i++)
        {
            parralaxScales[i] = backgrounds[i].position.z * -1;
        }
	}

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            // the parrallax is the opposite of the camera movement
            float parallax = (previousCamPos.x - cam.position.x) * parralaxScales[i];

            // set a target x position which is the current position plus the parralax
            float backgroundTargetPosX = backgrounds[i].position.x + parallax;

            //create a target position which is the background's current position with its target x position
            Vector3 backgroundTargetPos = new Vector3 (backgroundTargetPosX, backgrounds[i].position.y, backgrounds[i].position.z);

            // fade between current position and target position using lerp
            backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, backgroundTargetPos, smoothing * Time.deltaTime);
        }

        //set the previousCamPos to the camera's postion at the end of the frame
        previousCamPos = cam.position;
    }
}

