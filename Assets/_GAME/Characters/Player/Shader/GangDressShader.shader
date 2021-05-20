// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "GangDressShader"
{
	Properties
	{
		_Color_C("Color_C", Color) = (0,0,0,0)
		_Color_B("Color_B", Color) = (0,0,0,0)
		_Color_A("Color_A", Color) = (0,0,0,0)
		_Color_Mask("Color_Mask", 2D) = "white" {}
		_ColorMap("ColorMap", 2D) = "white" {}
		_Nor("Nor", 2D) = "bump" {}
		_ORM("ORM", 2D) = "white" {}
		_EmissiveColor("EmissiveColor", Color) = (0,0,0,0)
		_EmissiveFactor("EmissiveFactor", Float) = 1
		_ColorContrast("ColorContrast", Float) = 0
		_RoughnessFactor("RoughnessFactor", Float) = 0
		_OcculsionFactor("OcculsionFactor", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Nor;
		uniform float4 _Nor_ST;
		uniform sampler2D _ColorMap;
		uniform float4 _ColorMap_ST;
		uniform float4 _Color_C;
		uniform sampler2D _Color_Mask;
		uniform float4 _Color_Mask_ST;
		uniform float4 _Color_B;
		uniform float4 _Color_A;
		uniform float _ColorContrast;
		uniform float4 _EmissiveColor;
		uniform float _EmissiveFactor;
		uniform sampler2D _ORM;
		uniform float4 _ORM_ST;
		uniform float _RoughnessFactor;
		uniform float _OcculsionFactor;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Nor = i.uv_texcoord * _Nor_ST.xy + _Nor_ST.zw;
			o.Normal = UnpackNormal( tex2D( _Nor, uv_Nor ) );
			float2 uv_ColorMap = i.uv_texcoord * _ColorMap_ST.xy + _ColorMap_ST.zw;
			float4 tex2DNode16 = tex2D( _ColorMap, uv_ColorMap );
			float2 uv_Color_Mask = i.uv_texcoord * _Color_Mask_ST.xy + _Color_Mask_ST.zw;
			float4 tex2DNode11 = tex2D( _Color_Mask, uv_Color_Mask );
			float4 lerpResult18 = lerp( tex2DNode16 , ( ( ( _Color_C * tex2DNode11.b ) + ( ( _Color_B * tex2DNode11.g ) + ( _Color_A * tex2DNode11.r ) ) ) * tex2DNode16 ) , ( ( tex2DNode11.r + tex2DNode11.g ) + tex2DNode11.b ));
			o.Albedo = ( lerpResult18 * _ColorContrast ).rgb;
			o.Emission = ( ( _EmissiveColor * tex2DNode11.a ) * _EmissiveFactor ).rgb;
			float2 uv_ORM = i.uv_texcoord * _ORM_ST.xy + _ORM_ST.zw;
			float4 tex2DNode22 = tex2D( _ORM, uv_ORM );
			o.Metallic = tex2DNode22.b;
			o.Smoothness = ( _RoughnessFactor * tex2DNode22.g );
			o.Occlusion = ( _OcculsionFactor * tex2DNode22.r );
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18800
260;73;963;811;-267.5054;862.3409;1.966889;True;True
Node;AmplifyShaderEditor.CommentaryNode;30;-1390.046,-953.658;Inherit;False;956.853;814.33;Color;8;9;10;13;15;8;7;14;12;;1,1,1,1;0;0
Node;AmplifyShaderEditor.ColorNode;9;-1330.793,-351.3276;Inherit;False;Property;_Color_A;Color_A;2;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.46,0.200432,0.01623,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;8;-1326.943,-619.5128;Inherit;False;Property;_Color_B;Color_B;1;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.270498,0.02121,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;11;-1405.9,-9.361338;Inherit;True;Property;_Color_Mask;Color_Mask;3;0;Create;True;0;0;0;False;0;False;-1;None;4416b8704bfb24647a61b539fad6bdda;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-911.9467,-600.9525;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;7;-1340.046,-897.3811;Inherit;False;Property;_Color_C;Color_C;0;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.27,0.021457,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-905.0314,-327.7943;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;14;-697.5696,-571.5623;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-895.9341,-903.658;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;19;-799.1296,533.9839;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;15;-585.1946,-856.8221;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;16;-268.3737,-370.2025;Inherit;True;Property;_ColorMap;ColorMap;4;0;Create;True;0;0;0;False;0;False;-1;None;25f3141444ddd23489fb6cc9d15c96ad;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;144.8204,-553.4604;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;20;-534.6164,435.4395;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;33;166.2865,102.4676;Inherit;False;Property;_EmissiveColor;EmissiveColor;7;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;31;407.2496,252.2155;Inherit;False;370;280;c;1;21;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;453.0682,-102.3013;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;43;978.5316,158.2868;Inherit;False;Property;_OcculsionFactor;OcculsionFactor;11;0;Create;True;0;0;0;False;0;False;1;1.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;39;641.3828,-386.4887;Inherit;False;Property;_ColorContrast;ColorContrast;9;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;22;581.5734,656.6453;Inherit;True;Property;_ORM;ORM;6;0;Create;True;0;0;0;False;0;False;-1;None;4f9d42db48fb12d448ce795a6a96f172;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;18;457.7419,-560.3752;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;35;963.6894,-339.7271;Inherit;False;Property;_EmissiveFactor;EmissiveFactor;8;0;Create;True;0;0;0;False;0;False;1;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;41;572.9009,-23.88291;Inherit;False;Property;_RoughnessFactor;RoughnessFactor;10;0;Create;True;0;0;0;False;0;False;0;2.12;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;21;457.2496,302.2155;Inherit;True;Property;_Nor;Nor;5;0;Create;True;0;0;0;False;0;False;-1;None;d7ca184f6dbb0c148a0367567a98c12a;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;853.1014,-607.1351;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;851.5964,-154.8025;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;892.9005,-13.88291;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;1251.468,180.2769;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;37;1583.218,-376.434;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;GangDressShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;12;0;8;0
WireConnection;12;1;11;2
WireConnection;13;0;9;0
WireConnection;13;1;11;1
WireConnection;14;0;12;0
WireConnection;14;1;13;0
WireConnection;10;0;7;0
WireConnection;10;1;11;3
WireConnection;19;0;11;1
WireConnection;19;1;11;2
WireConnection;15;0;10;0
WireConnection;15;1;14;0
WireConnection;17;0;15;0
WireConnection;17;1;16;0
WireConnection;20;0;19;0
WireConnection;20;1;11;3
WireConnection;34;0;33;0
WireConnection;34;1;11;4
WireConnection;18;0;16;0
WireConnection;18;1;17;0
WireConnection;18;2;20;0
WireConnection;38;0;18;0
WireConnection;38;1;39;0
WireConnection;36;0;34;0
WireConnection;36;1;35;0
WireConnection;40;0;41;0
WireConnection;40;1;22;2
WireConnection;42;0;43;0
WireConnection;42;1;22;1
WireConnection;37;0;38;0
WireConnection;37;1;21;0
WireConnection;37;2;36;0
WireConnection;37;3;22;3
WireConnection;37;4;40;0
WireConnection;37;5;42;0
ASEEND*/
//CHKSM=B393DF292C95E2DCC9FC1D88E2FAADA0E90B8964