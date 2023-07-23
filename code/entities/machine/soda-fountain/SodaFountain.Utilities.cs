namespace Cinema;

public partial class SodaFountain
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns> </returns>
    public static string GetSodaFillParticlePath(SodaFountain.SodaType type)
    {
        return type switch
        {
            SodaFountain.SodaType.Conk => "particles/soda_fountain/walker/sodafill2_conk_f.vpcf",
            SodaFountain.SodaType.MionPisz => "particles/soda_fountain/walker/sodafill2_mionpisz_f.vpcf",
            SodaFountain.SodaType.Spooge => "particles/soda_fountain/walker/sodafill2_spooge_f.vpcf",

            _ => "particles/soda_fountain/walker/sodafill2_conk_f.vpcf"
        };
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns> </returns>
    public static string GetSodaDispenseParticlePath(SodaFountain.SodaType type)
    {
        return type switch
        {
            SodaFountain.SodaType.Conk => "particles/soda_fountain/walker/sodafill1_nocup_conk_f.vpcf",
            SodaFountain.SodaType.MionPisz => "particles/soda_fountain/walker/sodafill1_nocup_mionpisz_f.vpcf",
            SodaFountain.SodaType.Spooge => "particles/soda_fountain/walker/sodafill1_nocup_spooge_f.vpcf",

            _ => "particles/soda_fountain/walker/sodafill1_nocup_conk_f.vpcf"
        };
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns> </returns>
    public static CupColor GetCupColorBySodaType(SodaFountain.SodaType type)
    {
        return type switch
        {
            SodaFountain.SodaType.Conk => CupColor.Red,
            SodaFountain.SodaType.MionPisz => CupColor.Blue,
            SodaFountain.SodaType.Spooge => CupColor.Green,

            _ => CupColor.White
        };
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns> </returns>
    public static string GetCupItemIdBySodaType(SodaFountain.SodaType type)
    {
        return type switch
        {
            SodaFountain.SodaType.Conk => "soda-conk",
            SodaFountain.SodaType.MionPisz => "soda-mionpisz",
            SodaFountain.SodaType.Spooge => "soda-spooge",

            _ => "soda"
        };
    }
}
