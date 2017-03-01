//
// KinoBokeh - Fast DOF filter with hexagonal aperture
//
// Copyright (C) 2015 Keijiro Takahashi
using System.Collections;
using UnityEngine;

namespace Kino
{
    [ExecuteInEditMode, RequireComponent(typeof(Camera))]
    public class Bokeh : MonoBehaviour
    {
        #region Public Properties

        [SerializeField]
		Transform _focusedObject;

		Transform focusedObject {
			get { return _focusedObject; }
			set { _focusedObject = value; }
        }

        [SerializeField]
        float _distance = 10.0f;

        float distance {
            get { return _distance; }
            set { _distance = value; }
        }

        [SerializeField]
        float _fNumber = 1.4f;

        float fNumber {
            get { return _fNumber; }
            set { _fNumber = value; }
        }

        [SerializeField]
        bool _useCameraFov = false;

        bool useCameraFov {
            get { return _useCameraFov; }
            set { _useCameraFov = value; }
        }

        [SerializeField]
        float _focalLength = 0.05f;

        float focalLength {
            get { return _focalLength; }
            set { _focalLength = value; }
        }

        [SerializeField]
        float _maxBlur = 0.03f;

        float maxBlur {
            get { return _maxBlur; }
            set { _maxBlur = value; }
        }

        [SerializeField]
        float _irisAngle = 0;

        float irisAngle {
            get { return _irisAngle; }
            set { _irisAngle = value; }
        }

        public enum SampleCount { Low, Medium, High, UltraHigh }

        [SerializeField]
        public SampleCount _sampleCount = SampleCount.Medium;

        SampleCount sampleCount {
            get { return _sampleCount; }
            set { _sampleCount = value; }
        }

        [SerializeField]
        bool _foregroundBlur = true;

        bool foregroundBlur {
            get { return _foregroundBlur; }
            set { _foregroundBlur = value; }
        }

        [SerializeField]
        bool _visualize;

        [SerializeField]
        float _deltaFocusTime = 0.04f;  //default - 40ms

        float deltaFocusTime
        {
            get { return _deltaFocusTime; }
            set { _deltaFocusTime = value; }
        }

        [SerializeField]
        float _minRefocusTime = 0.04f;  //default - 40ms

        float minRefocusTime
        {
            get { return _minRefocusTime; }
            set { _minRefocusTime = value; }
        }

        //[SerializeField]
        //float _worldScale = 1.0f;
        
        //float worldScale
        //{
        //    get { return _worldScale; }
        //    set { _worldScale = value; }
        //}

        #endregion

        #region Private Properties and Functions

        // the camera
        Camera cam;

        // Standard film width = 24mm
        const float filmWidth = 0.024f;

        [SerializeField] Shader _shader;
        Material _material;

        int SeparableBlurSteps {
            get {
                if (_sampleCount == SampleCount.Low) return 5;
                if (_sampleCount == SampleCount.Medium) return 10;
                if (_sampleCount == SampleCount.High) return 15;
                return 20;
            }
        }

		// --------- 2016.1.21 commented by Xinjie Wang ---------

        // This is the original function
        //float CalculateSubjectDistance()
        //{
            //if (_subject == null) return _distance;
            //var cam = GetComponent<Camera>().transform;
            //return Vector3.Dot(_subject.position - cam.position, cam.forward);
        //}

		// --------- 2016.1.21 added by Xinjie Wang ---------
        // Detect whether an object is inside the camera-view or not
        bool notOnScreen(Transform obj, Camera cam)
		{
			Vector3 screenPoint = cam.WorldToViewportPoint (obj.position);
			bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
			return !onScreen;

		}

        //private float _elapsedTime;
        //private float _lastTime;
        //private int _repeatTimes;

        //// Set a Timer to simulate refocus
        //IEnumerator Timer(float _deltaDistance)
        //{
        //    while (_repeatTimes-- > 0)
        //    {
        //        _elapsedTime = Time.time - _lastTime;
        //        float waitedSeconds = 0.04f - _elapsedTime;
        //        if (waitedSeconds < 0)
        //        {
        //            waitedSeconds = 0;
        //        }

        //        _tmpDistance += _deltaDistance;
        //        _lastTime = Time.time + waitedSeconds;
        //        Debug.Log(string.Format("Timer2 is up !!! time={0}, dis={1}", Time.time, _tmpDistance));
        //        yield return new WaitForSeconds(waitedSeconds);
        //        //_lastTime = Time.time;
        //    }
        //}

        float clamp(float _dis)
        {
            //the focus distance within human eyes' ablity is from 0.01 meters to 100 meters
            if (_dis > 100) return 100f;
            if (_dis < 0.05) return 0.05f;
            return _dis;
        }


        float _startDistance,   // 一次对焦过程中的开始景深距离
              _destDistance,    // 一次对焦过程中的目标景深距离
              _tmpDistance,     // 一次对焦过程中的某一时刻的距离
              _refocusDistance,// 一次对焦过程的总距离
              _currentDistance, // 当前帧的实际景深距离
              _deltaDistance,   // 一次对焦的单位距离（总距离/总时间）
              _startTime;        // 一次对焦的开始时间
        bool _refocusInProcess = false;       // 标识是否处于一次对焦的过程中

        void calcTempDistance()
        {
            _tmpDistance = (Time.time - _startTime) * _deltaDistance;
            if (Mathf.Abs(_tmpDistance) > Mathf.Abs(_refocusDistance))
            {
                // end refocus
                _tmpDistance = _destDistance;
                //_refocusInProcess = false;
            }
            else
                _tmpDistance += _startDistance;
        }

        float CalculateSubjectDistance()
		{
			//var cam = GetComponent<Camera> ();
			var camTrans = cam.transform;

			// Obtain current distance at this moment 
			if (_focusedObject == null || notOnScreen(_focusedObject, cam)) { // No valid focused object
				//find the central object in FOV
				Ray ray = new Ray (camTrans.position, camTrans.forward);
				RaycastHit hit = new RaycastHit ();

				if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {
					_currentDistance = Vector3.Dot (hit.point - camTrans.position, camTrans.forward);
				} else {
					_currentDistance = _distance;
				}
			}
			else { // Valid focused object
				_currentDistance = Vector3.Dot(_focusedObject.position - camTrans.position, camTrans.forward);
			}

            //Debug.Log(_tmpDistance + "   " + _currentDistance);

            // need to refocus?
            float refocusTime = Mathf.Abs(_deltaFocusTime / clamp(_currentDistance) 
                                - _deltaFocusTime / clamp(_tmpDistance));
            //if (refocusTime > 500)
            //Debug.Log("refocusTime: " + refocusTime);
            if (refocusTime > _minRefocusTime)
            { // only process refocusing when delay > 100ms
                
                if (_refocusInProcess == false)
                {
                    _startTime = Time.time;
                    _startDistance = _tmpDistance;
                    _destDistance = _currentDistance;
                    _refocusDistance = _destDistance - _startDistance;
                    _deltaDistance = _refocusDistance / refocusTime;
                    _refocusInProcess = true;

                    //calcTempDistance();
                    return _tmpDistance;
                }
                else
                {
                    // need to change dest dis value?
                    float newRefocusTime = Mathf.Abs(_deltaFocusTime / clamp(_currentDistance) 
                                            - _deltaFocusTime / clamp(_destDistance));
                    if (newRefocusTime > _minRefocusTime)
                    {
                        _startTime = Time.time;
                        _startDistance = _tmpDistance;
                        _destDistance = _currentDistance;
                        _refocusDistance = _destDistance - _startDistance;
                        _deltaDistance = _refocusDistance / newRefocusTime;
                        _refocusInProcess = true;

                        //calcTempDistance();
                        return _tmpDistance;
                    }
                    else
                    {
                        _destDistance = _currentDistance;
                        calcTempDistance();
                        return _tmpDistance;
                    }
                }


                //_repeatTimes = Mathf.FloorToInt(refocusTime / 40);
                //_refocusInProcess = true;
                //float deltaDistance = Mathf.FloatToHalf(Mathf.Abs(_currentDistance - _oldDistance) / _repeatTimes);
                //StartCoroutine(Timer(deltaDistance));
                //_oldDistance = _tmpDistance;

                //return _tmpDistance;
            }
            else
            {
                _refocusInProcess = false;
                _tmpDistance = _currentDistance;
                return _tmpDistance;
            }
		}

//		float CalculateSubjectDistance()
//		{
//			var cam = GetComponent<Camera> ();
//			var camTrans = cam.transform;
//
//			if (_focusedObject == null || notOnScreen(_focusedObject, cam)) {
//				//find the central object in FOV
//				Ray ray = new Ray (camTrans.position, camTrans.forward);
//				RaycastHit hit = new RaycastHit ();
//
//				if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {
//					return Vector3.Dot (hit.point - camTrans.position, camTrans.forward);
//				} else {
//					return _distance;
//				}
//			} 
//			else {
//				return Vector3.Dot(_focusedObject.position - camTrans.position, camTrans.forward);
//			}
//
//		}

		// just for test!!! Need to be removed when the project is released
		// Update is called once per frame
		//void Update () {
		//	//var cam = GetComponent<Camera>().transform;
		//	Debug.DrawRay (cam.transform.position, cam.transform.forward * 10, Color.red);
		//}
		// ------- END 2016.1.21 added by Xinjie Wang  ---------

        float CalculateFocalLength()
        {
            if (!_useCameraFov) return _focalLength;
            var fov = GetComponent<Camera>().fieldOfView * Mathf.Deg2Rad;
            return 0.5f * filmWidth / Mathf.Tan(0.5f * fov);
        }

        void SetUpShaderKeywords()
        {
            if (_sampleCount == SampleCount.Low)
            {
                _material.DisableKeyword("BLUR_STEP10");
                _material.DisableKeyword("BLUR_STEP15");
                _material.DisableKeyword("BLUR_STEP20");
            }
            else if (_sampleCount == SampleCount.Medium)
            {
                _material.EnableKeyword("BLUR_STEP10");
                _material.DisableKeyword("BLUR_STEP15");
                _material.DisableKeyword("BLUR_STEP20");
            }
            else if (_sampleCount == SampleCount.High)
            {
                _material.DisableKeyword("BLUR_STEP10");
                _material.EnableKeyword("BLUR_STEP15");
                _material.DisableKeyword("BLUR_STEP20");
            }
            else // SampleCount.UltraHigh
            {
                _material.DisableKeyword("BLUR_STEP10");
                _material.DisableKeyword("BLUR_STEP15");
                _material.EnableKeyword("BLUR_STEP20");
            }

            if (_foregroundBlur)
                _material.EnableKeyword("FOREGROUND_BLUR");
            else
                _material.DisableKeyword("FOREGROUND_BLUR");
        }

        void SetUpShaderParameters(RenderTexture source)
        {
            var s1 = CalculateSubjectDistance();
            _material.SetFloat("_SubjectDistance", s1);

            var f = CalculateFocalLength();
            var coeff = f * f / (_fNumber * (s1 - f) * filmWidth);
            _material.SetFloat("_LensCoeff", coeff);

            var aspect = new Vector2((float)source.height / source.width, 1);
            _material.SetVector("_Aspect", aspect);
        }

        void SetSeparableBlurParameter(float dx, float dy)
        {
            float sin = Mathf.Sin(_irisAngle * Mathf.Deg2Rad);
            float cos = Mathf.Cos(_irisAngle * Mathf.Deg2Rad);
            var v = new Vector2(dx * cos - dy * sin, dx * sin + dy * cos);
            v *= _maxBlur * 0.5f / SeparableBlurSteps;
            _material.SetVector("_BlurDisp", v);
        }

        #endregion

        #region MonoBehaviour Functions

        void OnEnable()
        {
            cam = GetComponent<Camera>();
            cam.depthTextureMode |= DepthTextureMode.Depth;
            _startDistance = _distance;
            _tmpDistance = _startDistance;
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (_material == null)
            {
                _material = new Material(_shader);
                _material.hideFlags = HideFlags.DontSave;
            }

            // Set up the shader parameters.
            SetUpShaderKeywords();
            SetUpShaderParameters(source);

            // Create temporary buffers.
            var rt1 = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
            var rt2 = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
            var rt3 = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);

            // Make CoC map in alpha channel.
            Graphics.Blit(source, rt1, _material, 0);

            if (_visualize)
            {
                // CoC visualization.
                Graphics.Blit(rt1, destination, _material, 1);
            }
            else
            {
                // 1st separable filter: horizontal blur.
                SetSeparableBlurParameter(1, 0);
                Graphics.Blit(rt1, rt2, _material, 2);

                // 2nd separable filter: skewed vertical blur (left).
                SetSeparableBlurParameter(-0.5f, -1);
                Graphics.Blit(rt2, rt3, _material, 2);

                // 3rd separable filter: skewed vertical blur (right).
                SetSeparableBlurParameter(0.5f, -1);
                Graphics.Blit(rt2, rt1, _material, 2);

                // Combine the result.
                _material.SetTexture("_BlurTex1", rt1);
                _material.SetTexture("_BlurTex2", rt3);
                Graphics.Blit(source, destination, _material, 3);
            }

            // Release the temporary buffers.
            RenderTexture.ReleaseTemporary(rt1);
            RenderTexture.ReleaseTemporary(rt2);
            RenderTexture.ReleaseTemporary(rt3);
        }

        #endregion
    }
}
