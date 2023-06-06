using Sandbox;
using Sandbox.ModelEditor;
using System.Text.Json.Serialization;

[GameData("Interactable Volume", AllowMultiple = true)]
[Box("dimensions", Bone = "bonename", Angles = "offset_angles", Origin = "offset_origin")]
[Axis(Bone = "bonename", Origin = "offset_origin", Angles = "offset_angles")]
public class ModelInteract
{
    [JsonPropertyName("interact_name")]
    public string InteractionName { get; set; }

    [JsonPropertyName("interact_use_text")]
    public string UseText { get; set; }

    [FGDType("model_bone")]
    public string BoneName { get; set; }

    [ScaleBoneRelative, DefaultValue("10 10 10")]
    public Vector3 Dimensions { get; set; }

    [JsonPropertyName("offset_origin"), ScaleBoneRelative]
    public Vector3 Origin { get; set; }

    [JsonPropertyName("offset_angles")]
    public Angles Angles { get; set; }

    [JsonPropertyName("attachment_point"), FGDType("model_attachment")]
    public string AttachmentPoint { get; set; }

    public BBox Bounds => BBox.FromPositionAndSize(Origin, Dimensions); 
    
}

