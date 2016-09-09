Shader "CleanKit/Puddle" {
	Properties 
	{
		_color ("Color", Color) = (1,1,1,1)
	}

	SubShader 
	{
		Tags 
		{ 
			"Queue"="Transparent" 
		}

		Pass
		{
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			uniform float _color;

			struct vertexInput {
				float4 vertex: POSITION;
			};

			struct vertexOutput {
				float4 vertex: SV_POSITION;
				float4 worldPosition: TEXCOORD0;
			};

			vertexOutput vert(vertexInput input) {
				vertexOutput output;
				output.vertex = mul(UNITY_MATRIX_MVP, input.vertex);
				output.worldPosition = mul(unity_ObjectToWorld, input.vertex);
				return output;
			}

			float4 frag(vertexOutput output): COLOR {

				return float4(0,1,1,1);
			}

			ENDCG
		}
	}
}
