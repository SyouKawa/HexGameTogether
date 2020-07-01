Shader "Custom/Outline"
{
    Properties
    {
    _Outline ("Outline", Range(0, 0.2)) = 0.043
    _OutlineColor ("OutlineColor", Color) = (1, 1, 1, 1)
    _MainTex ("MainTex", 2D) = "white" {}
    _ChangeSpeed("Speed",Range(1,5)) = 0.5
    }

    SubShader
    {
        Tags{
            "Queue" = "Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha

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
                fixed2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float _Outline;
            float3 _OutlineColor;
            sampler2D _MainTex;
            float4 _MainTex_PixelSize;
            float _ChangeSpeed;
    
            fixed4 frag (v2f i) : SV_Target
            { 
                fixed4 col = tex2D(_MainTex,i.uv);

                float2 uv_up = i.uv + _MainTex_PixelSize.xy*float2(0,1) * _Outline;
                float2 uv_down = i.uv + _MainTex_PixelSize.xy*float2(0,-1) * _Outline;
                float2 uv_left = i.uv + _MainTex_PixelSize.xy*float2(-1,0) * _Outline;
                float2 uv_right = i.uv + _MainTex_PixelSize.xy*float2(1,0) * _Outline;

                //采样
                float w = tex2D(_MainTex,uv_up).a * tex2D(_MainTex,uv_down).a *tex2D(_MainTex,uv_left).a*tex2D(_MainTex,uv_right).a;

                col.rgb = lerp(_OutlineColor,col.rgb,w);

                float tmp = _ChangeSpeed*_Time.y;

                return col* abs(cos(tmp));
            }
            ENDCG
        }
    }
}