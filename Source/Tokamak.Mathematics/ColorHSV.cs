using System;
using System.Numerics;

namespace Tokamak.Mathematics
{
    /// <summary>
    /// Color Hue Saturation Value extensions.
    /// </summary>
    public static class ColorHSV
    {
        extension (in Color color)
        {
            /// <summary>
            /// Gets a color from Hue, Saturation, Value, and Alpha values
            /// </summary>
            public static Color FromHSV(float hue, float saturation, float value, byte alpha, double gamma = Color.DefaultGamma)
            {
                hue %= 1;

                if (hue < 0)
                    hue += 1;

                hue *= 360;
                saturation = Math.Clamp(saturation, 0, 1);
                value = Math.Clamp(value, 0, 1);

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
                    Color.LinearToGamma(r, gamma),
                    Color.LinearToGamma(g, gamma),
                    Color.LinearToGamma(b, gamma),
                    alpha
                );
            }

            /// <summary>
            /// Gets a color from Hue, Saturation, Value, and Alpha values
            /// </summary>
            public static Color FromHSV(float hue, float saturation, float value, float alpha = 1, double gamma = Color.DefaultGamma)
                => FromHSV(hue, saturation, value, float.ToByteRange(alpha), gamma);

            /// <summary>
            /// Gets a color from a <seealso cref="Vector3"/> representing Hue, Saturation, and Value components.
            /// </summary>
            /// <remarks>
            /// The Vector3's items map as:
            /// <list type="table">
            ///     <item><term>X</term><description>Hue</description></item>
            ///     <item><term>Y</term><description>Saturation</description></item>
            ///     <item><term>Z</term><description>Value</description></item>
            /// </list>
            /// </remarks>
            public static Color FromHSV(in Vector3 v, double gamma = Color.DefaultGamma)
                => FromHSV(v.X, v.Y, v.Z, 1f, gamma);

            /// <summary>
            /// Gets a color from a <seealso cref="Vector4"/> representing Hue, Saturation, Value, and Alpha values
            /// </summary>
            /// <remarks>
            /// The Vector4's items map as:
            /// <list type="table">
            ///     <item><term>X</term><description>Hue</description></item>
            ///     <item><term>Y</term><description>Saturation</description></item>
            ///     <item><term>Z</term><description>Value</description></item>
            ///     <item><term>W</term><description>Alpha</description></item>
            /// </list>
            /// </remarks>
            public static Color FromHSV(in Vector4 v, double gamma = Color.DefaultGamma)
                => FromHSV(v.X, v.Y, v.Z, v.W, gamma);
        }
    }
}
