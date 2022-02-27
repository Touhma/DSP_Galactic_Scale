namespace GalacticScale
{
    public enum EStar
    {
        BlackHole,
        WhiteDwarf,
        NeutronStar,
        O,
        B,
        A,
        F,
        G,
        M,
        K,
        BlueGiant,
        WhiteGiant,
        YellowGiant,
        RedGiant
    }

    internal static class EStarMethods
    {
        public static (EStarType, ESpectrType) Convert(this EStar star)
        {
            switch (star)
            {
                case EStar.A: return (EStarType.MainSeqStar, ESpectrType.A);
                case EStar.B: return (EStarType.MainSeqStar, ESpectrType.B);
                case EStar.M: return (EStarType.MainSeqStar, ESpectrType.M);
                case EStar.K: return (EStarType.MainSeqStar, ESpectrType.K);
                case EStar.O: return (EStarType.MainSeqStar, ESpectrType.O);
                case EStar.G: return (EStarType.MainSeqStar, ESpectrType.G);
                case EStar.F: return (EStarType.MainSeqStar, ESpectrType.F);
                case EStar.YellowGiant: return (EStarType.GiantStar, ESpectrType.F);
                case EStar.RedGiant: return (EStarType.GiantStar, ESpectrType.M);
                case EStar.WhiteGiant: return (EStarType.GiantStar, ESpectrType.A);
                case EStar.BlueGiant: return (EStarType.GiantStar, ESpectrType.O);
                case EStar.BlackHole: return (EStarType.BlackHole, ESpectrType.X);
                case EStar.NeutronStar: return (EStarType.NeutronStar, ESpectrType.X);
                case EStar.WhiteDwarf: return (EStarType.WhiteDwarf, ESpectrType.X);
            }

            GS2.Warn($"Failed to convert EStar {star}");
            return (EStarType.WhiteDwarf, ESpectrType.X);
        }
    }
}