using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema;

public static class TextureExtensions
{
    private static ComputeShader DownscaleShader { get; set; } = new ComputeShader("downscale_cs");
    private static ComputeShader MultiplyShader { get; set; } = new ComputeShader("multiplytexture_cs");
    private static ComputeShader GaussianBlurShader { get; set; } = new ComputeShader("gaussianblur_cs");

    public static void DispatchDownscale(this Texture InputTexture, Texture toTex)
    {
        DownscaleShader.Attributes.Set("InputTexture", InputTexture);
        DownscaleShader.Attributes.Set("OutputTexture", toTex);
        DownscaleShader.Dispatch(toTex.Width, toTex.Height, 1);
    }

    public static void DispatchMultiply(this Texture InputTexture, Texture multTex, Texture toTex)
    {
        MultiplyShader.Attributes.Set("InputTexture", InputTexture);
        MultiplyShader.Attributes.Set("MultiplicandTexture", multTex);
        MultiplyShader.Attributes.Set("OutputTexture", toTex);
        MultiplyShader.Dispatch(toTex.Width, toTex.Height, 1);
    }

    public static void DispatchGaussianBlur(this Texture InputTexture, Texture toTex)
    {
        GaussianBlurShader.Attributes.Set("InputTexture", InputTexture);
        GaussianBlurShader.Attributes.Set("OutputTexture", toTex);
        GaussianBlurShader.Dispatch(toTex.Width, toTex.Height, 1);
    }
}
