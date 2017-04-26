Shader "Custom/FlashRed" 
{
	Properties 
	{
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BodyColor ("Body Color", Color) = (1,1,1,1)

		_Caustics("Caustics Texture (RGBA)", 2D) = "white" {}
		_CausticsCoord("Tiling(XY) Offset(ZW) - Overrides texture coords", Vector) = (0.6,0.3,0,0)
		_CausticsSpeed("Speed", Float) = 2.5
		_CausticsIntensity0("Intensity A", Range(0, 1)) = 1
		_CausticsIntensity1("Intensity B", Range(0, 1)) = 0.05
		_CausticsPosition0("Position A (World Y)", Float) = 0
		_CausticsPosition1("Position B (World Y)", Float) = 10
		_CausticsBoost( "Boost", Range(0,10)) = 10
	}

	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		Cull Off

		CGPROGRAM

		#pragma surface surf CustomLambert fullforwardshadows 

		#pragma target 3.0

		sampler2D _MainTex;
		fixed4 _BlinkColor;
		fixed4 _BodyColor;

		sampler2D _Caustics;
		float4 _CausticsCoord;
		fixed _CausticsSpeed;
		fixed _CausticsIntensity0;
		fixed _CausticsIntensity1;
		fixed _CausticsPosition0;
		fixed _CausticsPosition1;
		fixed _CausticsBoost;


		struct SurfaceOutputCustom
		{
			fixed3 Albedo;
			fixed3 Normal;
			fixed3 Emission;
			half Specular;
			fixed Gloss;
			fixed Alpha;
			fixed3 worldPos;
			fixed3 worldNormal;
		};

		half4 LightingCustomLambert(SurfaceOutputCustom s, half3 lightDir, half atten)
		{				
			half diffuseReflection = dot(s.Normal, lightDir);
			fixed lightColor = diffuseReflection * atten;


			fixed4 caustics0 = tex2D(_Caustics, s.worldPos.zx * _CausticsCoord.xy + float2(_CausticsCoord.z + (_Time.x * _CausticsSpeed), _CausticsCoord.w));
			fixed4 caustics1 = tex2D(_Caustics, s.worldPos.zx * _CausticsCoord.xy + float2(_CausticsCoord.z - (_Time.x * _CausticsSpeed), _CausticsCoord.w));
			fixed4 caustics2 = tex2D(_Caustics, s.worldPos.zx * _CausticsCoord.xy + float2(_CausticsCoord.z, _CausticsCoord.w + (_Time.x * _CausticsSpeed)));
			fixed4 caustics3 = tex2D(_Caustics, s.worldPos.zx * _CausticsCoord.xy + float2(_CausticsCoord.z, _CausticsCoord.w - (_Time.x * _CausticsSpeed)));

			fixed3 caustics = ( ( saturate(caustics0.r + caustics1.g + caustics2.b + caustics3.a) * ( _CausticsBoost + 1 ) ) + _CausticsBoost );

			fixed causticsIntensity = clamp((s.worldPos.y - _CausticsPosition0) / (_CausticsPosition1 - _CausticsPosition0), 0, 1);
			causticsIntensity = lerp(_CausticsIntensity0, _CausticsIntensity1, causticsIntensity);

			fixed3 light = lightColor * _LightColor0;

			fixed3 ambient = (ShadeSH9(half4(s.worldNormal, 1))) * atten;

			ambient = lerp( ambient, ambient * caustics, causticsIntensity );

			half4 c;

			//c.rgb = s.Albedo * ( light + ambient );
			c.rgb = s.Albedo * ambient ;

			c.a = s.Alpha;
			
			return c;
		}


		struct Input
		{
			float2 uv_MainTex;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		void surf (Input IN, inout SurfaceOutputCustom o) 
		{
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);

			if(tex.a < 0.5) discard;

			fixed4 c = _BodyColor;
			c.rgb = lerp( c.rgb, _BlinkColor.rgb, _BlinkColor.a);  

			o.worldNormal = WorldNormalVector(IN, o.Normal);
			o.worldPos = IN.worldPos;
			o.Albedo = tex.rgb * c.rgb;
			o.Alpha = tex.a;
		}

		ENDCG
	}
	
	FallBack "Diffuse"
}
