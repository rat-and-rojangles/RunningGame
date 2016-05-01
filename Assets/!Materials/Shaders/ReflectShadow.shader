Shader "Reflective/BaseCubeBumpLightMaps" {
     Properties {
         _Color ("Main Color", Color) = (1, 1, 1, 1)
         _ReflectColor ("Reflection Color", Color) = (1, 1, 1, 1)
         _MainTex ("Base (RGB) RefStrength (A)", 2D) = "white" {} 
         _BumpMap ("Normalmap", 2D) = "bump" {}
         _LightMap ("Lightmap (RGB)", 2D) = "black" {}
         _Cube ("Reflection Cubemap", Cube) = "_Skybox" { TexGen CubeReflect }
     }
 
     SubShader {
 //        Tags {  "Queue" = "Transparent" "RenderType"="Transparent"}
         Tags { "Queue" = "Transparent" "RenderType"="Opaque"}
         LOD 300
 //        Cull Off
         Lighting On
 
         CGPROGRAM
         #pragma surface surf Lambert alpha addshadow 
 
         sampler2D _MainTex;
         sampler2D _BumpMap;
         sampler2D _LightMap;
         samplerCUBE _Cube;
         float4 _Color;
         float4 _ReflectColor;
 
         struct Input {
             float2 uv_MainTex;
             float2 uv_LightMap;
             float3 worldRefl;
             INTERNAL_DATA
         };
 
         void surf (Input IN, inout SurfaceOutput o) {
             half4 tex = tex2D(_MainTex, IN.uv_MainTex);
             half4 l = tex2D(_LightMap, IN.uv_LightMap);
 
             half4 c = tex * _Color * l;
             o.Albedo = c.rgb;
 
             half4 reflcol = texCUBE (_Cube, IN.worldRefl) * _ReflectColor;
 
             o.Alpha = _Color.a + (reflcol.a * _ReflectColor.a); // * c.a);
             o.Emission = (reflcol.rgb * _ReflectColor.a);   // * c.rgb;
 
             o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
         }
         ENDCG
 
     }
     Fallback "VertexLit"
 }