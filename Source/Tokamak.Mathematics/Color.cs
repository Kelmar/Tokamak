using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Tokamak.Mathematics
{
    public struct Color
    {
        private const float BYTE_MAX_F = (float)Byte.MaxValue;

        /// <summary>
        /// The default gamma value to use for color space conversions.
        /// </summary>
        /// <remarks>
        /// 2.2 is the gamma value used in PCs and is considered the "industry standard." by many now.
        /// 
        /// Prior to Mac OS X 10.6, Mac's used a gamma value of 1.8, but have sense changed to 2.2.
        /// </remarks>
        public const double DefaultGamma = 2.2;

        /// <summary>
        /// Inverse of the <seealso cref="DefaultGamma" /> constant.
        /// </summary>
        public const double InverseGamma = 1.0 / DefaultGamma;

        #region Color Pseudo Constants

        private static readonly Color s_black = new Color(0, 0, 0);

        private static readonly Color s_darkBlue = new Color(0, 0, 128);
        private static readonly Color s_darkGreen = new Color(0, 128, 0);
        private static readonly Color s_darkCyan = new Color(0, 128, 128);
        private static readonly Color s_darkRed = new Color(128, 0, 0);
        private static readonly Color s_purple = new Color(128, 0, 128);
        private static readonly Color s_brown = new Color(113, 91, 79);
        private static readonly Color s_darkYellow = new Color(128, 128, 0);
        private static readonly Color s_grey = new Color(192, 192, 192);
        private static readonly Color s_darkGrey = new Color(128, 128, 128);

        private static readonly Color s_liteBlue = new Color(0, 0, 255);
        private static readonly Color s_liteGreen = new Color(0, 255, 0);
        private static readonly Color s_liteCyan = new Color(0, 255, 255);
        private static readonly Color s_liteRed = new Color(255, 0, 0);
        private static readonly Color s_magenta = new Color(255, 0, 255);
        private static readonly Color s_beige = new Color(208, 197, 189);
        private static readonly Color s_yellow = new Color(255, 255, 0);

        private static readonly Color s_white = new Color(255, 255, 255);

        public static ref readonly Color Black => ref s_black;

        public static ref readonly Color DarkBlue => ref s_darkBlue;
        public static ref readonly Color DarkGreen => ref s_darkGreen;
        public static ref readonly Color DarkCyan => ref s_darkCyan;
        public static ref readonly Color DarkRed => ref s_darkRed;
        public static ref readonly Color Purple => ref s_purple;
        public static ref readonly Color Brown => ref s_brown;
        public static ref readonly Color DarkYellow => ref s_darkYellow;
        public static ref readonly Color Grey => ref s_grey;
        public static ref readonly Color Gray => ref s_grey;

        public static ref readonly Color DarkGrey => ref s_darkGrey;
        public static ref readonly Color DarkGray => ref s_darkGrey;
        public static ref readonly Color LiteBlue => ref s_liteBlue;
        public static ref readonly Color LiteGreen => ref s_liteGreen;
        public static ref readonly Color LiteCyan => ref s_liteCyan;
        public static ref readonly Color LiteRed => ref s_liteRed;
        public static ref readonly Color Magenta => ref s_magenta;
        public static ref readonly Color Yellow => ref s_yellow;
        public static ref readonly Color Beige => ref s_beige;

        public static ref readonly Color White => ref s_white;

        #endregion Color Pseudo Constants

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
        /// Converts the color to a 4 element RGBA byte array.
        /// </summary>
        /// <returns>An array with red, green, blue and alpha in that order.</returns>
        public byte[] ToArrayRGBA()
            => [Red, Green, Blue, Alpha];

        /// <summary>
        /// Converts the color to a 3 element RGB byte array.
        /// </summary>
        /// <returns>An array with red, green, and blue that order.</returns>
        public byte[] ToArrayRGB()
            => [Red, Green, Blue];

        /// <summary>
        /// Converts the color to a 4 element BGRA byte array.
        /// </summary>
        /// <returns>An array with blue, green, red and alpha in that order.</returns>
        public byte[] ToArrayBGRA()
            => [Blue, Green, Red, Alpha];

        /// <summary>
        /// Converts the color to a 3 element BGR byte array.
        /// </summary>
        /// <returns>An array with blue, green, and red in that order.</returns>
        public byte[] ToArrayBGR()
            => [Blue, Green, Red];

        /// <summary>
        /// Convert a Color into a <seealso cref="Vector3"/>
        /// </summary>
        /// <param name="gamma">Optional gamma value to use for converting gamma color to linear color.</param>
        public Vector3 ToVector3(double gamma = DefaultGamma)
            => new Vector3(
                (float)GammaToLinear(Red, gamma),
                (float)GammaToLinear(Green, gamma),
                (float)GammaToLinear(Blue, gamma)
            );

        /// <summary>
        /// Convert a Color into a <seealso cref="Vector4">
        /// </summary>
        /// <param name="gamma">Optional gamma value to use for converting gamma color to linear color.</param>
        public Vector4 ToVector(double gamma = DefaultGamma)
            => new Vector4(
                (float)GammaToLinear(Red, gamma),
                (float)GammaToLinear(Green, gamma),
                (float)GammaToLinear(Blue, gamma),
                Alpha / BYTE_MAX_F
            );

        /// <summary>
        /// Convert a <seealso cref="Vector3"/> to a Color value.
        /// </summary>
        /// <param name="v">Vector to convert</param>
        /// <param name="alpha">The alpha value for the color.</param>
        /// <param name="gamma">Optional gamma value to use for linear to gamma color space conversion.</param>
        public static Color FromVector(in Vector3 v, float alpha = 1, double gamma = DefaultGamma)
            => new Color(
                LinearToGamma(v.X, DefaultGamma),
                LinearToGamma(v.Y, gamma),
                LinearToGamma(v.Z, gamma),
                float.ToByteRange(alpha)
            );

        /// <summary>
        /// Convert a <seealso cref="Vector4"/> to a Color value.
        /// </summary>
        /// <param name="v">Vector to convert</param>
        /// <param name="gamma">Optional gamma value to use for linear to gamma color space conversion.</param>
        public static Color FromVector(in Vector4 v, double gamma = DefaultGamma)
            => new Color(
                LinearToGamma(v.X, DefaultGamma),
                LinearToGamma(v.Y, gamma),
                LinearToGamma(v.Z, gamma),
                float.ToByteRange(v.W)
            );

        /// <summary>
        /// Convert gamma color value to linear color value.
        /// </summary>
        public static double GammaToLinear(byte b, double gamma = DefaultGamma)
            => Math.Pow(b / 255d, gamma);

        /// <summary>
        /// Convert linear color value to gamma color value.
        /// </summary>
        public static byte LinearToGamma(double d, double gamma = DefaultGamma)
            => (byte)Math.Round(255 * Math.Pow(d, 1 / gamma));

        /// <summary>
        /// Linearly interpolate between two colors.
        /// </summary>
        /// <param name="distance">Distance from color 1 to color 2.</param>
        /// <param name="c1">Color 1</param>
        /// <param name="c2">Color 2</param>
        /// <returns>Mixed color between the two colors.</returns>
        public static Color Lerp(double distance, in Color c1, in Color c2)
        {
            distance = Math.Clamp(distance, 0, 1);
            double oneMinus = 1 - distance;

            return new Color
            {
                Red = (byte)Math.Round(c1.Red * oneMinus + c2.Red * distance),
                Green = (byte)Math.Round(c1.Green * oneMinus + c2.Green * distance),
                Blue = (byte)Math.Round(c1.Blue * oneMinus + c2.Blue * distance),
                Alpha = (byte)Math.Round(c1.Alpha * oneMinus + c2.Alpha * distance)
            };
        }

        // Less specific to more specific, no cast required.
        public static implicit operator Color(in Vector3 v) => Color.FromVector(v);
        public static implicit operator Color(in Vector4 v) => Color.FromVector(v);

        // More specific to less specific, require cast.
        public static explicit operator Vector4(in Color c) => c.ToVector();
        public static explicit operator Vector3(in Color c) => c.ToVector3();
    }
}
