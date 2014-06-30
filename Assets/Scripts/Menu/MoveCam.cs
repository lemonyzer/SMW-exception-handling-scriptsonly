using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class MoveCam : MonoBehaviour
{
    private Vector3 originalPos;
    private Vector3 randomPos;
    private Transform camTransform;
    public Transform lookAt;

	// Mobile
	private bool mobile;
	// Mobile Gyroscope
	private Quaternion referenceAttitude;
	private Quaternion currentAttitude;
	// Mobile Accelerometer
	private float camSpeed = 5;
	private Vector3 referencePoint;
	private Vector3 currentAcceleration;


	void Awake()
	{
		/**
		 * Mobile Gyroscope
		 **/
		mobile = false;
		if (Application.platform == RuntimePlatform.Android ||
		    Application.platform == RuntimePlatform.IPhonePlayer ||
		    Application.platform == RuntimePlatform.BlackBerryPlayer ||
		    Application.platform == RuntimePlatform.WP8Player ||
		    Application.platform == RuntimePlatform.WindowsEditor)
		{
			mobile = true;
		}
	}

	/**
	 * Mobile Gyroscope, Accelerometer
	 **/
	void SetReferencPoint()
	{
		referencePoint = Input.acceleration;
	}

	// Use this for initialization
	void Start () 
    {
//		if (mobile)
//		{
//			Input.gyro.enabled = true;
//			referenceAttitude = Input.gyro.attitude;
//			currentAttitude = Quaternion.identity;
//		}
//		if(!Input.gyro.enabled)
//			mobile = false;

        camTransform = GetComponent<Camera>().transform;
	    originalPos = camTransform.position;

		randomPos = RandomPosition();
    }
	
	// Update is called once per frame
    private void Update()
    {
		/**
		 * Mobile Gyroscope, Accelerometer
		 **/
		if (mobile)
		{
			currentAcceleration = Input.acceleration;
//			Debug.Log(currentAcceleration);
			currentAcceleration.z = 0;
			currentAcceleration.y = 0;
			camTransform.position = Vector3.Slerp(camTransform.position, camTransform.position-currentAcceleration, Time.deltaTime*camSpeed);
			camTransform.LookAt(lookAt);
		}
		else
		{
	        camTransform.position = Vector3.Slerp(camTransform.position, randomPos, Time.deltaTime);
	        camTransform.LookAt(lookAt);
	        if (Vector3.Distance(camTransform.position, randomPos) < 0.5f)
	        {
				randomPos = RandomPosition();
	        }
		}
    }

	Vector3 RandomPosition()
	{
		return originalPos + new Vector3(Random.Range(-2, 2), Random.Range(-2, 2), Random.Range(-1, 1));
	}
}
