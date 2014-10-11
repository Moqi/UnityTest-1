Shader "Custom/ShaderTestSnow" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_NormalTex ("Normal", 2D) = "bump" {}
		_Snow ("Snow Level", Range(0,1) ) = 0
		_SnowColor ("Snow Color", Color) = (1.0,1.0,1.0,1.0)
		_SnowDirection ("Snow Direction", Vector) = (0,1,0)
		_SnowDepth ("Snow Depth", Range(0,0.3)) = 0.1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf HalfLambert vertex:vert

		sampler2D _MainTex;
		sampler2D _NormalTex;
		float _Snow;
		float4 _SnowColor;
		float4 _SnowDirection;
		float _SnowDepth;

		struct Input {
			float2 uv_MainTex;
			float2 uv_NoramlTex;
			float3 worldNormal; INTERNAL_DATA
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Normal = UnpackNormal(tex2D(_NormalTex, IN.uv_NoramlTex));
			o.Albedo = c.rgb;

			if (dot(WorldNormalVector(IN, o.Normal), _SnowDirection.xyz) > lerp(1, -1, _Snow))
			{
				o.Albedo = _SnowColor.rgb;
			}


			o.Alpha = c.a;
		}

		void vert (inout appdata_full v)
		{
			float4 snow = mul (transpose(_Object2World), _SnowDirection);
			if (dot (v.normal, snow.xyz) >= lerp(1, -1, (_Snow * 2) / 3) )
			{
				v.vertex.xyz += (snow.xyz + v.normal) * _SnowDepth * _Snow;
			}
		}

		half4 LightingHalfLambert (SurfaceOutput s, half3 lightDir, half atten)
		{
			float difLight = max(0, dot (s.Normal, lightDir));
			difLight = difLight * 0.5 + 0.5;
			float4 col;
			col.rgb = s.Albedo * _LightColor0.rgb * (difLight * atten * 2);
			col.a = s.Alpha;
			return col;
		}

		ENDCG
	} 
	FallBack "Diffuse"
}
