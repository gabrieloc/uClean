// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "CleanKit/Surface"
{
	Properties
	{
		_stroke ("Stroke", Float) = 0.05
		_size ("Cell Size", Float) = 1.0
		_color ("Color", Color) = (1, 0, 1, 1)
		_highlight ("Highlight", Vector) = (0, 0, 0, 0)
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
			uniform float4 _highlight;

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
				output.worldPos = mul(unity_ObjectToWorld, input.vertex);
				return output;
			}

			// Fragment shader
			float4 frag(vertexOutput input): COLOR {

				bool colored = false;
				bool highlighted = true;
				float o = _size * 0.5;

				for (int i = 0; i < 3; i++) {
					float d = input.worldPos[i];
					float md = d;
					float h = _highlight[i];

					if (d < 0) {
						md -= o; 
						colored = colored | (md % _size) + _size < _stroke;
					} else {
						md += o;
						colored = colored | md % _size < _stroke;
					}

					bool withinMaxHighlight = d > (floor(h) - o) * _size;
					bool withinMinHighlight = d < (floor(h) + 1 - o) * _size;
					highlighted = highlighted && withinMaxHighlight && withinMinHighlight;
				}

				if (colored | highlighted) {
					return _color;
				}
				return float4(0, 0, 0, 0);
			}

			ENDCG
		}
	}
}
