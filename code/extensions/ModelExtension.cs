using Sandbox;
using Sandbox.ModelEditor;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Cinema;

[GameData("interact_box", AllowMultiple = true)]
[Box("dimensions", Origin ="offset_origin", Attachment = "attachment_point")]
[Axis(Origin = "offset_origin", Attachment = "attachment_point")]
public class ModelInteractionBox
{
    [DisplayName("Interaction")]
    public string Name { get; set; }

    [DefaultValue("10 10 10"), ScaleBoneRelative]
    public Vector3 Dimensions { get; set; }

    [JsonPropertyName("offset_origin"), ScaleBoneRelative]
    public Vector3 OriginOffset { get; set; }

    [JsonPropertyName("attachment_point"), FGDType("model_attachment")]
    public string Attachment { get; set; }
}
