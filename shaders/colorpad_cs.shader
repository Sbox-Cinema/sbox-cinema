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
    float g_flFitAspectRatio < Attribute("FitAspectRatio"); > ;
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

        uint2 inputSize;
        g_tInput.GetDimensions(inputSize.x, inputSize.y);

        uint2 outputSize;
        g_tOutput.GetDimensions(outputSize.x, outputSize.y);

        // Find where we are on the output texture.
        float2 normalizedCoordinates;
        normalizedCoordinates.x = float(vThreadId.x) / outputSize.x;
        normalizedCoordinates.y = float(vThreadId.y) / outputSize.y;

        float2 paddedCoordinates;


        float aspectRatio = float(inputSize.x) / inputSize.y;
        float halfPadRatio = g_flPadRatio / 2;
        if (aspectRatio > 1) // Horizontal aspect ratio
        {
            paddedCoordinates.x = normalizedCoordinates.x / g_flPadRatio;
            // Instead of making x wider, make y shorter so there's a margin at the bottom.
            paddedCoordinates.y = normalizedCoordinates.y / (1 / aspectRatio) / g_flPadRatio;
            float aspectFitness = aspectRatio / g_flFitAspectRatio;
            paddedCoordinates.x /= g_flPadRatio * aspectFitness;
            paddedCoordinates.y /= g_flPadRatio * aspectFitness;
            paddedCoordinates.x -= (1 - g_flPadRatio + (1 - aspectFitness));
            paddedCoordinates.y -= (1 - (1 / aspectRatio * aspectFitness) + (1 - g_flPadRatio));
        }
        else // Vertical aspect ratio
        {
            paddedCoordinates.x = normalizedCoordinates.x / aspectRatio / g_flPadRatio;
            paddedCoordinates.y = normalizedCoordinates.y / g_flPadRatio;
            // In the next three lines of code, I multiply stuff by magic constants I pulled out of my butt.
            // Looks "good enough" for the YouTube short format.
            paddedCoordinates *= 2;
            paddedCoordinates.x -= 0.5 / aspectRatio * 1.6;
            paddedCoordinates.y -= 0.5 * aspectRatio * 1.9;
        }

        if (paddedCoordinates.x < 0 || paddedCoordinates.x > 1)
            return;
        if (paddedCoordinates.y < 0 || paddedCoordinates.y > 1)
            return;

        // Figure out where we would be pulling from on the input texture.
        uint2 inputCoordinates;
        inputCoordinates.x = lerp(0, 1, paddedCoordinates.x) * inputSize.x;
        inputCoordinates.y = lerp(0, 1, paddedCoordinates.y) * inputSize.y;

        // Read the pixel from the input texture
        float4 inputPixel = g_tInput.Load(int3(inputCoordinates, 0));

        // Write the pixel to the output texture
        g_tOutput[vThreadId.xy] = inputPixel;
    }
}