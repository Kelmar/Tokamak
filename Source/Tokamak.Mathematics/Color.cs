using System;
using System.Numerics;

namespace Tokamak.Mathematics
{
    public struct Color
    {
        private const float BYTE_MAX_F = (float)Byte.MaxValue;

        // TODO: Make this configurable
        private const double GAMMA_VALUE = 2.2;

        private static readonly Color s_black = new Color(0, 0, 0);

        private static readonly Color s_darkBlue = new Color(0, 0, 128);
        private static readonly Color s_darkGreen = new Color(0, 128, 0);
        private static readonly Color s_darkCyan = new Color(0, 128, 128);
        private static readonly Color s_darkRed = new Color(128, 0, 0);
        private static readonly Color s_purple = new Color(128, 0, 128);
        private static readonly Color s_brown = new Color(128, 128, 0);
        private static readonly Color s_grey = new Color(192, 192, 192);
        private static readonly Color s_darkGrey = new Color(128, 128, 128);

        private static readonly Color s_liteBlue = new Color(0, 0, 255);
        private static readonly Color s_liteGreen = new Color(0, 255, 0);
        private static readonly Color s_liteCyan = new Color(0, 255, 255);
        private static readonly Color s_liteRed = new Color(255, 0, 0);
        private static readonly Color s_magenta = new Color(255, 0, 255);
        private static readonly Color s_yellow = new Color(255, 255, 0);

        private static readonly Color s_white = new Color(255, 255, 255);

        public static ref readonly Color Black => ref s_black;

        public static ref readonly Color DarkBlue => ref s_darkBlue;
        public static ref readonly Color DarkGreen => ref s_darkGreen;
        public static ref readonly Color DarkCyan => ref s_darkCyan;
        public static ref readonly Color DarkRed => ref s_darkRed;
        public static ref readonly Color Purple => ref s_purple;
        public static ref readonly Color Brown => ref s_brown;
        public static ref readonly Color Grey => ref s_grey;

        public static ref readonly Color DarkGrey => ref s_darkGrey;
        public static ref readonly Color LiteBlue => ref s_liteBlue;
        public static ref readonly Color LiteGreen => ref s_liteGreen;
        public static ref readonly Color LiteCyan => ref s_liteCyan;
        public static ref readonly Color LiteRed => ref s_liteRed;
        public static ref readonly Color Magenta => ref s_magenta;
        public static ref readonly Color Yellow => ref s_yellow;

        public static ref readonly Color White => ref s_white;

        public byte Red { get; set; }

        public byte Green { get; set; }

        public byte Blue { get; set; }

        public byte Alpha { get; set; }

        public Color(byte red, byte green, byte blue, byte alpha = Byte.MaxValue)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        /// <summary>
        /// Gets a color from Hue, Saturation, Value, and Alpha values
        /// </summary>
        public static Color FromHSV(float hue, float saturation, float value, byte alpha)
        {
            hue %= 1;

            if (hue < 0)
                hue += 1;

            hue *= 360;
            saturation = MathX.ClampF(saturation, 0, 1);
            value = MathX.ClampF(value, 0, 1);

            float c = value * saturation;
            float x = c * (1 - MathF.Abs((hue / 60) % 2 - 1));
            float m = value - c;

            float r, g, b;

            if (hue < 60)
            {
                r = c;
                g = x;
                b = 0;
            }
            else if (hue < 120)
            {
                r = x;
                g = c;
                b = 0;
            }
            else if (hue < 180)
            {
                r = 0;
                g = c;
                b = x;
            }
            else if (hue < 240)
            {
                r = 0;
                g = x;
                b = c;
            }
            else if (hue < 300)
            {
                r = x;
                g = 0;
                b = c;
            }
            else
            {
                r = c;
                g = 0;
                b = x;
            }

            return new Color(
                MathX.LinearToGamma(r, GAMMA_VALUE),
                MathX.LinearToGamma(g, GAMMA_VALUE),
                MathX.LinearToGamma(b, GAMMA_VALUE),
                alpha);
        }

        /// <summary>
        /// Gets a color from Hue, Saturation, Value, and Alpha values
        /// </summary>
        public static Color FromHSV(float hue, float saturation, float value, float alpha = 1) => FromHSV(hue, saturation, value, alpha.ToByteRange());

        /// <summary>
        /// Gets a color from Hue, Saturation, Value, and Alpha values
        /// </summary>
        public static Color FromHSV(in Vector3 v) => FromHSV(v.X, v.Y, v.Z, 1f);

        /// <summary>
        /// Gets a color from Hue, Saturation, Value, and Alpha values
        /// </summary>
        public static Color FromHSV(in Vector4 v) => FromHSV(v.X, v.Y, v.Z, v.W);

        /// <summary>
        /// Linearly interpolate between two colors.
        /// </summary>
        /// <param name="distance">Distance from color 1 to color 2.</param>
        /// <param name="c1">Color 1</param>
        /// <param name="c2">Color 2</param>
        /// <returns>Mixed color between the two colors.</returns>
        public static Color Lerp(double distance, in Color c1, in Color c2)
        {
            distance = MathX.Clamp(distance, 0, 1);
            double oneMinus = 1 - distance;

            return new Color
            {
                Red = (byte)Math.Round(c1.Red * oneMinus + c2.Red * distance),
                Green = (byte)Math.Round(c1.Green * oneMinus + c2.Green * distance),
                Blue = (byte)Math.Round(c1.Blue * oneMinus + c2.Blue * distance),
                Alpha = (byte)Math.Round(c1.Alpha * oneMinus + c2.Alpha * distance)
            };
        }

        // Convert to/from Vector4
        public static explicit operator Vector4(in Color c) => new Vector4(
            (float)MathX.GammaToLinear(c.Red, GAMMA_VALUE),
            (float)MathX.GammaToLinear(c.Green, GAMMA_VALUE),
            (float)MathX.GammaToLinear(c.Blue, GAMMA_VALUE),
            c.Alpha / BYTE_MAX_F);

        public static explicit operator Color(in Vector4 v) => new Color(
            MathX.LinearToGamma(v.X, GAMMA_VALUE),
            MathX.LinearToGamma(v.Y, GAMMA_VALUE),
            MathX.LinearToGamma(v.Z, GAMMA_VALUE),
            v.W.ToByteRange());
    }
}
