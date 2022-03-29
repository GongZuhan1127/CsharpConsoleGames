using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace 控制台游戏
{
    class ConsoleCanvas
    {

        int height;
        int width;
        char empty = ' ';
        char[,] backBuffer;
        char[,] buffer;
        ConsoleColor[,] color_buffer;

        public int Height
        {
            get { return height; }
        }

        public int Width
        {
            get { return width; }
        }

        public ConsoleCanvas(int h, int w, char _empty = ' ')
        {
            width = w;
            height = h;
            empty = _empty;
            buffer = new char[height, width];
            backBuffer = new char[height, width];
            color_buffer = new ConsoleColor[height, width];
            Console.CursorVisible = false;  // 光标不可见
            // 清空缓冲
            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    buffer[i, j] = empty;
                    color_buffer[i, j] = ConsoleColor.Gray;
                }
            }
        }


        void CopyArray2D(char[,] source, char[,] dest)
        {
            for (int i = 0; i < source.GetLength(0); ++i)
            {
                for (int j = 0; j < source.GetLength(1); ++j)
                {
                    dest[i, j] = source[i, j];
                }
            }
        }

        public void ClearBuffer_DoubleBuffer()
        {
            CopyArray2D(buffer, backBuffer);
            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    buffer[i, j] = empty;
                    color_buffer[i, j] = ConsoleColor.Gray;
                }
            }
        }

        public char[,] GetBuffer()
        {
            return buffer;
        }

        public ConsoleColor[,] GetColorBuffer()
        {
            return color_buffer;
        }

        public void Refresh_DoubleBuffer()
        {
            for (int i = 0; i < height; i++)
            {
                // 每行最后要改的地方
                int end = 0;
                for (int j = width - 1; j >= 0; --j)
                {
                    if (buffer[i, j] != empty || backBuffer[i, j] != empty)
                    {
                        end = j + 1;
                        break;
                    }
                }

                for (int j = 0; j < end; j++)
                {
                    if (buffer[i, j] != backBuffer[i, j])
                    {
                        //Console.SetCursorPosition(j * 2, i);

                        Console.SetCursorPosition(j, i);
                        ConsoleColor c = color_buffer[i, j];
                        Console.ForegroundColor = c;
                        Console.Write(buffer[i, j]);
                    }
                }
            }
        }
    }
}
