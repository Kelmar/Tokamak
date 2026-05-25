using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Tokamak.Readers.FBX.ObjectWrappers
{
    internal class GlobalSettings
    {
        public GlobalSettings(Node rootNode)
        {
            var globalSettings = rootNode
                .GetChildren("GlobalSettings")
                .FirstOrDefault();

            LoadSettings(globalSettings);
        }

        private void LoadSettings(Node? node)
        {
            if (node == null)
                return;

            var properties = node
                .GetChildren("Properties70")
                .SelectMany(p => p.GetChildren("P"));

            foreach (var prop in properties)
            {
                if (prop.Properties.Count < 5)
                    continue;

                string realName = prop.Properties[0].ToString();
                var propVal = prop.Properties[4];

                switch (propVal.Type)
                {
                case PropertyType.String:
                case PropertyType.Boolean:
                case PropertyType.FloatArray:
                case PropertyType.IntArray:
                case PropertyType.LongArray:
                case PropertyType.BoolArray:
                case PropertyType.RawBinary:
                case PropertyType.SignedLong:
                    continue;
                }

                int i = propVal.AsInt();

                switch (realName)
                {
                case "UpAxis":
                    UpAxis = i;
                    break;

                case "UpAxisSign":
                    UpAxisSign = i;
                    break;

                case "FrontAxis":
                    ForwardAxis = i;
                    break;

                case "FrontAxisSign":
                    ForwardAxisSign = i;
                    break;

                case "CoordAxis":
                    RightAxis = i;
                    break;

                case "CoordAxisSign":
                    RightAxisSign = i;
                    break;
                }
            }

            // TODO: Validate read in settings.
        }

        public int UpAxis { get; set; } = 1; // Default Y

        public int UpAxisSign { get; set; } = 1;

        public int ForwardAxis { get; set; } = 2; // Default Z

        public int ForwardAxisSign { get; set; } = 1;

        public int RightAxis { get; set; } = 0; // Default X

        public int RightAxisSign { get; set; } = 1;

        public Vector3 MapToVector(float[] values)
        {
            return new Vector3(
                values[RightAxis] * RightAxisSign,
                values[UpAxis] * UpAxisSign,
                values[ForwardAxis] * ForwardAxisSign
            );
        }
    }
}
