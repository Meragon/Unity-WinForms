namespace System.Drawing
{
    public static class Pens
    {
        private static Pen aliceBlue;
        private static Pen antiqueWhite;
        private static Pen aqua;
        private static Pen aquamarine;
        private static Pen azure;
        private static Pen beige;
        private static Pen bisque;
        private static Pen black;
        private static Pen blanchedAlmond;
        private static Pen blue;
        private static Pen blueViolet;
        private static Pen brown;
        private static Pen burlyWood;
        private static Pen cadetBlue;

        public static Pen AliceBlue
        {
            get
            {
                if (aliceBlue == null)
                    aliceBlue = new Pen(Color.AliceBlue);
                return aliceBlue;
            }
        }
        public static Pen AntiqueWhite
        {
            get
            {
                if (antiqueWhite == null)
                    antiqueWhite = new Pen(Color.AntiqueWhite);
                return antiqueWhite;
            }
        }
        public static Pen Aqua
        {
            get
            {
                if (aqua == null)
                    aqua = new Pen(Color.Aqua);
                return aqua;
            }
        }
        public static Pen Aquamarine
        {
            get
            {
                if (aquamarine == null)
                    aquamarine = new Pen(Color.Aquamarine);
                return aquamarine;
            }
        }
        public static Pen Azure
        {
            get
            {
                if (azure == null)
                    azure = new Pen(Color.Azure);
                return azure;
            }
        }
        public static Pen Beige
        {
            get
            {
                if (beige == null)
                    beige = new Pen(Color.Beige);
                return beige;
            }
        }
        public static Pen Bisque
        {
            get
            {
                if (bisque == null)
                    bisque = new Pen(Color.Bisque);
                return bisque;
            }
        }
        public static Pen Black
        {
            get
            {
                if (black == null)
                    black = new Pen(Color.Black);
                return black;
            }
        }
        public static Pen BlanchedAlmond
        {
            get
            {
                if (blanchedAlmond == null)
                    blanchedAlmond = new Pen(Color.BlanchedAlmond);
                return blanchedAlmond;
            }
        }
        public static Pen Blue
        {
            get
            {
                if (blue == null)
                    blue = new Pen(Color.Blue);
                return blue;
            }
        }
        public static Pen BlueViolet
        {
            get
            {
                if (blueViolet == null)
                    blueViolet = new Pen(Color.BlueViolet);
                return blueViolet;
            }
        }
        public static Pen Brown
        {
            get
            {
                if (brown == null)
                    brown = new Pen(Color.Brown);
                return brown;
            }
        }
        public static Pen BurlyWood
        {
            get
            {
                if (burlyWood == null)
                    burlyWood = new Pen(Color.BurlyWood);
                return burlyWood;
            }
        }
        public static Pen CadetBlue
        {
            get
            {
                if (cadetBlue == null)
                    cadetBlue = new Pen(Color.CadetBlue);
                return cadetBlue;
            }
        }

        // TODO: other pens.

        public static Pen Chartreuse = new Pen(Color.Chartreuse);
        public static Pen Chocolate = new Pen(Color.Chocolate);
        public static Pen Coral = new Pen(Color.Coral);
        public static Pen CornflowerBlue = new Pen(Color.CornflowerBlue);
        public static Pen Cornsilk = new Pen(Color.Cornsilk);
        public static Pen Crimson = new Pen(Color.Crimson);
        public static Pen Cyan = new Pen(Color.Cyan);
        public static Pen DarkBlue = new Pen(Color.DarkBlue);
        public static Pen DarkCyan = new Pen(Color.DarkCyan);
        public static Pen DarkGoldenrod = new Pen(Color.DarkGoldenrod);
        public static Pen DarkGray = new Pen(Color.DarkGray);
        public static Pen DarkGreen = new Pen(Color.DarkGreen);
        public static Pen DarkKhaki = new Pen(Color.DarkKhaki);
        public static Pen DarkMagenta = new Pen(Color.DarkMagenta);
        public static Pen DarkOliveGreen = new Pen(Color.DarkOliveGreen);
        public static Pen DarkOrange = new Pen(Color.DarkOrange);
        public static Pen DarkOrchid = new Pen(Color.DarkOrchid);
        public static Pen DarkRed = new Pen(Color.DarkRed);
        public static Pen DarkSalmon = new Pen(Color.DarkSalmon);
        public static Pen DarkSeaGreen = new Pen(Color.DarkSeaGreen);
        public static Pen DarkSlateBlue = new Pen(Color.DarkSlateBlue);
        public static Pen DarkSlateGray = new Pen(Color.DarkSlateGray);
        public static Pen DarkTurquoise = new Pen(Color.DarkTurquoise);
        public static Pen DarkViolet = new Pen(Color.DarkViolet);
        public static Pen DeepPink = new Pen(Color.DeepPink);
        public static Pen DeepSkyBlue = new Pen(Color.DeepSkyBlue);
        public static Pen DimGray = new Pen(Color.DimGray);
        public static Pen DodgerBlue = new Pen(Color.DodgerBlue);
        public static Pen Firebrick = new Pen(Color.Firebrick);
        public static Pen FloralWhite = new Pen(Color.FloralWhite);
        public static Pen ForestGreen = new Pen(Color.ForestGreen);
        public static Pen Fuchsia = new Pen(Color.Fuchsia);
        public static Pen Gainsboro = new Pen(Color.Gainsboro);
        public static Pen GhostWhite = new Pen(Color.GhostWhite);
        public static Pen Gold = new Pen(Color.Gold);
        public static Pen Goldenrod = new Pen(Color.Goldenrod);
        public static Pen Gray = new Pen(Color.Gray);
        public static Pen Green = new Pen(Color.Green);
        public static Pen GreenYellow = new Pen(Color.GreenYellow);
        public static Pen Honeydew = new Pen(Color.Honeydew);
        public static Pen HotPink = new Pen(Color.HotPink);
        public static Pen IndianRed = new Pen(Color.IndianRed);
        public static Pen Indigo = new Pen(Color.Indigo);
        public static Pen Ivory = new Pen(Color.Ivory);
        public static Pen Khaki = new Pen(Color.Khaki);
        public static Pen Lavender = new Pen(Color.Lavender);
        public static Pen LavenderBlush = new Pen(Color.LavenderBlush);
        public static Pen LawnGreen = new Pen(Color.LawnGreen);
        public static Pen LemonChiffon = new Pen(Color.LemonChiffon);
        public static Pen LightBlue = new Pen(Color.LightBlue);
        public static Pen LightCoral = new Pen(Color.LightCoral);
        public static Pen LightCyan = new Pen(Color.LightCyan);
        public static Pen LightGoldenrodYellow = new Pen(Color.LightGoldenrodYellow);
        public static Pen LightGray = new Pen(Color.LightGray);
        public static Pen LightGreen = new Pen(Color.LightGreen);
        public static Pen LightPink = new Pen(Color.LightPink);
        public static Pen LightSalmon = new Pen(Color.LightSalmon);
        public static Pen LightSeaGreen = new Pen(Color.LightSeaGreen);
        public static Pen LightSkyBlue = new Pen(Color.LightSkyBlue);
        public static Pen LightSlateGray = new Pen(Color.LightSlateGray);
        public static Pen LightSteelBlue = new Pen(Color.LightSteelBlue);
        public static Pen LightYellow = new Pen(Color.LightYellow);
        public static Pen Lime = new Pen(Color.Lime);
        public static Pen LimeGreen = new Pen(Color.LimeGreen);
        public static Pen Linen = new Pen(Color.Linen);
        public static Pen Magenta = new Pen(Color.Magenta);
        public static Pen Maroon = new Pen(Color.Maroon);

        public static Pen Transparent = new Pen(Color.Transparent);
    }
}
