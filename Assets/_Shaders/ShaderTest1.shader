Shader "Custom/ShaderTest1" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_NormalTex ("Normal", 2D) = "bump" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf BaseDiffuse

		sampler2D _MainTex;
		sampler2D _NormalTex;

		struct Input {
			float2 uv_MainTex;
			float2 uv_NoramlTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Normal = UnpackNormal(tex2D(_NormalTex, IN.uv_NoramlTex));
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}

		half4 LightingBaseDiffuse (SurfaceOutput s, half3 lightDir, half atten)
		{
			float difLight = max(0, dot (s.Normal, lightDir));
			float4 col;
			col.rgb = s.Albedo * _LightColor0.rgb * (difLight * atten * 2);
			col.a = s.Alpha;
			return col;
		}

		ENDCG
	} 
	FallBack "Diffuse"
}
