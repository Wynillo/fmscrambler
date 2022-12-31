using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using FMLib.Utility;

namespace FMLib.Models
{
    /// <summary>
    /// Model Class for a Card
    /// </summary>
    public class Card
    {
        /// <summary>
        /// ID of the Card as Integer
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the Card as String
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the Card as String
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Attack Value of the Card as Integer
        /// </summary>
        public int Attack { get; set; }

        /// <summary>
        /// Defense Value of the Card as Integer
        /// </summary>
        public int Defense { get; set; }

        /// <summary>
        /// Attribute of the Card as Integer
        /// </summary>
        public int Attribute { get; set; }

        /// <summary>
        /// Level of the Card as Integer
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Type of the Card as Integer
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Guardian Star 1 of the Card as Integer
        /// </summary>
        public int GuardianStar1 { get; set; }

        /// <summary>
        /// Guardian Star 2 of the Card as Integer
        /// </summary>
        public int GuardianStar2 { get; set; }

        /// <summary>
        /// Fusions of the Card as a List
        /// </summary>
        public List<Fusion> Fusions { get; set; } = new List<Fusion>();

        /// <summary>
        /// Equips of the Card as a List
        /// </summary>
        public List<int> Equips { get; set; } = new List<int>();

        /// <summary>
        /// Ritual of the Card as Ritual
        /// </summary>
        public Ritual Rituals { get; set; }

        /// <summary>
        /// Starchip information on the card Cost/Password
        /// </summary>
        public Starchips Starchip { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int DescriptionColor { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int NameColor { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TIM BigImage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TIM NameImage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TIM SmallImage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public class TIM
        {
            /// <summary>
            /// 
            /// </summary>
            public int _bpp;
            private unsafe byte* _current;
            private unsafe static byte* _current1;
            private Dictionary<int, int> bpptoInt;
            private byte[][] clut;
            /// <summary>
            /// 
            /// </summary>
            public int clutIndex;
            /// <summary>
            /// 
            /// </summary>
            public int clutNumber;
            private readonly int colorNumber = 0;
            private int current;
            private static int current1;
            private MemoryStream data;
            private bool hasClut;
            /// <summary>
            /// 
            /// </summary>
            public int Heigth;
            private long imageDataPosition;
            /// <summary>
            /// 
            /// </summary>
            public int maxPalleteIndex;
            /// <summary>
            /// 
            /// </summary>
            public int paletteIndex;
            private Dictionary<int, int> palleteSize;
            /// <summary>
            /// 
            /// </summary>
            public bool Transparency;
            /// <summary>
            /// 
            /// </summary>
            public int Width;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="data"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            /// <param name="bpp"></param>
            /// <param name="hasClut"></param>
            /// <param name="trasnsparency"></param>
            public TIM(byte[] data, int width, int height, int bpp, bool hasClut = true, bool trasnsparency = false)
            {
                Dictionary<int, int> dictionary1 = new Dictionary<int, int> {
                {
                    4,
                    0x20
                },
                {
                    8,
                    0x200
                },
                {
                    0x10,
                    0
                },
                {
                    0x18,
                    0
                }
            };
                this.palleteSize = dictionary1;
                Dictionary<int, int> dictionary2 = new Dictionary<int, int> {
                {
                    8,
                    4
                },
                {
                    9,
                    8
                },
                {
                    2,
                    0x10
                },
                {
                    3,
                    0x18
                }
            };
                this.bpptoInt = dictionary2;
                this.hasClut = true;
                this.hasClut = hasClut;
                this.bpp = bpp;
                this.Width = width;
                this.Heigth = height;
                this.data = new MemoryStream(data);
                this.Transparency = trasnsparency;
                this.readImage();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="bmp"></param>
            /// <param name="bpp"></param>
            /// <param name="hasClut"></param>
            /// <param name="transparency"></param>
            /// <returns></returns>
            public static unsafe TIM bmpToTim(Bitmap bmp, int bpp, bool hasClut = true, bool transparency = false)
            {
                List<Color> list = new List<Color>();
                int num = 0;
                _current1 = (byte*)bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb).Scan0;
                current1 = (int)_current1;
                using (MemoryStream stream = new MemoryStream())
                {
                    for (int i = 0; i < bmp.Height; i++)
                    {
                        for (int j = 0; j < bmp.Width; j++)
                        {
                            if (bpp == 4)
                            {
                                Color item = GetPixel(j, i, bmp.Width);
                                if ((list.IndexOf(item) == -1) && (list.Count < 0x10))
                                {
                                    list.Add(item);
                                    num++;
                                }
                                Color color2 = GetPixel(j + 1, i, bmp.Width);
                                if ((list.IndexOf(color2) == -1) && (list.Count < 0x10))
                                {
                                    list.Add(color2);
                                    num++;
                                }
                                byte num4 = (byte)((list.IndexOf(item) & 15) | ((list.IndexOf(color2) & 15) << 4));
                                stream.WriteByte(num4);
                                j++;
                            }
                            else if (bpp == 8)
                            {
                                Color color3 = GetPixel(j, i, bmp.Width);
                                if ((list.IndexOf(color3) == -1) && (list.Count < 0x100))
                                {
                                    list.Add(color3);
                                }
                                stream.WriteByte((byte)list.IndexOf(color3));
                            }
                        }
                    }
                    if (hasClut)
                    {
                        foreach (Color color4 in list)
                        {
                            stream.Write(Colorto16Bit(color4), 0, 2);
                        }
                    }
                    return new TIM(stream.ToArray(), bmp.Width, bmp.Height, bpp, hasClut, transparency);
                }
            }

            private Color CLUTColor(int index)
            {
                if (this.clut == null)
                {
                    this.GenerateRandonCLUT();
                }
                index *= 2;
                index += this.paletteIndex * this.palleteSize[this._bpp];
                ushort color = (ushort)(this.clut[this.clutIndex][index] | (this.clut[this.clutIndex][index + 1] << 8));
                return this.Get16bitColor(color);
            }

            private static byte[] Colorto16Bit(Color color)
            {
                byte num = (byte)(color.R / 8);
                byte num2 = (byte)(color.G / 8);
                int num3 = (((((byte)(color.B / 8)) << 10) & 0x7c00) + ((num2 << 5) & 0x3e0)) + (num & 0x1f);
                return new byte[] { ((byte)(num3 & 0xff)), ((byte)((num3 >> 8) & 0xff)) };
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public unsafe Bitmap CreateUnsafeBitmap()
            {
                this.data.Position = this.imageDataPosition;
                Bitmap bitmap = new Bitmap(this.Width, this.Heigth, PixelFormat.Format32bppArgb);
                BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, this.Width, this.Heigth), ImageLockMode.ReadWrite, bitmap.PixelFormat);
                this._current = (byte*)bitmapdata.Scan0;
                this.current = (int)this._current;
                for (int i = 0; i < this.Heigth; i++)
                {
                    for (int j = 0; j < this.Width; j++)
                    {
                        byte index = (byte)this.data.ReadByte();
                        if (this._bpp == 4)
                        {
                            Color color = this.CLUTColor(index & 15);
                            this.SetPixel(j, i, color);
                            color = this.CLUTColor((index >> 4) & 15);
                            this.SetPixel(j + 1, i, color);
                            j++;
                        }
                        else if (this._bpp == 8)
                        {
                            Color color2 = this.CLUTColor(index);
                            this.SetPixel(j, i, color2);
                        }
                        else if (this._bpp == 0x10)
                        {
                            ushort num4 = (ushort)(index + (this.data.ReadByte() << 8));
                            this.SetPixel(j, i, this.Get16bitColor(num4));
                        }
                    }
                }
                bitmap.UnlockBits(bitmapdata);
                return bitmap;
            }

            private void GenerateRandonCLUT()
            {
                this.clut = new byte[][] { new byte[0x200] };
                Random random = new Random(0x21);
                for (int i = 0; i < 0x200; i++)
                {
                    this.clut[0][i] = (byte)random.Next(0, 0xff);
                }
            }

            private Color Get16bitColor(ushort color)
            {
                int red = (color & 0x1f) * 8;
                int green = ((color & 0x3e0) >> 5) * 8;
                int blue = ((color & 0x7c00) >> 10) * 8;
                int num4 = (color & 0x8000) >> 15;
                return Color.FromArgb((((red == 0) && (green == 0)) && (((blue == 0) && (num4 == 0)) && this.Transparency)) ? 0 : 0xff, red, green, blue);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public byte[] getData() =>
                this.data.ToArray();

            private static unsafe Color GetPixel(int x, int y, int Width)
            {
                _current1 = (byte*)(current1 + (((y * Width) + x) * 4));
                return Color.FromArgb(_current1[3], _current1[2], _current1[1], _current1[0]);
            }

            private void ImageNameClut()
            {
                this.clut = new byte[][] { new byte[] {
                0, 0, 0x84, 0x10, 0x84, 0x10, 8, 0x21, 8, 0x21, 8, 0x21, 8, 0x21, 140, 0x31,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0
            } };
            }

            /// <summary>
            /// 
            /// </summary>
            public void readImage()
            {
                if (this.hasClut)
                {
                    this.data.Position = (this.Width * this.Heigth) / ((this._bpp == 4) ? 2 : 1);
                    this.clut = new byte[][] { this.data.ExtractPiece(0, this.palleteSize[this._bpp], -1) };
                }
                else
                {
                    this.ImageNameClut();
                }
                this.imageDataPosition = 0L;
            }

            private unsafe void SetPixel(int x, int y, Color color)
            {
                this._current = (byte*)(this.current + (((y * this.Width) + x) * 4));
                this._current[2] = color.R;
                this._current[1] = color.G;
                this._current[0] = color.B;
                this._current[3] = color.A;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="bmp"></param>
            /// <param name="bpp"></param>
            /// <param name="hasClut"></param>
            /// <param name="transparency"></param>
            /// <returns></returns>
            public static unsafe TIM TestbmpToTim(Bitmap bmp, int bpp, bool hasClut = true, bool transparency = false)
            {
                List<Color> list = new List<Color>();
                _current1 = (byte*)bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb).Scan0;
                current1 = (int)_current1;
                bool flag = false;
                Color transparent = Color.Transparent;
                for (int i = 0; i < bmp.Height; i++)
                {
                    for (int j = 0; j < bmp.Width; j++)
                    {
                        Color item = GetPixel(j, i, bmp.Width);
                        if (!flag)
                        {
                            transparent = item;
                            flag = true;
                        }
                        if ((list.IndexOf(item) == -1) && (list.Count < 0x10))
                        {
                            list.Add(item);
                        }
                    }
                }
                list.Sort((Comparison<Color>)((left, right) => right.GetBrightness().CompareTo(left.GetBrightness())));
                list.Remove(transparent);
                list.Insert(0, transparent);
                using (MemoryStream stream = new MemoryStream())
                {
                    for (int k = 0; k < bmp.Height; k++)
                    {
                        for (int m = 0; m < bmp.Width; m++)
                        {
                            if (bpp == 4)
                            {
                                Color color3 = GetPixel(m, k, bmp.Width);
                                Color color4 = GetPixel(m + 1, k, bmp.Width);
                                byte num5 = (byte)((list.IndexOf(color3) & 15) | ((list.IndexOf(color4) & 15) << 4));
                                stream.WriteByte(num5);
                                m++;
                            }
                            else if (bpp == 8)
                            {
                                Color color5 = GetPixel(m, k, bmp.Width);
                                if ((list.IndexOf(color5) == -1) && (list.Count < 0x100))
                                {
                                    list.Add(color5);
                                }
                                stream.WriteByte((byte)list.IndexOf(color5));
                            }
                        }
                    }
                    if (hasClut)
                    {
                        foreach (Color color6 in list)
                        {
                            stream.Write(Colorto16Bit(color6), 0, 2);
                        }
                    }
                    return new TIM(stream.ToArray(), bmp.Width, bmp.Height, bpp, hasClut, transparency);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="data"></param>
            /// <param name="w"></param>
            /// <param name="h"></param>
            /// <returns></returns>
            public static TIM TIMFromBMP(byte[] data, int w, int h)
            {
                TIM tim;
                using (MemoryStream stream = new MemoryStream(data))
                {
                    using (MemoryStream stream2 = new MemoryStream())
                    {
                        byte num1 = stream.ExtractPiece(0, 1, 2)[0];
                        int num = stream.ExtractPiece(0, 4, 0x12).ExtractInt32(0);
                        int num2 = stream.ExtractPiece(0, 4, -1).ExtractInt32(0);
                        int num3 = stream.ExtractPiece(0, 4, 0x2e).ExtractInt32(0);
                        num3 = (num3 == 0) ? 0xff : num3;
                        stream.Position = 0x36L;
                        for (int i = 0; i < num3; i++)
                        {
                            int blue = stream.ReadByte();
                            int green = stream.ReadByte();
                            int red = stream.ReadByte();
                            stream.ReadByte();
                            stream2.Write(Colorto16Bit(Color.FromArgb(red, green, blue)), 0, 2);
                        }
                        using (MemoryStream stream3 = new MemoryStream())
                        {
                            int num8 = ((((((8 * num) + 0x1f) / 0x20) * 4) * num2) + (num3 * 4)) + 0x36;
                            num8 = (num8 < stream.Length) ? 2 : 0;
                            for (int j = 1; j < (num2 + 1); j++)
                            {
                                stream.Position = (stream.Length - (((((8 * num) + 0x1f) / 0x20) * 4) * j)) - num8;
                                for (int k = 0; k < num; k++)
                                {
                                    stream3.WriteByte((byte)stream.ReadByte());
                                }
                            }
                            stream3.Write(stream2.ToArray(), 0, (int)stream2.Length);
                            tim = new TIM(stream3.ToArray(), w, h, 8, true, false);
                        }
                    }
                }
                return tim;
            }

            /// <summary>
            /// 
            /// </summary>
            public int bpp
            {
                get =>
                    this._bpp;
                set
                {
                    this._bpp = value;
                    if (value < 0x10)
                    {
                        this.maxPalleteIndex = (int)Math.Floor((double)(((double)this.colorNumber) / ((double)this.palleteSize[value])));
                    }
                }
            }
        }
    }
}
