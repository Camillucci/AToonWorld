Shader "Custom/Waving Sprite" 
{
    Properties 
    {
        [PerRendererData] _MainTex ("Texture", 2D) = "white" {}
        _EdgeLength ("Edge length", Range(2,50)) = 15
        [HideInInspector] _Color ("Tint", Color) = (1,1,1,1)
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        _Dampening ("Dampening", Range(0, 1000)) = 1
        _MinY ("Minimum Vertex Height", Float) = 0
        _MaxY ("Maximum Vertex Height", Float) = 100
        _Frequency ("Wave Frequency", Float) = 10
        _Amplitude ("Wave Amplitude", Float) = 0.1
        _PhaseSeed ("Wave Phase", Float) = 0
    }

    CGINCLUDE
    #include "UnitySprites.cginc"

    float _Dampening;
    half _PhaseSeed;
    half _Frequency;
    half _MinY;
    half _MaxY;
    half _Amplitude;

    fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
    {
        fixed4 c;
        c.rgb = s.Albedo; 
        c.a = s.Alpha;
        return c;
    }

    struct Input 
    {
        float2 uv_MainTex;
        float4 color : COLOR;
    };

    void vert(inout appdata_full v)
    {
        //Waving calculation
        half waveAmplitude = _Amplitude * clamp((v.vertex.y - _MinY) / (_MaxY - _MinY), 0, 1);
        float4 worldPos = mul (unity_ObjectToWorld, v.vertex);
        float4 worldUpDir = normalize(mul (unity_ObjectToWorld, float3(0, 1, 0)));
        float4 worldLeftDir = normalize(mul (unity_ObjectToWorld, float3(1, 0, 0)));
        float xWorldDiff = dot(worldPos, worldLeftDir);
        worldPos += (1 - sin(xWorldDiff/_Dampening + _Time.y * _Frequency + _PhaseSeed)) * waveAmplitude * worldUpDir;
        v.vertex = mul (unity_WorldToObject, worldPos);

        v.vertex = UnityFlipSprite(v.vertex, _Flip);

        #if defined(PIXELSNAP_ON)
        v.vertex = UnityPixelSnap (v.vertex);
        #endif
    }

    void surf (Input IN, inout SurfaceOutput o) 
    {
        fixed4 c = SampleSpriteTexture (IN.uv_MainTex) * IN.color * _Color * _RendererColor;
        o.Albedo = c.rgb * c.a;
        o.Alpha = c.a;
    }
    ENDCG

    SubShader 
    {
        Tags 
        { 
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
            "DisableBatching"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        CGPROGRAM
        #pragma surface surf NoLighting vertex:vert tessellate:tessEdge nofog noambient nolightmap nodynlightmap keepalpha alpha noinstancing
        #pragma multi_compile _ PIXELSNAP_ON
        #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
        #pragma target 4.6
        #include "Tessellation.cginc"

        float _EdgeLength;

        float4 tessEdge (appdata_full v0, appdata_full v1, appdata_full v2)
        {
            return UnityEdgeLengthBasedTess (v0.vertex, v1.vertex, v2.vertex, _EdgeLength);
        }
        ENDCG
    }

    //No tessellation variant
    SubShader 
    {
        Tags 
        { 
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
            "DisableBatching"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        CGPROGRAM
        #pragma surface surf NoLighting vertex:vert nofog noambient nolightmap nodynlightmap keepalpha alpha noinstancing
        #pragma multi_compile _ PIXELSNAP_ON
        #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
        ENDCG
    }

    FallBack "TextMeshPro/Sprite"
}