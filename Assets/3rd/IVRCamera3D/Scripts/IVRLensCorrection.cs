/************************************************************************************

Filename    :   IVRLensCorrection.cs
Content     :   Full screen image effect. 
				This script is used to add full-screen lens correction on a camera
				component
Created     :   January 17, 2016
Authors     :   Xinjie Wang

Copyright   :   Copyright 2016 Immersion Co., Ltd. All Rights reserved.

Use of this software is subject to the terms of the Immersion LLC license
agreement provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

************************************************************************************/

using UnityEngine;

[RequireComponent(typeof(Camera))]

//-------------------------------------------------------------------------------------
// ***** IVRLensCorrection
//
// IVRLensCorrection contains the variables required to set material properties
// for the lens correction image effect.
//
public class IVRLensCorrection : MonoBehaviour
{
    [System.NonSerialized]
    public Vector2 _Center = new Vector2(0.5f, 0.5f);
    [System.NonSerialized]
    public Vector2 _ScaleIn = new Vector2(1.0f, 1.0f);
    [System.NonSerialized]
    public Vector2 _Scale = new Vector2(1.0f, 1.0f);
    [System.NonSerialized]
    public Vector4 _HmdWarpParam = new Vector4(1.0f, 0.0f, 0.0f, 0.0f);

    // We will search for camera controller and set it here for access to its members
    private IVRCameraController CameraController = null;

    /// Provides a shader property that is set in the inspector
    /// and a material instantiated from the shader
    public Material material;

    void Start()
    {
        // Get the IVRCameraController
        CameraController = gameObject.transform.parent.parent.GetComponent<IVRCameraController>();

        if (CameraController == null)
            Debug.LogWarning("WARNING: IVRCameraController not found!");
    }

    // OnRenderImage
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (CameraController.LensCorrection == true)
        {
            SetPortraitProperties(false);

            material.SetVector("_HmdWarpParam", _HmdWarpParam);
        }

        if (material != null)
        {
            // Render with distortion
            Graphics.Blit(src, dest, material);
        }
        else
        {
            // Pass through
            Graphics.Blit(src, dest);
        }

    }

    private void SetPortraitProperties(bool portrait)
    {
        if (portrait == true)
        {
            Vector2 tmp = Vector2.zero;
            tmp.x = _Center.y;
            tmp.y = _Center.x;
            material.SetVector("_Center", tmp);
            tmp.x = _Scale.y;
            tmp.y = _Scale.x;
            material.SetVector("_Scale", tmp);
            tmp.x = _ScaleIn.y;
            tmp.y = _ScaleIn.x;
            material.SetVector("_ScaleIn", tmp);
        }
        else
        {
            material.SetVector("_Center", _Center);
            material.SetVector("_Scale", _Scale);
            material.SetVector("_ScaleIn", _ScaleIn);
        }
    }
}