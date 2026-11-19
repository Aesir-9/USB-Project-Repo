Shader "USB Project/Chapter 4/Stencil Read"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        _StencilRef ("Stencil Ref", Int) = 1
        _ClipRadius("Clip Radius", Range(0, 1)) = 1
        [KeywordEnum(On, Off)] _ClipEnabled("Clip Enabled", Float) = 1        
        [HideInInspector] _Position ("Position", Vector) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags
        {
            "RenderType" =
            "Opaque" "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry+1"            
        }

        Pass
        {
            Stencil
            {
                Ref [_StencilRef]
                Comp NotEqual
            }

            ZTest LEqual
            ZWrite On
            
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature_fragment _CLIPENABLED_ON _CLIPENABLED_OFF

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Assets/USB Project/Chapter 4/Stylized Flat Art//Materials/HLSL/sdCapsule.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _BaseMap_ST;
                float4 _Position;
                float _ClipRadius;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionWS = mul(unity_ObjectToWorld, float4(IN.positionOS.xyz, 1.0)).rgb;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;

                float3 pa = _Position.xyz;
                float3 pb = _WorldSpaceCameraPos;                
               
                float clipArea = sdCapsule(IN.positionWS, pa, pb, _ClipRadius);

                #if _CLIPENABLED_ON
                clip(clipArea);               
                #endif
                
                return color;
            }
            ENDHLSL
        }
    }
}
