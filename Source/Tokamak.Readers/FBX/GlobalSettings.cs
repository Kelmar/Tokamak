using System;
using System.Linq;
using System.Numerics;

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

            var compoundProperties = CompoundProperty.BuildAllFor(node).ToList();

            foreach (var prop in compoundProperties)
            {
                switch (prop.Name)
                {
                case "UpAxis":
                    UpAxis = Convert.ToInt32(prop.Data);
                    break;

                case "UpAxisSign":
                    UpAxisSign = Convert.ToInt32(prop.Data);
                    break;

                case "FrontAxis":
                    ForwardAxis = Convert.ToInt32(prop.Data);
                    break;

                case "FrontAxisSign":
                    ForwardAxisSign = Convert.ToInt32(prop.Data);
                    break;

                case "CoordAxis":
                    RightAxis = Convert.ToInt32(prop.Data);
                    break;

                case "CoordAxisSign":
                    RightAxisSign = Convert.ToInt32(prop.Data);
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
