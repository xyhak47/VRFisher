Shader "Custom/FlashRed" 
{
	Properties 
	{
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM

		#pragma surface surf Lambert
		#pragma target 3.0

		sampler2D _MainTex;
		fixed4 _BlinkColor;

		struct Input 
		{
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) 
		{
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);

			fixed4 c = fixed4(1,1,1,1);
			c.rgb = lerp( c.rgb, _BlinkColor.rgb, _BlinkColor.a);  

			o.Albedo = tex.rgb * c.rgb;
			o.Alpha = tex.a * c.a;
		}
		ENDCG
	}
	
	FallBack "Diffuse"
}
