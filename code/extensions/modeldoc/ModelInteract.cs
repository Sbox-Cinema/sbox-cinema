using Sandbox;
using Sandbox.ModelEditor;
using System.Text.Json.Serialization;

[GameData("Interactable Area", AllowMultiple = true)]
[Box(dimensionsKey: nameof(size), Angles = nameof(rotation), Origin = nameof(origin))]
public class ModelInteract
{
    public Vector3 size { get; set; }
    public Vector3 origin { get; set; }
    public Vector3 rotation { get; set; }

    [JsonPropertyName("attachment_point"), FGDType("model_attachment")]
    public string AttachmentPoint { get; set; }
}

