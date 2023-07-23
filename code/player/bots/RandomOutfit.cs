using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinema;

public static class RandomOutfit
{
    private static IEnumerable<Clothing> _allClothing;

    public static float FacialPercent { get; set; } = 0.4f;
    public static float ShirtlessPercent { get; set; } = 0.01f;
    public static float ZombiePercent { get; set; } = 0.005f;
    public static float SkeletonPercent { get; set; } = 0.005f;
    public static float CoatPercent { get; set; } = 0.4f;

    public static ClothingContainer Generate()
    {
        if (_allClothing == null)
            _allClothing = ResourceLibrary.GetAll<Clothing>();

        var outfit = new ClothingContainer();

        // Add body
        var mayBeShirtless = Game.Random.NextSingle() <= ShirtlessPercent;
        outfit.Add(GetRandomSkin(mayBeShirtless));
        outfit.Add(GetRandomHair());
        outfit.Add(GetRandomEyebrows());
        if (Game.Random.NextSingle() <= FacialPercent)
        {
            outfit.Add(GetRandomFacial());
        }

        // Add clothing
        Clothing top = GetRandomShirt(mayBeShirtless);
        outfit.Add(top);
        if (Game.Random.NextSingle() <= CoatPercent)
        {
            outfit.Add(GetRandomCoat(top));
        }
        outfit.Add(GetRandomBottom());
        outfit.Add(GetRandomFootwear());

        return outfit;
    }

    private static Clothing GetRandomShirt(bool mayBeShirtless)
    {
        return GetRandomClothing(
            c => c.Category == Clothing.ClothingCategory.Tops
                && c.SlotsOver.HasFlag(Clothing.Slots.Chest)
                && !c.HideBody.HasFlag(Clothing.BodyGroups.Legs),
            includeNull: mayBeShirtless,
            exclude: new[] { "Binman Polo Shirt", "Usher Jacket" }
            );
    }

    private static Clothing GetRandomCoat(Clothing shirt)
    {
        return GetRandomClothing(
                c => c.Category == Clothing.ClothingCategory.Tops
                && (shirt != null && c.CanBeWornWith(shirt))
                && c.SubCategory != "Vests",
                exclude: new[] { "Army Vest" }
                );
    }

    private static Clothing GetRandomBottom()
    {
        return GetRandomClothing(
            c => c.Category == Clothing.ClothingCategory.Bottoms,
            includeNull: false,
            exclude: new[] { "Leg Armour", "Cardboard Trousers", "Bin Man Trousers", "Usher Trousers" }
            );
    }

    private static Clothing GetRandomFootwear()
    {
        return GetRandomClothing(c => c.Category == Clothing.ClothingCategory.Footwear);
    }

    private static Clothing GetRandomSkin(bool mayBeShirtless)
    {

        var zombie = new[] { "Zombie", "Zombie 2" };
        var skeleton = new[] { "Skeleton", "Skeleton Clean" };
        var muscley = new[] { "Muscley", "Muscley 2", "Muscley 3" };

        var excludedSkins = new List<string>();
        if (Game.Random.NextSingle() <= ZombiePercent)
            excludedSkins.AddRange(zombie);
        if (Game.Random.NextSingle() <= SkeletonPercent)
            excludedSkins.AddRange(skeleton);
        if (!mayBeShirtless)
            excludedSkins.AddRange(muscley);

        return GetRandomClothing(
            c => c.Category == Clothing.ClothingCategory.Skin,
            exclude: excludedSkins.ToArray()
            );
    }

    private static Clothing GetRandomFacial()
    {
        return GetRandomClothing(c => c.Category == Clothing.ClothingCategory.Facial);
    }

    private static Clothing GetRandomHair()
    {
        return GetRandomClothing(
            c => c.Category == Clothing.ClothingCategory.Hair
            && c.SlotsUnder.HasFlag(Clothing.Slots.HeadTop)
            );
    }

    private static Clothing GetRandomEyebrows()
    {
        return GetRandomClothing(
            c => c.Category == Clothing.ClothingCategory.Hair
            && c.SlotsUnder.HasFlag(Clothing.Slots.EyeBrows)
            );
    }

    private static Clothing GetRandomClothing(Func<Clothing, bool> predicate, bool includeNull = true, params string[] exclude)
    {
        var options = _allClothing.Where(predicate);
        if (exclude != null)
        {
            options = options.Where(c => !exclude.Contains(c.Title));
        }
        if (includeNull)
        {
            options = options.Append(null);
        }
        return options.OrderBy(_ => Guid.NewGuid()).FirstOrDefault();
    }
}
