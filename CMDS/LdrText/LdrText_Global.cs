namespace LdrText
{
    public enum O_Txt
    {
        nameApp,
        nameCmd,
        typeObj,
        hLdr,
        scale,
        hTxtBot,
        topX,
        topY,
        dZ
    }

    public enum O_Ldr
    {
        nameApp,
        nameCmd,
        typeObj,
        hTxtTop,
        hTxtBot,
        ldr1X,
        ldr1Y,
        ldr2X,
        ldr2Y,
        ldr3X,
        ldr3Y
    }

    public enum O_Pnt
    {
        nameApp,
        nameCmd,
        hTxtTop
    }

    public enum GS_Txt
    {
        nameApp,
        nameCmd,
        typeObj,
        hLdr,
        scale,
        station,
        offset,
        topX,
        topY,
        idCgPnt1,
        idCgPnt2
    }

    public enum GS_Ldr
    {
        nameApp,
        nameCmd,
        typeObj,
        hTxt,
        ldr1X,
        ldr1Y,
        ldr2X,
        ldr2Y,
        ldrMX,
        ldrMY,
        angle
    }

    public enum GS_Pnt
    {
        nameApp,
        nameCmd,
        hTxt
    }

    public struct SegProps
    {
        public static double dir;
        public static double len;
    }
}
