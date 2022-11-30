Shader "Custom/Foggy"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FogColor ("Fog (RGBA)", Color) = (255,255,255,255)
        _ResX ("Resolution X", Range (0.00,1.00)) = 0.5
        _ResY ("Resolution Y", Range (0.00,1.00)) = 0.5 
        _Speed ("Speed", Range (0.00,10.00)) = 1.0 
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // @neuro: the following is adapted from GLSL source at https://thebookofshaders.com/13/

            sampler2D _MainTex;
            fixed4 _FogColor;
            float _ResX;
            float _ResY;
            float _Speed;

            float random (in float2 _st) {
                return frac(
                    sin(dot(_st.xy, float2(12.9898,78.233))) * 43758.5453123
                );
            }

            // Based on Morgan McGuire @morgan3d
            // https://www.shadertoy.com/view/4dS3Wd

            float noise (in float2 _st) {
                float2 i = floor(_st);
                float2 f = frac(_st);
                // Four corners in 2D of a tile
                float a = random(i);
                float b = random(i + float2(1.0, 0.0));
                float c = random(i + float2(0.0, 1.0));
                float d = random(i + float2(1.0, 1.0));
                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(a, b, u.x)
                    + (c - a) * u.y * (1.0 - u.x)
                    + (d - b) * u.x * u.y;
            }

            #define NUM_OCTAVES 5

            float fbm ( in float2 _st) {
                float v = 0.0;
                float a = 0.5;
                float2 shift = float2(100.0, 100.0);
                // Rotate to reduce axial bias
                float2x2 rot = float2x2(
                     cos(0.5), sin(0.5),
                    -sin(0.5), cos(0.50));
                for (int i = 0; i < NUM_OCTAVES; ++i) {
                    v += a * noise(_st);
                    _st = mul(_st, rot) * 2.0 + shift;
                    a *= 0.5;
                }
                return v;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 textCol = tex2D(_MainTex, i.uv);
                float2 st = i.uv/float2(_ResX, _ResY)*3.;
                float2 q = float2(0.0, 0.0);
                q.x = fbm( st );
                q.y = fbm( st + float2(1.0, 1.0));
                float2 r = float2(0.0, 0.0);
                r.x = fbm( st + 1.0*q + float2(1.7,9.2)+ 0.15*_Time*_Speed );
                r.y = fbm( st + 1.0*q + float2(8.3,2.8)+ 0.126*_Time*_Speed );
                float f = fbm(st+r);
                float3 color = lerp(
                    float3(0.101961,0.619608,0.666667),
                    float3(0.666667,0.666667,0.498039),
                    clamp((f*f)*4.0,0.0,1.0)
                );
                color = lerp(
                    color,
                    float3(0,0,0.164706),
                    clamp(length(q),0.0,1.0)
                );
                color = lerp(
                    color,
                    float3(0.666667,1,1),
                    clamp(length(r.x),0.0,1.0)
                );
                return fixed4(
                    max(
                        textCol.rgb,
                        textCol.rgb * (1 - _FogColor.a) + (f * f * f + .6 * f * f +.5 * f) * _FogColor.rgb * _FogColor.a
                    ),
                    textCol.a
                );
            }


            ENDCG
        }
    }
}
