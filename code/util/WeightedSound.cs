using System.Collections.Generic;
using Sandbox;

namespace Cinema;

public class WeightedSoundEffect
{
    public class Sound
    {
        /// <summary>
        /// Path to the sound effect
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The weight of the sound
        /// </summary>
        public float Weight { get; set; }
    }

    /// <summary>
    /// The list of sounds
    /// </summary>
    public List<Sound> Sounds { get; private set; } = new();

    /// <summary>
    /// The total weight of all sounds
    /// </summary>
    public float TotalWeight { get; private set; } = 0;

    public static WeightedSoundEffect Create()
    {
        return new WeightedSoundEffect();
    }

    /// <summary>
    /// Adds a sound to the list
    /// </summary>
    /// <param name="sound">The sound to add</param>
    public WeightedSoundEffect Add(Sound sound)
    {
        Sounds.Add(sound);
        TotalWeight += sound.Weight;

        return this;
    }

    /// <summary>
    /// Adds a sound to the list
    /// </summary>
    /// <param name="sound">The sound to add</param>
    /// <param name="weight">The weight of the sound</param>
    public WeightedSoundEffect Add(string sound, float weight)
    {
        Add(new Sound
        {
            Path = sound,
            Weight = weight
        });

        return this;
    }

    /// <summary>
    /// Gets a random sound from the list
    /// </summary>
    /// <returns>The sound</returns>
    public string GetRandom()
    {
        var random = Game.Random.Float(0, TotalWeight);

        foreach (var sound in Sounds)
        {
            if (random < sound.Weight)
            {
                return sound.Path;
            }

            random -= sound.Weight;
        }

        return null;
    }
}
