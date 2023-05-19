//=========================================================================================================================
// Optional
//=========================================================================================================================
HEADER
{
    DevShader = true;
    Description = "Downscales a texture to fit an output texture of an arbitrary size that is smaller in both dimensions.";
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
    // Output texture
    RWTexture2D<float4> g_tOutput < Attribute("OutputTexture"); > ;

    [numthreads(8, 8, 1)]
    void MainCs(uint2 vGroupID : SV_GroupID, uint2 vGroupThreadID : SV_GroupThreadID, uint uGroupIndex : SV_GroupIndex, uint3 vThreadId : SV_DispatchThreadID )
    {
        uint2 outputSize;
        g_tOutput.GetDimensions(outputSize.x, outputSize.y);

        // Calculate the normalized coordinates for the current thread in the output texture
        float2 normalizedCoordinates;
        normalizedCoordinates.x = float(vThreadId.x) / outputSize.x;
        normalizedCoordinates.y = float(vThreadId.y) / outputSize.y;

        uint2 inputSize;
        g_tInput.GetDimensions(inputSize.x, inputSize.y);

        // Calculate the input coordinates
        uint2 inputCoordinates;
        inputCoordinates.x = normalizedCoordinates.x * inputSize.x;
        inputCoordinates.y = normalizedCoordinates.y * inputSize.y;

        // Read the pixel from the input texture
        float4 inputPixel = g_tInput.Load(int3(inputCoordinates, 0));

        // Write the pixel to the output texture
        g_tOutput[vThreadId.xy] = inputPixel;
    }
}