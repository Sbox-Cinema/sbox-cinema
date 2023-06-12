using Cinema.Jobs;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema;

[GameResource(title: "Uniform Definition", extension: "uniform", 
    description: "Defines an outfit that is worn by a specific job, which will be used to replace the existing clothes worn by a player.", 
    Icon = "checkroom")]
public class JobUniform : GameResource
{
    /// <summary>
    /// The clothing that will be worn by a player or NPC who wears this outfit.
    /// </summary>
    [ResourceType("clothing")]
    public List<string> Clothing { get; set; }
    /// <summary>
    /// If true, hats will replace hair. If false, hats will be added on top of hair.
    /// </summary>
    public bool HatShouldReplaceHair { get; set; } = true;

    public static JobUniform Get(string name)
    {
        var uniformName = name.Split('.').First();
        return ResourceLibrary
            .GetAll<JobUniform>()
            .Where(u => System.IO.Path.GetFileNameWithoutExtension(u.ResourcePath) == uniformName)
            .FirstOrDefault();
    }

    public ClothingContainer GetOutfit(ClothingContainer existing = null)
    {
        var finalClothing = new ClothingContainer();

        if (existing != null)
        {
            foreach (var clothing in existing.Clothing)
            {
                finalClothing.Add(clothing);
            }
        }
        foreach (var clothingPath in Clothing)
        {
            var article = ResourceLibrary.Get<Clothing>(clothingPath);
            if (!HatShouldReplaceHair && article.Category == Sandbox.Clothing.ClothingCategory.Hat)
            {
                // If we force hats to replace hair, we're still removing existing hats,
                // so do that manually here.
                var hats = finalClothing
                    .Clothing
                    .Where(c => c.Category == Sandbox.Clothing.ClothingCategory.Hat && c.SubCategory != "glasses")
                    .ToList();
                foreach (var hat in hats)
                {
                    finalClothing.Remove(hat);
                }
                finalClothing.ForceAdd(article);
                continue;
            }
            finalClothing.Add(article);
        }
        return finalClothing;
    }
}
