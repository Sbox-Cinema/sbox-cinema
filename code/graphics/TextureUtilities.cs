using Sandbox;

namespace Cinema;

public class TextureUtilities
{
    /// <summary>
    /// Creates and returns an RGBA8888 texture with UAV binding and dynamic usage with a size 
    /// matching the specified width and height.
    /// </summary>
    public static Texture CreateShaderTexture(int width, int height)
    {
        return Texture.Create(width, height)
            .WithUAVBinding()
            .WithFormat(ImageFormat.RGBA8888)
            .WithDynamicUsage()
            .Finish();
    }
}
