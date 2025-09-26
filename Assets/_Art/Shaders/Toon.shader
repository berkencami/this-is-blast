Shader "Custom/Toon"
{
	Properties
	{
		_Color("Base Color", Color) = (1,1,1,1)
		_MainTex("Main Texture", 2D) = "white" {}
		_AmbientColor("Ambient Color", Color) = (0.2,0.2,0.2,1)
		_SpecularColor("Specular Color", Color) = (1,1,1,1)
		_SpecularStrength("Specular Strength", Range(0, 2)) = 1
		_Shininess("Shininess", Range(1, 256)) = 64
		[Toggle(_SPECULAR_ON)] _EnableSpecular("Enable Specular", Float) = 1
		_SpecularThreshold("Specular Threshold", Range(0, 1)) = 0.6
		_SpecularFeather("Specular Feather", Range(0, 0.2)) = 0.02
		_DiffuseSteps("Diffuse Steps", Range(1, 8)) = 4
		_DiffuseFeather("Diffuse Feather (blend to smooth)", Range(0, 1)) = 0.0
		_RimColor("Rim Color", Color) = (1,1,1,1)
		_RimPower("Rim Power", Range(0.1, 8)) = 2
		_RimAmount("Rim Amount", Range(0, 1)) = 0.4
		[Toggle(_OUTLINE_ON)] _EnableOutline("Enable Outline", Float) = 1
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_OutlineThickness("Outline Thickness", Range(0, 1)) = 0.005
	}

	SubShader
	{
		Tags
		{
			"RenderType" = "Opaque"
			"RenderPipeline" = "UniversalPipeline"
			"Queue" = "Geometry"
		}

		Pass
		{
			Name "ForwardToon"
			Tags { "LightMode" = "UniversalForward" }

			Cull Back
			ZTest LEqual
			ZWrite On

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#pragma target 3.0

			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _SHADOWS_SOFT
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
			#pragma multi_compile_fragment _ _LIGHT_LAYERS
			#pragma multi_compile_fragment _ _LIGHT_COOKIES
			#pragma multi_compile _ _CLUSTERED_RENDERING
			#pragma shader_feature_local _SPECULAR_ON
			#pragma shader_feature_local _OUTLINE_ON

			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 worldNormal : TEXCOORD1;
				float3 viewDir : TEXCOORD2;
				float3 worldPos : TEXCOORD3;
				SHADOW_COORDS(4)
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			float4 _Color;
			float4 _AmbientColor;
			float4 _SpecularColor;
			float _SpecularStrength;
			float _Shininess;
			float _SpecularThreshold;
			float _SpecularFeather;
			float _DiffuseSteps;
			float _DiffuseFeather;
			float4 _RimColor;
			float _RimPower;
			float _RimAmount;

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.viewDir = WorldSpaceViewDir(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				TRANSFER_SHADOW(o)
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float3 normal = normalize(i.worldNormal);
				float3 viewDir = normalize(i.viewDir);

				// Main directional light (Built-in compatible variables)
				float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
				fixed3 lightColor = _LightColor0.rgb;

				// Shadows
				fixed shadow = 1.0;
				#ifdef _MAIN_LIGHT_SHADOWS
					shadow = SHADOW_ATTENUATION(i);
				#endif

				// Diffuse toon steps (procedural)
				float NdotL = saturate(dot(normal, lightDir));
				float nx = saturate(NdotL * shadow);
				float steps = max(_DiffuseSteps, 1.0);
				float denom = max(steps - 1.0, 1.0);
				float quant = floor(nx * steps + 1e-5) / denom;
				float toonDiffuse = lerp(quant, nx, saturate(_DiffuseFeather));
				fixed3 diffuse = toonDiffuse * lightColor;

				// Sample albedo early (used by specular/rim)
				fixed4 albedoSample = tex2D(_MainTex, i.uv);
				fixed3 albedo = albedoSample.rgb * _Color.rgb;

				// Specular toon threshold (procedural) and modulation to avoid white-out
				fixed3 specular = 0;
				#ifdef _SPECULAR_ON
				float3 halfVector = normalize(lightDir + viewDir);
				float NdotH = saturate(dot(normal, halfVector));
				float specPow = pow(NdotH, _Shininess);
				float specStep = smoothstep(_SpecularThreshold - _SpecularFeather, _SpecularThreshold + _SpecularFeather, specPow);
				// scale specular by light intensity and albedo luminance to keep texture visible
				float lightIntensity = toonDiffuse; // reuse diffuse factor
				float albedoLum = dot(albedo, float3(0.299, 0.587, 0.114));
				specular = specStep * _SpecularColor.rgb * _SpecularStrength * lightIntensity * saturate(albedoLum + 0.2);
				#endif

				// Rim lighting (mask with albedo to avoid washing out)
				float rim = pow(1.0 - saturate(dot(viewDir, normal)), _RimPower);
				rim = smoothstep(_RimAmount - 0.01, _RimAmount + 0.01, rim);
				fixed3 rimCol = rim * _RimColor.rgb * (albedo * 0.7 + 0.3);

				fixed3 color = (diffuse + _AmbientColor.rgb) * albedo + specular + rimCol;
				color = saturate(color);
				return fixed4(color, albedoSample.a * _Color.a);
			}
			ENDCG
		}

		// Outline pass: inverted hull extrusion along vertex normals
		Pass
		{
			Name "Outline"
			Tags { "LightMode" = "SRPDefaultUnlit" }
			Cull Front
			ZWrite On
			ZTest LEqual

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma shader_feature_local _OUTLINE_ON

			#include "UnityCG.cginc"

			float4 _OutlineColor;
			float _OutlineThickness;

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				float3 normalWS = UnityObjectToWorldNormal(v.normal);
				float3 posWS = mul(unity_ObjectToWorld, v.vertex).xyz + normalWS * _OutlineThickness;
				o.pos = UnityWorldToClipPos(float4(posWS, 1.0));
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				#ifdef _OUTLINE_ON
				return _OutlineColor;
				#else
				clip(-1);
				return 0;
				#endif
			}
			ENDCG
		}

		// Shadow casting support
		UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
	}
}

