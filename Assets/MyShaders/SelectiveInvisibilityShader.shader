Shader "Custom/SelectiveInvisibilityShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SphereCenter ("Sphere Center", Vector) = (0, 0, 0, 0)
        _SphereRadius ("Sphere Radius", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }

        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha

        #pragma target 3.0

        sampler2D _MainTex;
        float4 _SphereCenter;
        float _SphereRadius;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;

            // Calculate the distance from the current fragment to the sphere center
            float3 worldPos = WorldPosition(IN);
            float distToCenter = distance(worldPos, _SphereCenter);

            // If the fragment is inside the sphere, make it transparent
            if (distToCenter < _SphereRadius)
            {
                o.Alpha = 0;
            }
            else
            {
                o.Alpha = c.a;
            }
        }
        ENDCG
    }
    FallBack "Diffuse"
}
