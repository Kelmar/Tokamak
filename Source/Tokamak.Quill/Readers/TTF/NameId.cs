using System;

namespace Tokamak.Quill.Readers.TTF
{
    internal enum NameId : UInt16
    {
        /// <summary>
        /// Copyright info
        /// </summary>
        Copyright = 0,

        /// <summary>
        /// Display name (short name)
        /// </summary>
        Family = 1,

        /// <summary>
        /// Display style
        /// </summary>
        Subfamily = 2,

        /// <summary>
        /// Unique ID for application use
        /// </summary>
        FontId = 3,

        /// <summary>
        /// Complete unique human readable name.
        /// </summary>
        FontName = 4,

        /// <summary>
        /// Version info from the vendor
        /// </summary>
        Version = 5,

        /// <summary>
        /// Name of post script font to use for PS printers
        /// </summary>
        PostScriptName = 6,

        /// <summary>
        /// Trademark information
        /// </summary>
        Trademark = 7,

        /// <summary>
        /// Name of the maker of the font. (usually a company or group entity)
        /// </summary>
        Manufacturer = 8,

        /// <summary>
        /// Designer of the font (usually a person)
        /// </summary>
        Designer = 9,

        /// <summary>
        /// Detailed description of the font
        /// </summary>
        Description = 10,

        /// <summary>
        /// URL to the vendor of the font
        /// </summary>
        VendorUrl = 11,

        /// <summary>
        /// URL to the font designer
        /// </summary>
        DesignerUrl = 12,

        /// <summary>
        /// Detailed descriptin of the li
        /// </summary>
        LicenseDetails = 13,

        /// <summary>
        /// Short name of the liscese
        /// </summary>
        License = 14,

        Reserved = 15,

        PreferedFamily = 16,

        PerferedSubfamily = 17,

        /// <summary>
        /// (Mac only)
        /// </summary>
        CompatibleFull = 18,

        SampleText = 19,

        PostScriptPSD = 20,

        WWSFamily = 21,

        WWSSubFamily = 22,

        LightBackground = 23,

        DarkBackground = 24,

        PSVariationsPrefix = 25
    }

}
