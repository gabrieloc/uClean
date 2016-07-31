Shader "CleanKit/Grid"
{
	Properties
	{
		_stroke ("Stroke", Float) = 0.01
		_size ("Cell Size", Float) = 10.0
		_color ("Color", Color) = (1, 1, 1, 1)
	}

	SubShader
	{
		Tags 
		{
			"Queue" = "Transparent"
		}

		Pass
		{
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			uniform float _stroke;
			uniform float _size;
			uniform float4 _color;

			struct vertexInput {
				float4 vertex: POSITION;
			};

			struct vertexOutput {
				float4 pos: SV_POSITION;
				float4 worldPos: TEXCOORD0;
			};

			// Vertex shader
			vertexOutput vert(vertexInput input) {
				vertexOutput output;
				output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
				output.worldPos = mul(_Object2World, input.vertex);
				return output;
			}

			// Fragment shader
			float4 frag(vertexOutput input): COLOR {

				float x = input.worldPos.x;
				float y = input.worldPos.y;
				float z = input.worldPos.z;

				float mx = (x % _size);
				float my = (y % _size);
				float mz = (z % _size);

				mx += mx > 0 ? 0 : _size;
				my += my > 0 ? 0 : _size;
				mz += mz > 0 ? 0 : _size;

				bool strokeX = mx < _stroke;
				bool strokeY = my < _stroke;
				bool strokeZ = mz < _stroke;

				if (strokeX || strokeY || strokeZ) {
					return _color;
				}
				return float4(0, 0, 0, 0);
			}

			ENDCG
		}
	}
}
