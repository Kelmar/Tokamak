using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace Tokamak.Readers.SVG
{
    /// <summary>
    /// Reader for loading Scalable Vector Graphics format.
    /// </summary>
    public sealed class SVGReader : IDisposable
    {
        private readonly bool m_disposeReader;
        private readonly TextReader m_reader;
        private readonly XDocument m_doc;

        public SVGReader(TextReader reader, bool disposeReader = true)
        {
            ArgumentNullException.ThrowIfNull(reader, nameof(reader));

            m_disposeReader = disposeReader;
            m_reader = reader;

            m_doc = XDocument.Load(m_reader);
        }

        public SVGReader(Stream input, Encoding? encoding = null, bool closeStream = true)
            : this(new StreamReader(input, encoding ?? Encoding.UTF8, closeStream))
        {
        }

        public void Dispose()
        {
            if (m_disposeReader)
                m_reader.Dispose();

            GC.SuppressFinalize(this);
        }

        private void ProcessGenericAttributes(XElement element)
        {
            var fill = element.Attribute("fill")?.Value;
            var stroke = element.Attribute("stroke")?.Value;
        }

        private void ProcessRect(XElement rect)
        {
            ProcessGenericAttributes(rect);

            var x = rect.Attribute("x")?.Value;
            var y = rect.Attribute("y")?.Value;
            var width = rect.Attribute("width")?.Value;
            var height = rect.Attribute("height")?.Value;
        }

        public void Import()
        {
            if (m_doc.Root == null)
                return;

            foreach (var child in m_doc.Root.Elements())
            {
                switch (child.Name.LocalName)
                {
                case "rect": ProcessRect(child); break;
                default:
                    break;
                }
            }
        }
    }
}
