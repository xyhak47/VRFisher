//************************************************************************************
//
// Filename    :   LensCorrection.shader
// Content     :   Full screen shader
//				   This shader warps the final camera image to match the lens curvature on the Nubela.
// Created     :   January 17, 2015
// Authors     :   Xinjie Wang
//
// Copyright   :   Copyright 2014 Immersion, Inc. All Rights reserved.
//
//************************************************************************************/

Shader "Custom/IVRLensCorrection"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "" {}

		// for game special effect
		_IceTex("ice tex", 2D) = "white" {}
		_IceTrigger("ice effect trigger", Float) = 0.0
	}

		// Shader code pasted into all further CGPROGRAM blocks
	CGINCLUDE

	#include "UnityCG.cginc"

	struct v2f
	{
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
	};

	sampler2D _MainTex;

	v2f vert(appdata_img v)
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	}

	float2 _Center = float2(0, 0);
	float2 _ScaleIn = float2(0, 0);
	float2 _Scale = float2(0, 0);
	float4 _HmdWarpParam = float4(0, 0, 0, 0);
	float2 _IMSScale = float2(0, 0);

	// game effect -- add by xyh
	sampler2D _IceTex;
	fixed _IceTrigger;

	// Scales input texture coordinates for distortion.
	// ScaleIn maps texture coordinates to Scales to ([-1, 1] * scaleFactor),
	// where scaleFactor compensates input for K1 and K2, to allow full screen size to be used.
	// Scale factor that fits into screen size can be determined by solving this
	// equation for Scale: 1 = Scale * (K0 + K1 * Scale^2 + K2 * Scale^4).    
	float2 HmdWarp(float2 in01)
	{
		float2 vecFromCenter = (in01 - _Center) * _ScaleIn; // Scales to [-1, 1] 
		float  rSq = vecFromCenter.x * vecFromCenter.x + vecFromCenter.y * vecFromCenter.y;
		float2 vecResult = vecFromCenter * (_HmdWarpParam.x + _HmdWarpParam.y * rSq + _HmdWarpParam.z * rSq * rSq);
		return _Center + _Scale * vecResult;
	}

	half4 GetColor(float2 uv)
	{
		float2 tc = HmdWarp(uv);

		if (any(clamp(tc, float2(0.0, 0.0), float2(1.0, 1.0)) - tc))
			return 0;
		
		float4 sourceCol = tex2D(_MainTex, tc);
		float4 iceCol = tex2D(_IceTex, tc);
		iceCol.a = iceCol.a * _IceTrigger;
		sourceCol.rgb = lerp(sourceCol.rgb, iceCol.rgb, iceCol.a);
		return sourceCol;
	}

	half4 frag(v2f i) : COLOR
	{
		float2 tc = i.uv;
		half4 c = 0;
		c += GetColor(tc);
		return c;
	}

	ENDCG

	Subshader 
	{

		Pass
		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }

			CGPROGRAM

			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vert
			#pragma fragment frag

			ENDCG
		}

	}

	Fallback off

} // shader