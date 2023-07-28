namespace Cinema;

public partial class SodaFountain
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns> </returns>
    public static string GetSodaFillParticlePath(SodaType type)
    {
        return type switch
        {
            SodaType.Conk => "particles/soda_fountain/walker/sodafill2_conk_f.vpcf",
            SodaType.MionPisz => "particles/soda_fountain/walker/sodafill2_mionpisz_f.vpcf",
            SodaType.Spooge => "particles/soda_fountain/walker/sodafill2_spooge_f.vpcf",

            _ => "particles/soda_fountain/walker/sodafill2_conk_f.vpcf"
        };
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns> </returns>
    public static string GetSodaDispenseParticlePath(SodaType type)
    {
        return type switch
        {
            SodaType.Conk => "particles/soda_fountain/walker/sodafill1_nocup_conk_f.vpcf",
            SodaType.MionPisz => "particles/soda_fountain/walker/sodafill1_nocup_mionpisz_f.vpcf",
            SodaType.Spooge => "particles/soda_fountain/walker/sodafill1_nocup_spooge_f.vpcf",

            _ => "particles/soda_fountain/walker/sodafill1_nocup_conk_f.vpcf"
        };
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns> </returns>
    public static CupColor GetCupColorBySodaType(SodaType type)
    {
        return type switch
        {
            SodaType.Conk => CupColor.Red,
            SodaType.MionPisz => CupColor.Blue,
            SodaType.Spooge => CupColor.Green,

            _ => CupColor.White
        };
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns> </returns>
    public static string GetCupItemIdBySodaType(SodaType type)
    {
        return type switch
        {
            SodaType.Conk => "soda-conk",
            SodaType.MionPisz => "soda-mionpisz",
            SodaType.Spooge => "soda-spooge",

            _ => "soda"
        };
    }
}
