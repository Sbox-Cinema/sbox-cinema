//=========================================================================================================================
// Optional
//=========================================================================================================================
HEADER
{
    DevShader = true;
    Description = "Pads a texture with a color.";
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
    // Padding ratio
    float g_flPadRatio < Attribute("PadRatio"); > ;
    // Padding color
    float4 g_flColor < Attribute("PadColor"); > ;
    // Input texture
    Texture2D<float4> g_tInput < Attribute("InputTexture"); > ;
    // Output texture
    RWTexture2D<float4> g_tOutput < Attribute("OutputTexture"); > ;

    [numthreads(8, 8, 1)]
    void MainCs(uint2 vGroupID : SV_GroupID, uint2 vGroupThreadID : SV_GroupThreadID, uint uGroupIndex : SV_GroupIndex, uint3 vThreadId : SV_DispatchThreadID )
    {
        // Set the padding color. It will be overwritten if we needed to put something here.
        g_tOutput[vThreadId.xy] = g_flColor;

        uint2 outputSize;
        g_tOutput.GetDimensions(outputSize.x, outputSize.y);

        // Find where we are on the output texture.
        float2 normalizedCoordinates;
        normalizedCoordinates.x = float(vThreadId.x) / outputSize.x;
        normalizedCoordinates.y = float(vThreadId.y) / outputSize.y;

        uint2 inputSize;
        g_tInput.GetDimensions(inputSize.x, inputSize.y);
        float aspectRatio = float(inputSize.x) / inputSize.y;
        if (aspectRatio > 1)
        {
            float halfPadRatio = g_flPadRatio / 2;
            if (normalizedCoordinates.x < halfPadRatio / aspectRatio || normalizedCoordinates.x > 1 - halfPadRatio / aspectRatio)
                return;
            if (normalizedCoordinates.y < halfPadRatio || normalizedCoordinates.y > 1 - halfPadRatio )
                return;
        }
        else
        {
            float halfPadRatio = g_flPadRatio / 2;
            if (normalizedCoordinates.x < halfPadRatio || normalizedCoordinates.x > 1 - halfPadRatio)
                return;
            if (normalizedCoordinates.y < halfPadRatio * aspectRatio || normalizedCoordinates.y > 1 - halfPadRatio * aspectRatio)
                return;
        }

    
        float2 subCoords;
        subCoords.x = lerp(0 - g_flPadRatio / aspectRatio, 1 + g_flPadRatio / aspectRatio, normalizedCoordinates.x);
        subCoords.y = lerp(0 - g_flPadRatio * aspectRatio, 1 + g_flPadRatio * aspectRatio, normalizedCoordinates.y);

        // Figure out where we would be pulling from on the input texture.
        uint2 inputCoordinates;
        inputCoordinates.x = subCoords.x * inputSize.x;
        inputCoordinates.y = subCoords.y * inputSize.y;

        // Read the pixel from the input texture
        float4 inputPixel = g_tInput.Load(int3(inputCoordinates, 0));

        // Write the pixel to the output texture
        g_tOutput[vThreadId.xy] = inputPixel;
    }
}