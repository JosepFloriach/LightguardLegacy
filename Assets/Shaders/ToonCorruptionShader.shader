Shader "Custom/ToonCorruptionShader" {
	Properties 
    {
    	//Base Parameters
		[MaterialToggle(_OUTL_ON)] _Outl ("Outline", Float) = 0 				
		_MainTex ("Detail", 2D) = "white" {}        								
		[MaterialToggle(_COLOR_ON)] _TintColor ("Enable Color Tint", Float) = 0 	
		_Color ("Base Color", Color) = (1,1,1,1)									
		[MaterialToggle(_VCOLOR_ON)] _VertexColor ("Enable Vertex Color", Float) = 0      
		_Brightness ("Brightness 1 = neutral", Float) = 1.0								
		[MaterialToggle(_DS_ON)] _DS ("Enable DoubleSided", Float) = 0				
		[Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull mode", Float) = 2		
		_OutlineColor ("Outline Color", Color) = (0.5,0.5,0.5,1.0)					
		_Outline ("Outline width", Float) = 0.01									
		[MaterialToggle(_ASYM_ON)] _Asym ("Enable Asymmetry", Float) = 0        	
		_Asymmetry ("OutlineAsymmetry", Vector) = (0.0,0.25,0.5,0.0)     			
		[MaterialToggle(_TRANSP_ON)] _Trans ("Enable Transparency", Float) = 0   	
		[Enum(TRANS_OPTIONS)] _TrOp ("Transparency mode", Float) = 0                
		
		//Corruption Parameters
		[MaterialToggle(_CORR_ON)] _Corruption ("Enable Corruption", Float) = 1 
		_CorruptionTexture ("Corruption Texture", 2D) = "white" {}  
		_CorruptionColor ("Corruption Color", Color) = (0.5,0.5,0.5,0.3)
        _OutlineSize ("Corruption Wave Size", Float) = 10 
        _YCutOut ("Y CutOut", Float) = 1000
        _DissolveNoise ("Dissolve Noise", 2D) = "white" {}													
		_TileSize ("NoiseTilesRatio", Range(0,1.0)) = 0.15							
		_OriginCorruption ("OriginCorruption", Vector) = (0.0,0.0,0.0,0.0)     		
		
		//Lighting Parameters
		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {} 
		
    }
   
    Subshader 
    {
    	Tags { "RenderType"="Opaque" }
		LOD 200
    	ZWrite On
	   	Cull [_Cull]
		Lighting Off
		Fog { Mode Off }
		
		
        Pass 
        {
            Name "BASE"
            Tags { "LightMode" = "ForwardBase" }
            CGPROGRAM
                #pragma vertex vert_surf
                #pragma fragment frag_surf
                #pragma fragmentoption ARB_precision_hint_fastest
                #pragma lighting ToonRamp exclude_path:prepass
				#pragma multi_compile_fog
				#pragma multi_compile_fwdbase
				#pragma multi_compile _COLOR_OFF _COLOR_ON
				#include "HLSLSupport.cginc"
				#include "UnityShaderVariables.cginc"
                #pragma glsl_no_auto_normalization
                #pragma multi_compile _COLOR_OFF _COLOR_ON
                #pragma multi_compile _CORR_OFF _CORR_ON

				#define UNITY_PASS_FORWARDBASE
				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				#include "AutoLight.cginc"
                #define INTERNAL_DATA
				#define WorldReflectionVector(data,normal) data.worldRefl
				#define WorldNormalVector(data,normal) normal
				
				#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
				#endif
				
				sampler2D _Ramp;
                sampler2D _MainTex;
				half4 _MainTex_ST;
				
				#if _CORR_ON
			    sampler2D _CorruptionTexture;
				half4 _CorruptionTexture_ST;
				fixed4 _CorruptionColor;
       		    float _OutlineSize;
        		float _YCutOut;
                sampler2D _DissolveNoise;
                half4 _DissolveNoise_ST;
				float _TileSize;
				float _CorruptionAlpha = 0.3;
				float4 _OriginCorruption;
				#endif

                fixed _Brightness;
                
                #if _COLOR_ON
                fixed4 _Color;
                #endif
				
                struct appdata_base0 
				{
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					float4 texcoord : TEXCOORD0;
				};
				
				#ifdef LIGHTMAP_OFF
				struct v2f_surf {
				  float4 pos : SV_POSITION;
				  half2 uvn : TEXCOORD0;
		          half2 uv : TEXCOORD1;
		          #if _DISS_ON
		            half2 disuv : TEXCOORD2;
		          #endif
				  float2 pack0 : TEXCOORD5; // _MainTex
				  half3 worldNormal : TEXCOORD6;
				  fixed3 vlight : TEXCOORD7; // ambient/SH/vertexlights
				  SHADOW_COORDS(3)
				  #if SHADER_TARGET >= 30
				  float4 lmap : TEXCOORD8;
				  #endif
				  #if _CORR_ON
	                 float4 world : TEXCOORD9;
	                 half2 uvC : TEXCOORD10;
				  #endif
				};
				#endif
				// with lightmaps:
				#ifndef LIGHTMAP_OFF
				struct v2f_surf {
				  float4 pos : SV_POSITION;
				  half2 uvn : TEXCOORD0;
                  half2 uv : TEXCOORD1;
				  float2 pack0 : TEXCOORD2; // _MainTex
				  half3 worldNormal : TEXCOORD4;
				  float4 lmap : TEXCOORD5;
				  SHADOW_COORDS(3)
				  #if _CORR_ON
	                 float4 world : TEXCOORD6;
	                 half2 uvC : TEXCOORD7;
				  #endif
				};
				#endif
               
               v2f_surf vert_surf (appdata_full v) {
				  v2f_surf o;
				  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
				  o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				  o.pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				  float3 worldPos = mul(_Object2World, v.vertex).xyz;
				  fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
				  #if !defined(LIGHTMAP_OFF) && defined(DIRLIGHTMAP_COMBINED)
				  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
				  fixed3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;
				  #endif
				  #if !defined(LIGHTMAP_OFF) && defined(DIRLIGHTMAP_COMBINED)
				  o.tSpace0 = fixed3(worldTangent.x, worldBinormal.x, worldNormal.x);
				  o.tSpace1 = fixed3(worldTangent.y, worldBinormal.y, worldNormal.y);
				  o.tSpace2 = fixed3(worldTangent.z, worldBinormal.z, worldNormal.z);
				  #endif
				  o.worldNormal = worldNormal;
				  #ifndef DYNAMICLIGHTMAP_OFF
				  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
				  #endif
				  #ifndef LIGHTMAP_OFF
				  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				  #endif

				  // SH/ambient and vertex lights
				  #ifdef LIGHTMAP_OFF
				  #if UNITY_SHOULD_SAMPLE_SH
				  float3 shlight = ShadeSH9 (float4(worldNormal,1.0));
				  o.vlight = shlight;
				  #else
				  o.vlight = 0.0;
				  #endif
				  #ifdef VERTEXLIGHT_ON
				  o.vlight += Shade4PointLights (
				    unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
				    unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
				    unity_4LightAtten0, worldPos, worldNormal );
				  #endif // VERTEXLIGHT_ON
				  #endif // LIGHTMAP_OFF

				  TRANSFER_SHADOW(o); // pass shadow coordinates to pixel shader
				  UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
				  
				   o.pos = mul ( UNITY_MATRIX_MVP, v.vertex );
                   float3 n = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
                   n = n * float3(0.5,0.5,0.5) + float3(0.5,0.5,0.5);
                   o.uvn = n.xy;
                   o.uv = TRANSFORM_TEX ( v.texcoord, _MainTex );
                    
                   #if _CORR_ON
	                 o.world =  mul( _Object2World, v.vertex );
	                 o.uvC = TRANSFORM_TEX ( v.texcoord, _CorruptionTexture );
				   #endif
					
				  return o;
				}
				
                inline half4 LightingToonRamp (SurfaceOutput s, half3 lightDir, half atten)
				{
					#ifndef USING_DIRECTIONAL_LIGHT
					lightDir = normalize(lightDir);
					#endif
					
					half d = dot (s.Normal, lightDir)*0.5 + 0.5;
					half3 ramp = tex2D (_Ramp, float2(d,d)).rgb;
					
					half4 c;
					c.rgb = s.Albedo * _LightColor0.rgb * ramp * (atten * 2);
					c.a = 0;
					return c;
				}

				struct Input {
					float2 uv_MainTex : TEXCOORD0;
				};

				inline bool isCorrupted(v2f_surf i){
					#if _CORR_ON
						float yPosition = distance(i.world,_OriginCorruption);
	                   
	                   //If between corrupted and nonCorrupted
	                   if(yPosition<(_YCutOut+(_OutlineSize/2.0)) && yPosition>(_YCutOut-(_OutlineSize/2.0))){
	            			float magnitude = 1.0 - (((_YCutOut+(_OutlineSize/2.0)) - yPosition)/_OutlineSize);
	            		
							fixed4 dissolveColor = tex2D( _DissolveNoise, (i.world.xz*_TileSize));
							fixed highest = 0;
							if(dissolveColor.r>highest){highest = dissolveColor.r;}
							if(dissolveColor.g>highest){highest = dissolveColor.g;}
							if(dissolveColor.b>highest){highest = dissolveColor.b;}
							
							if(!(highest>magnitude || highest == 0)){
								return true;
							}
		            	}else if(yPosition>_YCutOut){
		            		return true;
		            	}
		            #endif
		            return false;
		            	
				}

                inline fixed4 applyCorruption(v2f_surf i,float4 colorO){
               	 	#if _CORR_ON
                       float4 mainTexture = colorO;
					   float4 corruptionTexture = tex2D(_CorruptionTexture, i.uvC);
					   corruptionTexture = corruptionTexture * _CorruptionColor;
					   float alphaCorruption = corruptionTexture.a;
	                   float4 mixedTexture;
	                   mainTexture.r = mainTexture.r * (1.0 - alphaCorruption);
	                   mainTexture.g = mainTexture.g * (1.0 - alphaCorruption);
	                   mainTexture.b = mainTexture.b * (1.0 - alphaCorruption);
	                   
	                   corruptionTexture.r = corruptionTexture.r * (alphaCorruption);
	                   corruptionTexture.g = corruptionTexture.g * (alphaCorruption);
	                   corruptionTexture.b = corruptionTexture.b * (alphaCorruption);
	                   mixedTexture = mainTexture+corruptionTexture;
	                   mixedTexture.a = 1.0;
	                   float yPosition = distance(i.world,_OriginCorruption);
	                   
	                   //If between corrupted and nonCorrupted
	                   if(isCorrupted(i)){
	                   	return mixedTexture;
	                   }
		            #endif
					
				    fixed4 detail = colorO;
					return  detail*_Brightness;
                }
                
				void surf (v2f_surf IN, inout SurfaceOutput o) {
					#if _COLOR_ON
					half4 c = applyCorruption(IN,tex2D(_MainTex, IN.uv) * _Color);
					#else
					half4 c = applyCorruption(IN,tex2D(_MainTex, IN.uv));
					#endif
					o.Albedo = c.rgb;
					o.Alpha = c.a;
				}

                
                
                fixed4 frag_surf (v2f_surf IN) : SV_Target {
				  // prepare and unpack data
				  Input surfIN;
				  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
				  surfIN.uv_MainTex = IN.pack0.xy;
				  float3 lightDir = _WorldSpaceLightPos0.xyz;
				  #ifdef UNITY_COMPILER_HLSL
				  SurfaceOutput o = (SurfaceOutput)0;
				  #else
				  SurfaceOutput o;
				  #endif
				  o.Albedo = 0.0;
				  o.Emission = 0.0;
				  o.Specular = 0.0;
				  o.Alpha = 0.0;
				  o.Gloss = 0.0;
				  fixed3 normalWorldVertex = fixed3(0,0,1);
				  o.Normal = IN.worldNormal;
				  normalWorldVertex = IN.worldNormal;

				  // call surface function
				  surf (IN, o);
				  //if(isCorrupted(IN)){
				  //	return fixed4(o.Albedo.r,o.Albedo.g,o.Albedo.b,o.Alpha);
				  //}

				  // compute lighting & shadowing factor
				  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
				  fixed4 c = 0;
				  #ifdef LIGHTMAP_OFF
				  c.rgb += o.Albedo * IN.vlight;
				  #endif // LIGHTMAP_OFF

				  // lightmaps
				  #ifndef LIGHTMAP_OFF
				    #ifdef DIRLIGHTMAP_OFF
				      // single lightmap
				      fixed4 lmtex = UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.lmap.xy);
				      fixed3 lm = DecodeLightmap (lmtex);
				    #elif DIRLIGHTMAP_COMBINED
				      // directional lightmaps
				      fixed4 lmtex = UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.lmap.xy);
				      fixed4 lmIndTex = UNITY_SAMPLE_TEX2D_SAMPLER(unity_LightmapInd, unity_Lightmap, IN.lmap.xy);
				      half3 lm = DecodeDirectionalLightmap (DecodeLightmap(lmtex), lmIndTex, o.Normal);
				    #elif DIRLIGHTMAP_SEPARATE
				      // directional with specular - no support
				      half4 lmtex = 0;
				      half3 lm = 0;
				    #endif // DIRLIGHTMAP_OFF

				  #endif // LIGHTMAP_OFF


				  // realtime lighting: call lighting function
				  #ifdef LIGHTMAP_OFF
				  c += LightingToonRamp (o, lightDir, atten);
				  #else
				    c.a = o.Alpha;
				  #endif

				  #ifndef LIGHTMAP_OFF
				    // combine lightmaps with realtime shadows
				    #ifdef SHADOWS_SCREEN
				      #if defined(UNITY_NO_RGBM)
				      c.rgb += o.Albedo * min(lm, atten*2);
				      #else
				      c.rgb += o.Albedo * max(min(lm,(atten*2)*lmtex.rgb), lm*atten);
				      #endif
				    #else // SHADOWS_SCREEN
				      c.rgb += o.Albedo * lm;
				    #endif // SHADOWS_SCREEN
				  #endif // LIGHTMAP_OFF

				  #ifndef DYNAMICLIGHTMAP_OFF
				  fixed4 dynlmtex = UNITY_SAMPLE_TEX2D(unity_DynamicLightmap, IN.lmap.zw);
				  c.rgb += o.Albedo * DecodeRealtimeLightmap (dynlmtex);
				  #endif

				  UNITY_APPLY_FOG(IN.fogCoord, c); // apply fog
				  UNITY_OPAQUE_ALPHA(c.a);
				  return c;
				}
            ENDCG
        }
        
         	
        Pass
        {
            Cull Front
            ZWrite On
            CGPROGRAM
			#include "UnityCG.cginc"
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma glsl_no_auto_normalization
            #pragma vertex vert
 			#pragma fragment frag
			
            struct appdata_t 
            {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f 
			{
				float4 pos : SV_POSITION;
			};

            fixed _Outline;
            fixed _DissolveThreshold;

            
            v2f vert (appdata_t v) 
            {
                v2f o;
			    o.pos = v.vertex;
			    o.pos.xyz += v.normal.xyz *_Outline*0.01;
			    o.pos = mul(UNITY_MATRIX_MVP, o.pos);
			    return o;
            }
            
            fixed4 _OutlineColor;
            
            fixed4 frag(v2f i) :COLOR 
			{
				fixed4 outline = _OutlineColor;
		    	return outline;
			}
            
            ENDCG
        }
        
        //UsePass "Toon/LitG/FORWARD"
        //UsePass "Toon/LitG/Meta"
        
    }
    Fallback "Diffuse"
}
