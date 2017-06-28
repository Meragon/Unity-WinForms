using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Unity.Controls
{
    public class BitmapFont
    {
        internal Dictionary<int, BitmapChar> textureList;

        public bool Loaded { get; private set; }

        public BitmapFont()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fontImage"></param>
        /// <param name="format">GHL format (FontBuilder)</param>
        public void Load(Bitmap fontImage, byte[] format)
        {
            var ser = new XmlSerializer(typeof(font));
            using (var str = new System.IO.MemoryStream(format))
            {
                var font = (font)ser.Deserialize(str);
                textureList = new Dictionary<int, BitmapChar>();
                for (int i = 0; i < font.chars.Length; i++)
                {
                    var fChar = font.chars[i];

                    var bChar = new BitmapChar();
                    bChar.Id = fChar.id[0];
                    bChar.Advance = fChar.advance;

                    var charRect = fChar.rect.Split(' ');
                    int charX = Convert.ToInt32(charRect[0]);
                    int charY = Convert.ToInt32(charRect[1]);
                    int charW = Convert.ToInt32(charRect[2]);
                    int charH = Convert.ToInt32(charRect[3]);

                    var offset = fChar.offset.Split(' ');
                    bChar.OffsetX = Convert.ToInt32(offset[0]);
                    bChar.OffsetY = Convert.ToInt32(offset[1]);

                    bChar.Texture = new Bitmap(charW, charH);
                    if (charW > 0 && charH > 0)
                    {
                        
                    }

                    if (charW > 0 && charH > 0)
                        for (int y = charY, by = 0; by < charH; y++, by++)
                            for (int x = charX, bx = 0; bx < charW; x++, bx++)
                            {
                                var c1 = fontImage.GetPixel(x, y);
                                bChar.Texture.SetPixel(bx, by, c1);
                            }

                    bChar.Texture.Apply();
                    textureList.Add(bChar.Id, bChar);
                }
            }

            Loaded = true;
        }

        public class BitmapChar
        {
            public byte Advance { get; set; }
            public int Id { get; set; }
            public int OffsetX { get; set; }
            public int OffsetY { get; set; }
            public Bitmap Texture { get; set; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public partial class font
        {

            private fontDescription descriptionField;

            private fontMetrics metricsField;

            private fontTexture textureField;

            private fontChar[] charsField;

            private string typeField;

            /// <remarks/>
            public fontDescription description
            {
                get
                {
                    return descriptionField;
                }
                set
                {
                    descriptionField = value;
                }
            }

            /// <remarks/>
            public fontMetrics metrics
            {
                get
                {
                    return metricsField;
                }
                set
                {
                    metricsField = value;
                }
            }

            /// <remarks/>
            public fontTexture texture
            {
                get
                {
                    return textureField;
                }
                set
                {
                    textureField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlArrayItemAttribute("char", IsNullable = false)]
            public fontChar[] chars
            {
                get
                {
                    return charsField;
                }
                set
                {
                    charsField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string type
            {
                get
                {
                    return typeField;
                }
                set
                {
                    typeField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class fontDescription
        {

            private byte sizeField;

            private string familyField;

            private string styleField;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public byte size
            {
                get
                {
                    return sizeField;
                }
                set
                {
                    sizeField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string family
            {
                get
                {
                    return familyField;
                }
                set
                {
                    familyField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string style
            {
                get
                {
                    return styleField;
                }
                set
                {
                    styleField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class fontMetrics
        {

            private byte ascenderField;

            private byte heightField;

            private sbyte descenderField;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public byte ascender
            {
                get
                {
                    return ascenderField;
                }
                set
                {
                    ascenderField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public byte height
            {
                get
                {
                    return heightField;
                }
                set
                {
                    heightField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public sbyte descender
            {
                get
                {
                    return descenderField;
                }
                set
                {
                    descenderField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class fontTexture
        {

            private ushort widthField;

            private ushort heightField;

            private string fileField;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public ushort width
            {
                get
                {
                    return widthField;
                }
                set
                {
                    widthField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public ushort height
            {
                get
                {
                    return heightField;
                }
                set
                {
                    heightField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string file
            {
                get
                {
                    return fileField;
                }
                set
                {
                    fileField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class fontChar
        {

            private string offsetField;

            private string rectField;

            private byte advanceField;

            private string idField;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string offset
            {
                get
                {
                    return offsetField;
                }
                set
                {
                    offsetField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string rect
            {
                get
                {
                    return rectField;
                }
                set
                {
                    rectField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public byte advance
            {
                get
                {
                    return advanceField;
                }
                set
                {
                    advanceField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string id
            {
                get
                {
                    return idField;
                }
                set
                {
                    idField = value;
                }
            }
        }
    }
}

