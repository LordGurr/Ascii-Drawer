//jmedved.com

using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Linq;
using System.IO;

namespace MouseTest
{
    internal class Program
    {
        private static Button clear = new Button(36, 1, 20, 5, "Clear");
        private static Button reDraw = new Button(58, 1, 20, 5, "Redraw");
        private static Button white = new Button(79, 1, 10, 4, "White");
        private static Button red = new Button(90, 1, 10, 4, "Red");
        private static Button green = new Button(101, 1, 10, 4, "Green");
        private static Button blue = new Button(112, 1, 10, 4, "Blue");
        private static Button back = new Button(0, 0, Console.BufferWidth - 1, 8, "");
        private static int width = Console.BufferWidth;
        private static int height = Console.BufferHeight;
        private static ConsoleColor colour = ConsoleColor.White;
        private static string brushDraw = "▓";

        private enum State
        {
            drawing,
            files,
        };

        private static State currentState = State.files;

        private static void Main(string[] args)
        {
            var handle = NativeMethods.GetStdHandle(NativeMethods.STD_INPUT_HANDLE);
            Console.ForegroundColor = ConsoleColor.White;
            int mode = 0;
            if (!(NativeMethods.GetConsoleMode(handle, ref mode))) { throw new Win32Exception(); }

            mode |= NativeMethods.ENABLE_MOUSE_INPUT;
            mode &= ~NativeMethods.ENABLE_QUICK_EDIT_MODE;
            mode |= NativeMethods.ENABLE_EXTENDED_FLAGS;

            if (!(NativeMethods.SetConsoleMode(handle, mode))) { throw new Win32Exception(); }

            var record = new NativeMethods.INPUT_RECORD();
            uint recordLen = 0;
            int x = -1;
            int y = -1;
            Stopwatch timeForUpdate = new Stopwatch();
            string[] brush = new string[3]
            {
                string.Concat(Enumerable.Repeat(brushDraw, 2)),
                string.Concat(Enumerable.Repeat(brushDraw, 4)),
                string.Concat(Enumerable.Repeat(brushDraw, 2)),
            };
            int[][] brushOffset = new int[3][]
            {
                new int[2]
                {
                0,
                -1,
                },
                new int[2]
                {
                -1,
                -1,
                },
                new int[2]
                {
                0,
                -1,
                },
            };
            Console.CursorVisible = false;
            timeForUpdate.Start();
            int iconSize = 48;
            if (currentState == State.drawing)
            {
                back.Draw(width);
                green.Draw(width);
                blue.Draw(width);
                red.Draw(width);
                white.Draw(width);
                reDraw.Draw(width);
                clear.Draw(width);

                MyConsole.SetCursorPosition(0, 12);
                Bitmap bmpSrc = new Bitmap(@"C:\Users\gustav.juul\Pictures\nedladdning (3).png", true);
                ConsoleWriteImage(bmpSrc, iconSize);

                MyConsole.SetCursorPosition(0, MyConsole.CursorTop + 5);
                bmpSrc = new Bitmap(@"C:\Users\gustav.juul\Pictures\nedladdning (3).jfif", true);
                ConsoleWriteImage(bmpSrc, iconSize);
                MyConsole.SetCursorPosition(0, MyConsole.CursorTop + 5);
                bmpSrc = new Bitmap(@"C:\Users\gustav.juul\Pictures\Logo.png", true);
                ConsoleWriteImage(bmpSrc, iconSize);
                MyConsole.SetCursorPosition(0, MyConsole.CursorTop + 5);
                bmpSrc = new Bitmap(@"C:\Users\gustav.juul\Pictures\GBStudioLogoCropped.ico", true);
                ConsoleWriteImage(bmpSrc, iconSize);
                MyConsole.SetCursorPosition(0, MyConsole.CursorTop + 5);
                bmpSrc = new Bitmap(@"C:\Users\gustav.juul\Pictures\Lbs croppped.png", true);
                ConsoleWriteImage(bmpSrc, iconSize);
                MyConsole.SetCursorPosition(0, MyConsole.CursorTop + 5);
                bmpSrc = new Bitmap(@"C:\Users\gustav.juul\Pictures\darkcalc.ico", true);
                ConsoleWriteImage(bmpSrc, iconSize);
                MyConsole.SetCursorPosition(0, MyConsole.CursorTop + 5);
                bmpSrc = new Bitmap(@"C:\Users\gustav.juul\Pictures\MonoGameLogoOnly_128px.png", true);
                ConsoleWriteImage(bmpSrc, iconSize);
                MyConsole.SetCursorPosition(0, MyConsole.CursorTop + 5);
                bmpSrc = new Bitmap(@"C:\Users\gustav.juul\Pictures\Icons\img_colormap.gif", true);
                ClickableImage image = new ClickableImage(0, MyConsole.CursorTop, (int)(iconSize * 1.09), iconSize / 2 + 10, "ColorWheel", bmpSrc, iconSize);
                image.Setup(width);
                image.Draw(width);
                //ConsoleWriteImage(bmpSrc, iconSize);
                DrawColor(width);
            }
            else if (currentState == State.files)
            {
                MyConsole.SetCursorPosition(0, 10);
                string folder = @"C:\Users\gustav.juul\source\repos\SpaceShooter\SpaceShooter\Content"; // C:\Users\gustav.juul\source\repos\SpaceShooter\SpaceShooter\Content. C:\Users\gustav.juul\Pictures"
                string[] files = Directory.GetDirectories(folder); // Directory.GetFiles(folder)
                files = files.Concat(Directory.GetFiles(folder)).ToArray();
                int xOffset = 5;
                int yOffset = 5;
                int highestImage = 0;
                for (int i = 0; i < files.Length; i++)
                {
                    //MyConsole.SetCursorPosition(0, MyConsole.CursorTop + 5);
                    Bitmap bmpSrc = new Bitmap(@"C:\Users\gustav.juul\Pictures\Icons\Unknown.png", true);
                    //Bitmap bmpSrc = new Bitmap(@"C:\Users\gustav.juul\Pictures\folderIcon.png", true);
                    try
                    {
                        bmpSrc = new Bitmap(files[i], true);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                        if (Directory.Exists(files[i]))
                        {
                            bmpSrc = new Bitmap(@"C:\Users\gustav.juul\Pictures\folderIcon.png", true);
                        }
                        //continue;
                    }
                    int backslash = files[i].LastIndexOf(@"\") + 1;
                    int length = files[i].Length - backslash;//files[i].LastIndexOf('.') - backslash;
                    if (xOffset + iconSize + 2 > Console.BufferWidth || xOffset + length > Console.BufferWidth)
                    {
                        xOffset = 5;
                        yOffset += highestImage + 5;
                        highestImage = 0;
                    }
                    int start = yOffset;
                    ClickableImage image = new ClickableImage(xOffset, yOffset, iconSize + 2, iconSize / 2 + 10, files[i].Substring(backslash, length > 0 ? length : files[i].Length - backslash), bmpSrc, iconSize);
                    image.Setup(width);
                    image.Draw(width);
                    if (Console.CursorTop - start > highestImage)
                    {
                        highestImage = Console.CursorTop - start;
                    }
                    xOffset += image.width + 6;
                }
            }
            int lastInput = int.MaxValue;
            Console.SetCursorPosition(0, 0);
            while (true)
            {
                if (!NativeMethods.ReadConsoleInput(handle, ref record, 1, ref recordLen)) { throw new Win32Exception(); }
                MyConsole.SetCursorPosition(1, 1);
                switch (record.EventType)
                {
                    case NativeMethods.MOUSE_EVENT:
                        {
                            timeForUpdate.Restart();
                            if (currentState == State.drawing)
                            {
                                MyConsole.WriteLine("Mouse event");
                                MyConsole.WriteLine(string.Format("│    X ...............:   {0,4:0}  ", record.MouseEvent.dwMousePosition.X));
                                MyConsole.WriteLine(string.Format("│    Y ...............:   {0,4:0}  ", record.MouseEvent.dwMousePosition.Y));
                                MyConsole.WriteLine(string.Format("│    dwButtonState ...: 0x{0:X4}  ", record.MouseEvent.dwButtonState));
                                MyConsole.WriteLine(string.Format("│    dwControlKeyState: 0x{0:X4}  ", record.MouseEvent.dwControlKeyState));
                                MyConsole.WriteLine(string.Format("│    dwEventFlags ....: 0x{0:X4}  ", record.MouseEvent.dwEventFlags));
                            }
                            //if (x >= 0 && y >= 0)
                            //{
                            //    MyConsole.SetCursorPosition(x, y);
                            //    MyConsole.Write(" ");
                            //}
                            x = record.MouseEvent.dwMousePosition.X;
                            y = record.MouseEvent.dwMousePosition.Y;
                            if (width != Console.BufferWidth || height != Console.BufferHeight)
                            {
                                ReDraw();
                            }
                            if (record.MouseEvent.dwButtonState == 1)
                            {
                                if (currentState == State.drawing)
                                {
                                    if (!back.Clicked(x, y - 1) && !back.Clicked(x, y + 1))
                                    {
                                        Console.ForegroundColor = colour;
                                        for (int i = 0; i < brush.Length; i++)
                                        {
                                            MyConsole.SetCursorPosition(Clamp(x + brushOffset[i][0], 0, Console.BufferWidth), Clamp(y + i + brushOffset[i][1], 0, Console.BufferHeight));
                                            MyConsole.Write(brush[i]);
                                        }
                                        Console.ForegroundColor = ConsoleColor.White;
                                    }
                                    else if (record.MouseEvent.dwButtonState != lastInput)
                                    {
                                        if (clear.Clicked(x, y))
                                        {
                                            MyConsole.Clear();
                                            back.Draw(width);
                                            green.Draw(width);
                                            blue.Draw(width);
                                            red.Draw(width);
                                            white.Draw(width);
                                            reDraw.Draw(width);
                                            clear.Draw(width);
                                            DrawColor(width);
                                        }
                                        else if (reDraw.Clicked(x, y))
                                        {
                                            ReDraw();
                                        }
                                        else if (white.Clicked(x, y))
                                        {
                                            colour = ConsoleColor.White;
                                            DrawColor(width);
                                        }
                                        else if (red.Clicked(x, y))
                                        {
                                            colour = ConsoleColor.Red;
                                            DrawColor(width);
                                        }
                                        else if (green.Clicked(x, y))
                                        {
                                            colour = ConsoleColor.Green;
                                            DrawColor(width);
                                        }
                                        else if (blue.Clicked(x, y))
                                        {
                                            colour = ConsoleColor.Blue;
                                            DrawColor(width);
                                        }
                                    }
                                }
                            }
                            MyConsole.SetCursorPosition(0, 7);
                            if (currentState == State.drawing)
                            {
                                MyConsole.WriteLine("│    updateTime: " + timeForUpdate.Elapsed.TotalMilliseconds);
                            }
                            lastInput = record.MouseEvent.dwButtonState;
                        }
                        break;

                    case NativeMethods.KEY_EVENT:
                        {
                            MyConsole.WriteLine("Key event  ");
                            MyConsole.WriteLine(string.Format("│    bKeyDown  .......:  {0,5}  ", record.KeyEvent.bKeyDown));
                            MyConsole.WriteLine(string.Format("│    wRepeatCount ....:   {0,4:0}  ", record.KeyEvent.wRepeatCount));
                            MyConsole.WriteLine(string.Format("│    wVirtualKeyCode .:   {0,4:0}  ", record.KeyEvent.wVirtualKeyCode));
                            MyConsole.WriteLine(string.Format("│    uChar ...........:      {0}  ", record.KeyEvent.UnicodeChar));
                            MyConsole.WriteLine(string.Format("│    dwControlKeyState: 0x{0:X4}  ", record.KeyEvent.dwControlKeyState));
                            if (record.KeyEvent.wVirtualKeyCode == (int)ConsoleKey.PrintScreen)
                            {
                                string temp = MyConsole.GetString();
                            }
                            if (record.KeyEvent.wVirtualKeyCode == (int)ConsoleKey.Escape) { Environment.Exit(0); }
                        }
                        break;
                }
            }
        }

        private static void DrawColor(int _width)
        {
            if (_width > 125)
            {
                ConsoleColor temp = Console.ForegroundColor;
                Console.ForegroundColor = colour;
                MyConsole.SetCursorPosition(125, 2);
                MyConsole.Write(String.Concat(Enumerable.Repeat(brushDraw, 6)));
                MyConsole.SetCursorPosition(125, 3);
                MyConsole.Write(String.Concat(Enumerable.Repeat(brushDraw, 6)));
                MyConsole.SetCursorPosition(125, 4);
                MyConsole.Write(String.Concat(Enumerable.Repeat(brushDraw, 6)));
                Console.ForegroundColor = temp;
            }
        }

        /// <summary>
        /// Redraws the screen.
        /// </summary>
        private static void ReDraw()
        {
            MyConsole.ReDraw();
            width = Console.BufferWidth;
            height = Console.BufferHeight;
            //back.Remove();
            if (currentState == State.drawing)
            {
                back.width = width - 1;
                back.Draw(width);
                green.Draw(width);
                red.Draw(width);
                white.Draw(width);
                blue.Draw(width);
                reDraw.Draw(width);
                clear.Draw(width);
                DrawColor(width);
            }
        }

        /// <summary>
        /// Clamps a value between a min and a max.
        /// </summary>
        private static int Clamp(int value, int min, int max)
        {
            return value < min ? min : value > max ? max : value;
        }

        private static int[] cColors = { 0x000000, 0x000080, 0x008000, 0x008080, 0x800000, 0x800080, 0x808000, 0xC0C0C0, 0x808080, 0x0000FF, 0x00FF00, 0x00FFFF, 0xFF0000, 0xFF00FF, 0xFFFF00, 0xFFFFFF };

        public static void ConsoleWritePixel(Color cValue)
        {
            Color[] cTable = cColors.Select(x => Color.FromArgb(x)).ToArray();
            char[] rList = new char[] { (char)9617, (char)9618, (char)9619, (char)9608 }; // 1/4, 2/4, 3/4, 4/4
            int[] bestHit = new int[] { 0, 0, 4, int.MaxValue }; //ForeColor, BackColor, Symbol, Score

            for (int rChar = rList.Length; rChar > 0; rChar--)
            {
                for (int cFore = 0; cFore < cTable.Length; cFore++)
                {
                    for (int cBack = 0; cBack < cTable.Length; cBack++)
                    {
                        int R = (cTable[cFore].R * rChar + cTable[cBack].R * (rList.Length - rChar)) / rList.Length;
                        int G = (cTable[cFore].G * rChar + cTable[cBack].G * (rList.Length - rChar)) / rList.Length;
                        int B = (cTable[cFore].B * rChar + cTable[cBack].B * (rList.Length - rChar)) / rList.Length;
                        int iScore = (cValue.R - R) * (cValue.R - R) + (cValue.G - G) * (cValue.G - G) + (cValue.B - B) * (cValue.B - B);
                        if (!(rChar > 1 && rChar < 4 && iScore > 50000)) // rule out too weird combinations
                        {
                            if (iScore < bestHit[3])
                            {
                                bestHit[3] = iScore; //Score
                                bestHit[0] = cFore;  //ForeColor
                                bestHit[1] = cBack;  //BackColor
                                bestHit[2] = rChar;  //Symbol
                            }
                        }
                    }
                }
            }
            Console.ForegroundColor = (ConsoleColor)bestHit[0];
            Console.BackgroundColor = (ConsoleColor)bestHit[1];
            MyConsole.Write(rList[bestHit[2] - 1]);
        }

        /// <summary>
        /// Writes a image in Console.
        /// </summary>
        public static void ConsoleWriteImage(Bitmap source, int sMax)
        {
            sMax /= 2;
            decimal percent = Math.Min(decimal.Divide(sMax, source.Width), decimal.Divide(sMax, source.Height));
            Size dSize = new Size((int)(source.Width * percent), (int)(source.Height * percent));
            Bitmap bmpMax = new Bitmap(source, dSize.Width * 2, dSize.Height);
            for (int i = 0; i < dSize.Height; i++)
            {
                for (int j = 0; j < dSize.Width; j++)
                {
                    ConsoleWritePixel(bmpMax.GetPixel(j * 2, i));
                    ConsoleWritePixel(bmpMax.GetPixel(j * 2 + 1, i));
                }
                MyConsole.WriteLine();
            }
            Console.ResetColor();
        }

        /// <summary>
        /// Writes a image in Console with a set x starting position.
        /// </summary>
        public static void ConsoleWriteImage(Bitmap source, int sMax, int xPos)
        {
            MyConsole.SetCursorPosition(xPos, MyConsole.CursorTop);
            sMax /= 2;
            decimal percent = Math.Min(decimal.Divide(sMax, source.Width), decimal.Divide(sMax, source.Height));
            Size dSize = new Size((int)(source.Width * percent), (int)(source.Height * percent));
            Bitmap bmpMax = new Bitmap(source, dSize.Width * 2, dSize.Height);
            for (int i = 0; i < dSize.Height; i++)
            {
                for (int j = 0; j < dSize.Width; j++)
                {
                    ConsoleWritePixel(bmpMax.GetPixel(j * 2, i));
                    ConsoleWritePixel(bmpMax.GetPixel(j * 2 + 1, i));
                }
                MyConsole.SetCursorPosition(xPos, MyConsole.CursorTop + 1);
            }
            Console.ResetColor();
        }

        private class NativeMethods
        {
            public const Int32 STD_INPUT_HANDLE = -10;

            public const Int32 ENABLE_MOUSE_INPUT = 0x0010;
            public const Int32 ENABLE_QUICK_EDIT_MODE = 0x0040;
            public const Int32 ENABLE_EXTENDED_FLAGS = 0x0080;

            public const Int32 KEY_EVENT = 1;
            public const Int32 MOUSE_EVENT = 2;

            [DebuggerDisplay("EventType: {EventType}")]
            [StructLayout(LayoutKind.Explicit)]
            public struct INPUT_RECORD
            {
                [FieldOffset(0)]
                public Int16 EventType;

                [FieldOffset(4)]
                public KEY_EVENT_RECORD KeyEvent;

                [FieldOffset(4)]
                public MOUSE_EVENT_RECORD MouseEvent;
            }

            [DebuggerDisplay("{dwMousePosition.X}, {dwMousePosition.Y}")]
            public struct MOUSE_EVENT_RECORD
            {
                public COORD dwMousePosition;
                public Int32 dwButtonState;
                public Int32 dwControlKeyState;
                public Int32 dwEventFlags;
            }

            [DebuggerDisplay("{X}, {Y}")]
            public struct COORD
            {
                public UInt16 X;
                public UInt16 Y;
            }

            [DebuggerDisplay("KeyCode: {wVirtualKeyCode}")]
            [StructLayout(LayoutKind.Explicit)]
            public struct KEY_EVENT_RECORD
            {
                [FieldOffset(0)]
                [MarshalAsAttribute(UnmanagedType.Bool)]
                public Boolean bKeyDown;

                [FieldOffset(4)]
                public UInt16 wRepeatCount;

                [FieldOffset(6)]
                public UInt16 wVirtualKeyCode;

                [FieldOffset(8)]
                public UInt16 wVirtualScanCode;

                [FieldOffset(10)]
                public Char UnicodeChar;

                [FieldOffset(10)]
                public Byte AsciiChar;

                [FieldOffset(12)]
                public Int32 dwControlKeyState;
            };

            public class ConsoleHandle : SafeHandleMinusOneIsInvalid
            {
                public ConsoleHandle() : base(false)
                {
                }

                protected override bool ReleaseHandle()
                {
                    return true; //releasing console handle is not our business
                }
            }

            [DllImportAttribute("kernel32.dll", SetLastError = true)]
            [return: MarshalAsAttribute(UnmanagedType.Bool)]
            public static extern Boolean GetConsoleMode(ConsoleHandle hConsoleHandle, ref Int32 lpMode);

            [DllImportAttribute("kernel32.dll", SetLastError = true)]
            public static extern ConsoleHandle GetStdHandle(Int32 nStdHandle);

            [DllImportAttribute("kernel32.dll", SetLastError = true)]
            [return: MarshalAsAttribute(UnmanagedType.Bool)]
            public static extern Boolean ReadConsoleInput(ConsoleHandle hConsoleInput, ref INPUT_RECORD lpBuffer, UInt32 nLength, ref UInt32 lpNumberOfEventsRead);

            [DllImportAttribute("kernel32.dll", SetLastError = true)]
            [return: MarshalAsAttribute(UnmanagedType.Bool)]
            public static extern Boolean SetConsoleMode(ConsoleHandle hConsoleHandle, Int32 dwMode);
        }

        private static void Box(int StartPointX, int StartPointY, int EndPointX, int EndPointY, int Form, string Ground) //Ritar en box.
        {
            for (int a = StartPointY; a < EndPointY + 1; a++)
            {
                string print = string.Empty;
                for (int i = StartPointX; i < EndPointX + 1; i++)
                {
                    if (a >= 0 && i >= 0)
                    {
                        //MyConsole.SetCursorPosition(i, a);
                        if (i == StartPointX || i == EndPointX || a == StartPointY || a == EndPointY)
                        {
                            if (Form == 1) //Om form är 1 ska den rita den rita den här varje gång
                            {
                                //MyConsole.Write("█");
                                print += ("█");
                            }
                            else if (Form == 2 || Form == 3) //Annars använd de här fina karaktärerna så att det ser snyggare ut
                            {
                                if (i == StartPointX && a == StartPointY)
                                {
                                    //MyConsole.Write("┌");
                                    print += ("┌");
                                }
                                else if (i == EndPointX && a == StartPointY)
                                {
                                    //MyConsole.Write("┐");
                                    print += ("┐");
                                }
                                else if (i == StartPointX && a == EndPointY)
                                {
                                    //MyConsole.Write("└");
                                    print += ("└");
                                }
                                else if (i == EndPointX && a == EndPointY)
                                {
                                    //MyConsole.Write("┘");
                                    print += ("┘");
                                }
                                else if (a == StartPointY || a == EndPointY)
                                {
                                    //MyConsole.Write("─");
                                    print += ("─");
                                }
                                else
                                {
                                    //MyConsole.Write("│");
                                    print += ("│");
                                }
                            }
                        }
                        else if (Form != 3) //Annars ritar den marken
                        {
                            ConsoleColor temp = Console.ForegroundColor;
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            //MyConsole.Write(Ground);
                            print += (Ground);
                            Console.ForegroundColor = temp;
                        }
                    }
                }
                MyConsole.SetCursorPosition(StartPointX, a);
                MyConsole.Write(print);
            }
        }

        private class Button
        {
            public int x { get; protected set; }
            public int y { get; protected set; }
            public int width;
            public int height { get; protected set; }
            public string text { get; protected set; }

            public Button(int _x, int _y, int _width, int _height, string _text)
            {
                x = _x;
                y = _y;
                width = _width;
                height = _height;
                text = _text;
            }

            public bool Clicked(int xPos, int yPos)
            {
                return (xPos >= x && xPos <= x + width && yPos >= y && yPos <= y + height);
                //return (xPos >= x - 1 && xPos <= x + width + 1 && yPos >= y - 1 && yPos <= y + height + 1);
            }

            public virtual void Draw(int screenWidth)
            {
                if (screenWidth > x)
                {
                    Box(x, y, x + width, y + height, 2, " ");
                    if (screenWidth > x + width / 2 - text.Length / 2)
                    {
                        MyConsole.SetCursorPosition(x + width / 2 - text.Length / 2, y + height / 2);
                        MyConsole.Write(text);
                    }
                }
            }

            public void Remove()
            {
                Box(x, y, x + width, y + height, 4, " ");
                MyConsole.SetCursorPosition(x + width / 2 - text.Length / 2, y + height / 2);
                //MyConsole.Write(text);
            }
        }

        private class ClickableImage : Button
        {
            private Bitmap image;
            private int maxSize;
            private int imageWidth;

            public ClickableImage(int _x, int _y, int _width, int _height, string _text, Bitmap _image, int _maxSize)
                : base(_x, _y, _width, _height, _text)
            {
                image = _image;
                maxSize = _maxSize;
            }

            public void Setup(int screenWidth)
            {
                if (screenWidth > x)
                {
                    if (width < maxSize)
                    {
                        maxSize = width;
                    }

                    MyConsole.SetCursorPosition(0, y + 2);
                    ConsoleWriteImage(image, maxSize, x + 1);
                    imageWidth = MyConsole.GetLine(x, MyConsole.CursorTop - 1).Length;

                    if (text.Length >= width)
                    {
                        width = text.Length + 2;
                    }
                    MyConsole.SetCursorPosition(x + width / 2 - text.Length / 2, MyConsole.CursorTop + 2);
                    if (MyConsole.CursorLeft < x)
                    {
                        MyConsole.SetCursorPosition(x, MyConsole.CursorTop);
                    }
                    MyConsole.Write(text);
                    if (MyConsole.CursorTop + 2 < y + height)
                    {
                        height = MyConsole.CursorTop + 2 - y;
                    }
                    //if (MyConsole.CursorLeft - x > width)
                    //{
                    //    width = MyConsole.CursorLeft + 2 - x;
                    //}
                    //Box(x, y, x + width, y + height, 2, " ");
                }
            }

            public override void Draw(int screenWidth)
            {
                if (screenWidth > x)
                {
                    Box(x, y, x + width, y + height, 2, " ");
                    MyConsole.SetCursorPosition(x + width / 2 + imageWidth / 2, y + 2);
                    ConsoleWriteImage(image, maxSize, (int)Math.Round(x + (double)width / (double)2 - (double)imageWidth / (double)2, MidpointRounding.AwayFromZero));
                    if (screenWidth > x + width / 2 - text.Length / 2)
                    {
                        MyConsole.SetCursorPosition(x + width / 2 - text.Length / 2, MyConsole.CursorTop + 2);
                        MyConsole.Write(text);
                    }
                }
            }
        }

        private static class MyConsole
        {
            private static List<string> everyThing = new List<string>();
            private static List<List<ConsoleColor>> everythingForeColour = new List<List<ConsoleColor>>();
            private static List<List<ConsoleColor>> everythingBackColour = new List<List<ConsoleColor>>();
            private static int[] position = new int[2];

            private static void RealWrite(string str)
            {
                if (Console.CursorLeft != position[0] || Console.CursorTop != position[1])
                {
                    Console.SetCursorPosition(position[0], position[1]);
                }
                if (str.Length + Console.CursorLeft > Console.BufferWidth)
                {
                    Console.Write(str.Remove((Console.CursorLeft - Console.BufferWidth) * -1));
                }
                else
                {
                    Console.Write(str);
                }
            }

            public static void WriteLine(string str)
            {
                UpdateCursor(str.Length);
                //if (everyThing[position[1]].Length < str.Length + position[0])
                //{
                //    everyThing[position[1]] = everyThing[position[1]].PadRight(position[0] + str.Length);
                //}
                RealWrite(str);
                foreach (char c in str)
                {
                    everyThing[position[1]] = ReplaceAt(everyThing[position[1]], position[0], c);
                    everythingForeColour[position[1]] = ReplaceAt(everythingForeColour[position[1]], position[0], Console.ForegroundColor);
                    everythingBackColour[position[1]] = ReplaceAt(everythingBackColour[position[1]], position[0], Console.BackgroundColor);
                    position[0]++;
                }
                position[1]++;
                position[0] = 0;
                //UpdateCursor();
            }

            public static void WriteLine()
            {
                //UpdateCursor();
                position[1]++;
                position[0] = 0;
                UpdateCursor();
            }

            private static void UpdateCursor()
            {
                if (everyThing.Count < 1)
                {
                    everyThing.Add("");
                }

                if (position[1] > everyThing.Count - 1)
                {
                    int temp = everyThing.Count - 1;
                    for (int i = temp; i < position[1]; i++)
                    {
                        everyThing.Add("");
                    }
                }
                if (position[0] > 0 && everyThing[position[1]].Length - 1 < position[0])
                {
                    everyThing[position[1]] = everyThing[position[1]].PadRight(position[0]);
                }

                if (everythingForeColour.Count < 1)
                {
                    everythingForeColour.Add(new List<ConsoleColor>());
                }

                if (position[1] > everythingForeColour.Count - 1)
                {
                    int temp = everythingForeColour.Count - 1;
                    for (int i = temp; i < position[1]; i++)
                    {
                        everythingForeColour.Add(new List<ConsoleColor>());
                    }
                }
                if (position[0] > 0 && everythingForeColour[position[1]].Count - 1 < position[0])
                {
                    for (int i = everythingForeColour[position[1]].Count; i < position[0]; i++)
                    {
                        everythingForeColour[position[1]].Add(ConsoleColor.White);
                    }
                }

                if (everythingBackColour.Count < 1)
                {
                    everythingBackColour.Add(new List<ConsoleColor>());
                }

                if (position[1] > everythingBackColour.Count - 1)
                {
                    int temp = everythingBackColour.Count - 1;
                    for (int i = temp; i < position[1]; i++)
                    {
                        everythingBackColour.Add(new List<ConsoleColor>());
                    }
                }
                if (position[0] > 0 && everythingBackColour[position[1]].Count - 1 < position[0])
                {
                    for (int i = everythingBackColour[position[1]].Count; i < position[0]; i++)
                    {
                        everythingBackColour[position[1]].Add(ConsoleColor.Black);
                    }
                }
            }

            private static void UpdateCursor(int offset)
            {
                if (everyThing.Count < 1)
                {
                    everyThing.Add("");
                }

                if (position[1] > everyThing.Count - 1)
                {
                    int temp = everyThing.Count - 1;
                    for (int i = temp; i < position[1]; i++)
                    {
                        everyThing.Add("");
                    }
                }
                if (position[0] > 0 && everyThing[position[1]].Length - 1 < position[0])
                {
                    everyThing[position[1]] = everyThing[position[1]].PadRight(position[0]);
                }
                if (everyThing[position[1]].Length < offset + position[0])
                {
                    everyThing[position[1]] = everyThing[position[1]].PadRight(position[0] + offset);
                }

                if (everythingForeColour.Count < 1)
                {
                    everythingForeColour.Add(new List<ConsoleColor>());
                }

                if (position[1] > everythingForeColour.Count - 1)
                {
                    int temp = everythingForeColour.Count - 1;
                    for (int i = temp; i < position[1]; i++)
                    {
                        everythingForeColour.Add(new List<ConsoleColor>());
                    }
                }
                if (position[0] > 0 && everythingForeColour[position[1]].Count - 1 < position[0])
                {
                    for (int i = everythingForeColour[position[1]].Count; i < position[0]; i++)
                    {
                        everythingForeColour[position[1]].Add(ConsoleColor.White);
                    }
                }
                if (everythingForeColour[position[1]].Count < offset + position[0])
                {
                    for (int i = everythingForeColour[position[1]].Count; i < position[0] + offset; i++)
                    {
                        everythingForeColour[position[1]].Add(ConsoleColor.White);
                    }
                }

                if (everythingBackColour.Count < 1)
                {
                    everythingBackColour.Add(new List<ConsoleColor>());
                }

                if (position[1] > everythingBackColour.Count - 1)
                {
                    int temp = everythingBackColour.Count - 1;
                    for (int i = temp; i < position[1]; i++)
                    {
                        everythingBackColour.Add(new List<ConsoleColor>());
                    }
                }
                if (position[0] > 0 && everythingBackColour[position[1]].Count - 1 < position[0])
                {
                    for (int i = everythingBackColour[position[1]].Count; i < position[0]; i++)
                    {
                        everythingBackColour[position[1]].Add(ConsoleColor.Black);
                    }
                }
                if (everythingBackColour[position[1]].Count < offset + position[0])
                {
                    for (int i = everythingBackColour[position[1]].Count; i < position[0] + offset; i++)
                    {
                        everythingBackColour[position[1]].Add(ConsoleColor.Black);
                    }
                }
            }

            public static void Write(string str)
            {
                UpdateCursor(str.Length);
                RealWrite(str);
                foreach (char c in str)
                {
                    everyThing[position[1]] = ReplaceAt(everyThing[position[1]], position[0], c);
                    everythingForeColour[position[1]] = ReplaceAt(everythingForeColour[position[1]], position[0], Console.ForegroundColor);
                    everythingBackColour[position[1]] = ReplaceAt(everythingBackColour[position[1]], position[0], Console.BackgroundColor);
                    position[0]++;
                }
                //UpdateCursor();
            }

            public static void Write(char c)
            {
                UpdateCursor(1);
                //if (everyThing[position[1]].Length < 1 + position[0])
                //{
                //    everyThing[position[1]] = everyThing[position[1]].PadRight(position[0] + 1);
                //}
                RealWrite(c.ToString());
                everyThing[position[1]] = ReplaceAt(everyThing[position[1]], position[0], c);
                everythingForeColour[position[1]] = ReplaceAt(everythingForeColour[position[1]], position[0], Console.ForegroundColor);
                everythingBackColour[position[1]] = ReplaceAt(everythingBackColour[position[1]], position[0], Console.BackgroundColor);
                position[0]++;
                //UpdateCursor();
            }

            public static void SetCursorPosition(int x, int y)
            {
                position[0] = x;
                position[1] = y;
                //UpdateCursor();
            }

            public static void ReDraw()
            {
                Console.Clear();
                int x = Console.CursorLeft;
                int y = Console.CursorTop;
                //string sendString = string.Empty;
                for (int i = 0; i < everyThing.Count; i++)
                {
                    if (everyThing[i].Trim() != string.Empty)
                    {
                        Console.SetCursorPosition(0, i);
                        if (everyThing[i].Length > Console.BufferWidth)
                        {
                            //Console.WriteLine(everyThing[i].Remove(Console.BufferWidth));
                            string str = everyThing[i].Remove(Console.BufferWidth);
                            for (int a = 0; a < str.Length; a++)
                            {
                                Console.BackgroundColor = everythingBackColour[i][a];
                                Console.ForegroundColor = everythingForeColour[i][a];
                                Console.Write(str[a]);
                            }
                            //sendString += everyThing[i].Remove(Console.BufferWidth);
                        }
                        //else if (everyThing[i].Length + 1 > Console.BufferWidth)
                        //{
                        //    //sendString += everyThing[i];
                        //    string str = everyThing[i];
                        //    for (int a = 0; a < str.Length; a++)
                        //    {
                        //        Console.BackgroundColor = everythingBackColour[i][a];
                        //        Console.ForegroundColor = everythingForeColour[i][a];
                        //        Console.Write(str[a]);
                        //    }
                        //}
                        else
                        {
                            //Console.Write(everyThing[i]);
                            //sendString += everyThing[i] + "\n";
                            string str = everyThing[i];
                            for (int a = 0; a < str.Length; a++)
                            {
                                Console.BackgroundColor = everythingBackColour[i][a];
                                Console.ForegroundColor = everythingForeColour[i][a];
                                Console.Write(str[a]);
                            }
                        }
                    }
                    else
                    {
                        //Console.WriteLine();
                        //sendString += "\n";
                    }
                }
                Console.SetCursorPosition(0, 0);
                //Console.Write(sendString);
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.SetCursorPosition(x, y);
            }

            private static string ReplaceAt(string input, int index, char newChar)
            {
                if (input == null)
                {
                    throw new ArgumentNullException("input");
                }
                char[] chars = input.ToCharArray();
                chars[index] = newChar;
                return new string(chars);
            }

            private static List<ConsoleColor> ReplaceAt(List<ConsoleColor> input, int index, ConsoleColor newCol)
            {
                if (input == null)
                {
                    throw new ArgumentNullException("input");
                }
                input[index] = newCol;
                return input;
            }

            public static int CursorTop
            {
                get
                {
                    return position[1];
                }
            }

            public static int CursorLeft
            {
                get
                {
                    return position[0];
                }
            }

            public static void Clear()
            {
                everyThing.Clear();
                everythingForeColour.Clear();
                everythingBackColour.Clear();
                position = new int[2];
                Console.Clear();
            }

            public static string GetString()
            {
                string SendString = string.Empty;
                for (int i = 0; i < everyThing.Count; i++)
                {
                    SendString += everyThing[i] + "\n";
                }
                return SendString;
            }

            public static string[] GetStringArray()
            {
                return everyThing.ToArray();
            }

            public static char GetChar(int x, int y)
            {
                return everyThing[y][x];
            }

            public static ConsoleColor GetForegroundColor(int x, int y)
            {
                return everythingForeColour[y][x];
            }

            public static ConsoleColor GetBackgroundColor(int x, int y)
            {
                return everythingBackColour[y][x];
            }

            public static string GetLine(int x, int y)
            {
                UpdateCursor();
                return everyThing[y].Substring(x);
            }
        }
    }
}