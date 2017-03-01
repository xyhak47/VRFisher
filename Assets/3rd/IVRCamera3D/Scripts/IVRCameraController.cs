//wnq 4.3.2015
//xinjie wang 3.15.2016
using UnityEngine;
using System.Collections;

public class IVRCameraController : MonoBehaviour
{
    public static IVRCameraController _instance;

#if CHILDREN_VR
    // Config IPD parameters for Children VR
    public float IPD = 61f;
#else
    // Config IPD parameters for Nebula VR
    public float IPD = 64f;
#endif

    public float EyeHeight = 0.15f;
    public Camera originalCamera;

    // Turn lens distortion on/off; use Chromatic Aberration in lens distortion calculation
    //[HideInInspector]
    public bool LensCorrection = true;

#if CHILDREN_VR
    public bool nightShiftOn = true;
    public bool copyrightOn = true;
#endif

    [HideInInspector]
    public bool Chromatic = false;
    [HideInInspector]
    public static Quaternion _gyroAttitude;

    // PRIVATE MEMBERS
    //private bool UpdateCamerasDirtyFlag = false;
    private Camera leftCamera, rightCamera = null;
	private GameObject _head;
    //private GameObject _3rdCamera;
    //private float AspectRatio = 1.0f;
    private float DistK0, DistK1, DistK2, DistK3 = 0.0f;    // lens distortion parameters

#if CHILDREN_VR
    private bool rotation_fix = false;
#endif

    void Awake()
    {
        _instance = this;
    }
    // Use this for initialization
    void Start ()
	{
        // Get the required Rift infromation needed to set cameras
        InitCameraControllerVariables();

        UpdateCameras();

#if CHILDREN_VR

        PlayerPrefs.SetInt("UnitySelectMonitor", 1);

        if (nightShiftOn)
        {
            GameObject nightShift = (GameObject) Resources.Load("nightShift");
            GameObject mynightShift = Instantiate(nightShift) as GameObject;
            mynightShift.transform.parent = this.transform;
            mynightShift.transform.localPosition = nightShift.transform.position;
            mynightShift.transform.localRotation = nightShift.transform.rotation;
            mynightShift.transform.localScale = nightShift.transform.localScale;
        }

        if (copyrightOn)
        {
            GameObject copyrightProtection = (GameObject)Resources.Load("copyrightProtection");
            GameObject mycopyrightProtection = Instantiate(copyrightProtection) as GameObject;
            mycopyrightProtection.transform.parent = this.transform;
            mycopyrightProtection.transform.localPosition = copyrightProtection.transform.position;
            mycopyrightProtection.transform.localRotation = copyrightProtection.transform.rotation;
            mycopyrightProtection.transform.localScale = copyrightProtection.transform.localScale;
        }
#endif

    }
	
	// Update is called once per frame
	void Update ()
    {

        //InitCameraControllerVariables();
        //ConfigureCameraLensCorrection(ref _leftCamera);
        //ConfigureCameraLensCorrection(ref _rightCamera);

        //IVRDevice.GetOrientation (0, ref _gyroAttitude);

#if CHILDREN_VR
        if (Input.GetKey(KeyCode.Alpha3) && rotation_fix == false)
        {
            IVRDevice.ResetOrientation(0);
            rotation_fix = true;
        }
#endif

        IVRDevice.GetPredictedOrientation (0, ref _gyroAttitude);
		transform.localRotation = _gyroAttitude;

	}

	void UpdateCameras ()
	{
        //leftCamera = originalCamera;
        //_leftCamera.name = "Camera_left";
        leftCamera = Instantiate(originalCamera, Vector3.zero, Quaternion.identity) as Camera;
        leftCamera.name = "leftCamera";
        rightCamera = Instantiate(originalCamera, Vector3.zero, Quaternion.identity) as Camera;
		rightCamera.name = "rightCamera";

        originalCamera.enabled = false;


        _head = new GameObject ("head");
		_head.transform.parent = this.transform;
		_head.transform.localPosition = Vector3.zero;
		_head.transform.localRotation = Quaternion.identity;
		_head.AddComponent<AudioListener>();

        originalCamera.transform.parent = _head.transform;
		leftCamera.transform.parent = _head.transform;
		rightCamera.transform.parent = _head.transform;
        //if(_is3rdCamera){
        //	_3rdCamera = Instantiate(_leftCamera);
        //	_3rdCamera.name = "3rdCamera";
        //	_3rdCamera.transform.parent = _head.transform;
        //	_3rdCamera.transform.localPosition = new Vector3 (0, _neckHight, 0);
        //	_3rdCamera.transform.localRotation = Quaternion.identity;
        //	_3rdCamera.GetComponent<Camera>().rect = new Rect(0f, 0, 0.5f, 1);
        //}

        originalCamera.transform.localPosition = new Vector3(0, EyeHeight, 0);
		leftCamera.transform.localPosition = new Vector3 (IPD/2000 *(-1), EyeHeight, 0);
		rightCamera.transform.localPosition = new Vector3 (IPD/2000 *1, EyeHeight, 0);
		leftCamera.transform.localRotation = Quaternion.identity;
		rightCamera.transform.localRotation = Quaternion.identity;
        //if (!_is3rdCamera)
        //{
            originalCamera.GetComponent<Camera>().rect = new Rect(0.25f, 0, 0.5f, 1);
            leftCamera.GetComponent<Camera>().rect = new Rect(0, 0, 0.5f, 1);
            rightCamera.GetComponent<Camera>().rect = new Rect(0.5f, 0, 0.5f, 1);
        //}
        //else {
        //    _leftCamera.GetComponent<Camera>().rect = new Rect(0.5f, 0, 0.25f, 1);
        //    _rightCamera.GetComponent<Camera>().rect = new Rect(0.75f, 0, 0.25f, 1);
        //}

        //Screen.SetResolution (3840,1080, true);

        ConfigureCameraLensCorrection(ref leftCamera);
        ConfigureCameraLensCorrection(ref rightCamera);
    }


    // InitCameraControllerVariables
    // Made public so that it can be called by classes that require information about the
    // camera to be present when initing variables in 'Start'
    public void InitCameraControllerVariables()
    {
        // Get the IPD value (distance between eyes in meters)
        //IVRDevice.GetIPD(ref ipd);

        // Get the values for both IPD and lens distortion correction shift. We don't normally
        // need to set the PhysicalLensOffset once it's been set here.
        //IVRDevice.CalculatePhysicalLensOffsets(ref LensOffsetLeft, ref LensOffsetRight);

        // Using the calculated FOV, based on distortion parameters, yeilds the best results.
        // However, public functions will allow to override the FOV if desired
        //VerticalFOV = IVRDevice.VerticalFOV();

        // Store aspect ratio as well
        //AspectRatio = IVRDevice.CalculateAspectRatio();

        //Since we haven't setup this, use constant numbers instead.
        IVRDevice.GetDistortionCorrectionCoefficients(ref DistK0, ref DistK1, ref DistK2, ref DistK3);


        // Check to see if we should render in portrait mode
        //if (PortraitMode != true)
            //PortraitMode = OVRDevice.RenderPortraitMode();

        //PrevPortraitMode = false;

        // Get our initial world orientation of the cameras from the scene (we can grab it from 
        // the set FollowOrientation object or this OVRCameraController gameObject)
        //if (FollowOrientation != null)
            //OrientationOffset = FollowOrientation.rotation;
        //else
            //OrientationOffset = transform.rotation;
    }

    void ConfigureCameraLensCorrection(ref Camera camera)
    {
        // Get the distortion scale and aspect ratio to use when calculating distortion shader
        float distortionScale = 1.0f / IVRDevice.DistortionScale();
        float aspectRatio = IVRDevice.CalculateAspectRatio();

        // These values are different in the SDK World Demo; Unity renders each camera to a buffer
        // that is normalized, so we will respect this rule when calculating the distortion inputs
        float NormalizedWidth = 1.0f;
        float NormalizedHeight = 1.0f;

        IVRLensCorrection lc = camera.GetComponent<IVRLensCorrection>();

        lc.enabled = true;

        lc._Scale.x = (NormalizedWidth / 2.0f) * distortionScale;
        lc._Scale.y = (NormalizedHeight / 2.0f) * distortionScale * aspectRatio;
        lc._ScaleIn.x = (2.0f / NormalizedWidth);
        lc._ScaleIn.y = (2.0f / NormalizedHeight) / aspectRatio;
        lc._HmdWarpParam.x = DistK0;
        lc._HmdWarpParam.y = DistK1;
        lc._HmdWarpParam.z = DistK2;
    }
}
