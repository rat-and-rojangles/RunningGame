Shader "Projector/image" {
   Properties {
      _Color ("Blend Color", Color) = (1,1,1,1)
      _ShadowTex ("Projected Image", 2D) = "white" {}
   }
   SubShader {
      Pass {      
         //Blend One One 
         Blend SrcAlpha OneMinusSrcAlpha
            // add color of _ShadowTex to the color in the framebuffer 
         ZWrite Off // don't change depths
         Offset -1, -1 // avoid depth fighting

         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
 
         // User-specified properties
         uniform sampler2D _ShadowTex; 
         uniform fixed4 _Color;

         // Projector-specific uniforms
         uniform fixed4x4 _Projector; // transformation matrix 
            // from object space to projector space 
 
          struct vertexInput {
            fixed4 vertex : POSITION;
            float3 normal : NORMAL;
         };
         struct vertexOutput {
            fixed4 pos : SV_POSITION;
            fixed4 posProj : TEXCOORD0;
               // position in projector space
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 
            output.posProj = mul(_Projector, input.vertex);
            output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
            return output;
         }
 
 
         fixed4 frag(vertexOutput input) : COLOR
         {
            if (input.posProj.w > 0.0) // in front of projector?
            {
               fixed4 f = tex2D(_ShadowTex , input.posProj.xy / input.posProj.w); 
               f.rgb *= _Color.rgb;
               return f;
            }
            else // behind projector
            {
               return fixed4(0.0, 0.0, 0.0, 0.0);
            }
         }
 
         ENDCG
      }
   }  
   Fallback "Projector/Light"
}
