//=========================================================================================================================
// Optional
//=========================================================================================================================
HEADER
{
	DevShader = true;
	Description = "Colorful Test Shader";
}

//=========================================================================================================================
// Optional
//=========================================================================================================================
FEATURES
{
}

//=========================================================================================================================
MODES
{
    Default();
}

//=========================================================================================================================
COMMON
{
	#include "common/shared.hlsl"
}

//=========================================================================================================================
CS
{
	// You could set this to the current game time... or whatever value you'd like. 
	float g_flGameTime< Attribute ( "GameTime" ); >;
	// Texture that will be multiplied against.
	Texture2D<float4> g_tMultTex< Attribute( "MultiplicandTexture" ); >;
    // Output texture
    RWTexture2D<float4> g_tOutput< Attribute( "OutputTexture" ); >;

    [numthreads(8, 8, 1)] 
    void MainCs( uint uGroupIndex : SV_GroupIndex, uint3 vThreadId : SV_DispatchThreadID )
    {
		int2 size;
		g_tOutput.GetDimensions(size.x, size.y);

		float xCoord = vThreadId.x / size.x;
		float yCoord = vThreadId.y / size.y;

		float red = (sin(g_flGameTime * 0.2) + 1) / 2;
		float green = (sin(g_flGameTime * 0.5) + 1) / 2;
		float blue = (sin(g_flGameTime * 0.9) + 1) / 2;
		
		float4 modColor = float4(red, green, blue, 1);
		float4 multColor = g_tMultTex[vThreadId.xy];
		g_tOutput[vThreadId.xy] = modColor * multColor;
    }
}