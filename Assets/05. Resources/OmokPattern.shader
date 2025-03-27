Shader "Custom/OmokPattern"
{
    Properties
    {
        
         _MainTex ("Texture", 2D) = "white" {}
        _Color1 ("Color 1", Color) = (0.0, 0.0, 0.0, 1) // 검정색
        _Color2 ("Color 2", Color) = (1.0, 1.0, 1.0, 1) // 흰색
        _GridSize ("Grid Size", Range(1, 100)) = 10     // 격자 크기
        _ScrollSpeedX ("Scroll Speed X", Range(-2, 2)) = 0.5
        _ScrollSpeedY ("Scroll Speed Y", Range(-2, 2)) = 0.5
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Background" }
        LOD 100
        
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
            
            float4 _Color1;
            float4 _Color2;
            float _GridSize;
            float _ScrollSpeedX;
            float _ScrollSpeedY;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // 애니메이션을 위한 시간 오프셋 계산
                float2 timeOffset = float2(_Time.y * _ScrollSpeedX, _Time.y * _ScrollSpeedY);
                
                // UV 좌표에 시간 오프셋 적용
                float2 uv = i.uv + timeOffset;
                
                // 바둑판 격자 계산
                float2 gridPos = floor(uv * _GridSize);
                
                // 격자 위치의 합이 짝수인지 홀수인지에 따라 색상 결정
                float isEven = fmod(gridPos.x + gridPos.y, 2.0);
                
                // 두 색상 사이를 전환하는 패턴
                fixed4 col = lerp(_Color1, _Color2, isEven);
                
                return col;
            }
            ENDCG
        }
    }
}