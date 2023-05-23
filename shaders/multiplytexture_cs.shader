//=========================================================================================================================
// Optional
//=========================================================================================================================
HEADER
{
    DevShader = true;
    Description = "Multiplies an input texture by another and writes it to an output texture.";
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
    // Input texture
    Texture2D<float4> g_tInput < Attribute("InputTexture"); > ;
    Texture2D<float4> g_tMult < Attribute("MultiplicandTexture"); > ;
    // Output texture
    RWTexture2D<float4> g_tOutput < Attribute("OutputTexture"); > ;

    [numthreads(8, 8, 1)]
    void MainCs(uint3 vThreadId : SV_DispatchThreadID)
    {
        g_tOutput[vThreadId.xy] = g_tInput[vThreadId.xy] * g_tMult[vThreadId.xy];
    }
}