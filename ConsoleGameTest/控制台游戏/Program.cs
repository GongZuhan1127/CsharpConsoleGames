using System;
using System.Collections.Generic;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace 控制台游戏
{
    struct Pos
    {
        public int h;
        public int w;

        public Pos(int h, int w)
        {
            this.h = h;
            this.w = w;
        }

    }


    class Program
    {
        // 游戏流程
        static bool skipSS = false;        // skip the start screen
        static bool to_start = true;
        static bool gameover = false;

        // 音乐相关
        /*
        [DllImport("winmm.dll")]
        public static extern bool PlaySound(String Filename, int Mod, int Flags);
        */

        // 画布相关
        static ConsoleCanvas canvas;
        const int height = 45;
        const int width = 75;  //每行长度，y
        static char[,] buffer;
        static ConsoleColor[,] color_buffer;
        const int gh = 41;    // Gamepad Height
        const int gw = 70;    // Gamepad Width 
        static Pos go = new Pos(2, 1);  // Gamepad Offset
        // 所有的图形3X3 ； 右上角是起始点

        // 玩家相关
        public static Pos nextPos;
        public static Player player;

        // 敌人相关
        public static Dictionary<int, Enemy> enemies;
        public static int direction = 1;          // 敌人摆动方向
        public static int counter = 0;            // 敌人摆动等待
        public static int attackCounter = 0;
        public static bool attack = true;
        public static bool attack_purple = false;
        public static bool rightDirect = true;
        public static bool onChangeDirect = false;
        public static Dictionary<int, Enemy> staticEnemy;      // 停泊的敌人
        public static Dictionary<int, Enemy> moveEnemy;        // 进攻的敌人
        //public static int moveCounter = 0;
        //static int[] moveOrder = { 39, 49, 59, 38, 48, 58, 51, 41, 31, 50, 40, 30 };
        static int[] rightGreen = { 39, 49, 59, 38, 48, 58, 37, 47, 57, 36, 46, 56, 35, 45, 55 };
        static int[] leftGreen = { 30, 40, 50, 31, 41, 51, 32, 42, 52, 33, 43, 53, 34, 44, 54 };
        static int[] rightPurple = { 28, 27, 26, 25 };
        static int[] leftPurple = { 21, 22, 23, 24 };

        public static Pos rightPort = new Pos(22, gw - 5);
        public static Pos leftPort = new Pos(22, 5);
        public static List<Bullet> bullets;
        public static List<Ruin> ruins;


        static void Init()
        {
            Console.WindowHeight = height + 2;
            Console.WindowWidth = width;

            canvas = new ConsoleCanvas(height, width);
            buffer = canvas.GetBuffer();
            color_buffer = canvas.GetColorBuffer();

            player = new Player();
            player.SetPos(gh - 3, gw / 2);

            enemies = new Dictionary<int, Enemy>();
            moveEnemy = new Dictionary<int, Enemy>();

            enemies.Add(3, new Boss(3, 24));
            enemies.Add(6, new Boss(3, 42));
            for (int i = 2; i < 8; i++)
            {
                enemies.Add(10 + i, new RedEnemy(6, 6 + 6 * i));
            }
            for (int i = 1; i < 9; i++)
            {
                enemies.Add(20 + i, new PurpleEnemy(9, 6 + 6 * i));
            }
            for (int j = 3; j < 6; j++)
            {
                for (int i = 0; i < 10; i++)
                {
                    enemies.Add(10 * j + i, new GreenEnemy(3 + 3 * j, 6 + 6 * i));
                }
            }

            staticEnemy = new Dictionary<int, Enemy>(enemies);

            bullets = new List<Bullet>();
            ruins = new List<Ruin>();

        }

        static void Input()
        {

            nextPos = player.pos;

            XKeyboard keyboard = new XKeyboard();

            if (keyboard.IsKeyDown(XKeys.Left))
            {
                nextPos.w--;
            }
            if (keyboard.IsKeyDown(XKeys.Right))
            {
                nextPos.w++;
            }

            if (keyboard.IsKeyDown(XKeys.Space))
            {
                player.bullet.launch = true;
            }

            #region control_unused
            /*
            if (Console.KeyAvailable)                     // 如何连续移动 平缓地？
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.LeftArrow:
                        nextPos.w--;
                        break;
                    case ConsoleKey.RightArrow:
                        nextPos.w++;
                        break;

                    case ConsoleKey.Spacebar:
                        if (!player.bullet.launch)
                            player.bullet.launch = true;
                        break;

                    default:
                        break;
                }
            }*/
            #endregion 

        }


        static void Logic()
        {

            // 玩家 边界 碰撞
            if (nextPos.w < 0 || nextPos.w >= gw - 2)    // 3 - 玩家图像占3位
            {
                nextPos = player.pos;
            }
            player.pos = nextPos;


            // 玩家导弹移动
            if (player.bullet.launch)
            {
                player.bullet.pos.h--;
                if (player.bullet.pos.h <= 3)
                {
                    player.UpdateBullet();
                }
            }
            else
            {
                player.UpdateBullet();
            }


            // 敌人 子弹移动
            List<Bullet> removeBullets = new List<Bullet>();
            foreach (var bullet in bullets)
            {
                if (bullet.counter > 0)
                    bullet.counter--;
                else
                {
                    bullet.counter = 1;
                    if (bullet.pos.h < gh - 2)
                    {
                        bullet.pos.h++;
                    }
                    else
                    {
                        removeBullets.Add(bullet);
                    }
                }
            }
            foreach (var r in removeBullets)
            {
                bullets.Remove(r);
            }
            removeBullets.Clear();


            // 敌人 摆动
            if (counter != 0)
                counter--;
            else
            {
                counter = RandomHelper.GetRandom(0, 100) < 10 ? 36 : 12;
                foreach (var pair in staticEnemy)
                {
                    int curPos = pair.Value.pos.w;

                    if (curPos <= 0 && direction < 0)      // 转向 -> 右
                    {
                        direction = 1;
                        rightDirect = true;
                        onChangeDirect = true;
                        break;
                    }
                    if (curPos > gw - 4 && direction > 0)    // 转向 -> 左
                    {
                        direction = -1;
                        rightDirect = false;
                        onChangeDirect = true;
                        break;
                    }
                    onChangeDirect = false;
                }

                foreach (var pair in staticEnemy)
                {
                    pair.Value.pos.w += direction;
                }
            }

                /*
                // 侦察到玩家没有瞄准任何目标 -> 等待
                bool wait = false;
                foreach (var pair in staticEnemy)
                {
                    int curPos = pair.Value.pos.w;
                    int dangerPos = player.bullet.pos.w;

                    if (!wait)
                    {
                        if ((curPos - 1 == dangerPos || curPos + 3 == dangerPos) && player.bullet.launch && 
                                player.bullet.pos.h < 20)
                        {
                            wait = true;
                            break;
                        }
                    }
                }

                if (!wait)
                {
                    foreach (var pair in staticEnemy)
                    {
                        pair.Value.pos.w += direction;
                    }
                }
            }*/



            // 击中 敌人
            int enemyKey = -1;
            foreach (var pair in enemies)
            {
                int bh = player.bullet.pos.h;
                int bw = player.bullet.pos.w;
                int ph = pair.Value.pos.h;
                int pw = pair.Value.pos.w;
                if (ph <= bh && ph + 2 >= bh && pw <= bw && pw + 2 >= bw && player.bullet.launch == true)
                {
                    enemyKey = pair.Key;
                    ruins.Add(new Ruin(ph, pw));
                    player.UpdateBullet();
                }
            }
            if (enemyKey != -1)
            {
                enemies.Remove(enemyKey);
                staticEnemy.Remove(enemyKey);
                moveEnemy.Remove(enemyKey);
            }

            // 玩家被敌机击中
            foreach (var enemy in enemies)
            {
                int ph = player.pos.h;
                int pw = player.pos.w;
                int eh = enemy.Value.pos.h;
                int ew = enemy.Value.pos.w;
                if((ph - 2 <= eh && eh <= ph + 2) && (pw - 2 <= ew && ew <= pw + 2))
                {
                    enemyKey = enemy.Key;
                    ruins.Add(new Ruin(eh, ew));
                    ruins.Add(new Ruin(ph, pw));
                    gameover = true;
                    break;
                }
            }
            if (enemyKey != -1)
            {
                enemies.Remove(enemyKey);
                staticEnemy.Remove(enemyKey);
                moveEnemy.Remove(enemyKey);
            }

            // 玩家被子弹击中
            foreach (var bullet in bullets)
            {
                int ph = player.pos.h;
                int pw = player.pos.w;
                int bh = bullet.pos.h;
                int bw = bullet.pos.w;
                if( (ph <= bh && bh <= ph + 2) && (pw - 2 <= bw && bw <= pw))
                {
                    ruins.Add(new Ruin(ph, pw));
                    gameover = true;
                    break;
                }
            }


            // 废墟 消散
            for (int i = ruins.Count - 1; i >= 0; i--)
            {
                ruins[i].time--;
                if (ruins[i].time <= 0)
                    ruins.RemoveAt(i);
            }

            // 测试敌人移动
            if (!attack)
            {
                attack = true;
                attackCounter = 180;
                if (staticEnemy.Count <= 36 && staticEnemy.Count > 22)
                    attackCounter = 108;
                else if (staticEnemy.Count <= 22)
                {
                    attackCounter = (RandomHelper.GetRandom(0, 100)) < 10 ? 0 : 72;
                    attack_purple = true;
                }
            }
            if (attackCounter > 0)
            {
                attackCounter--;
            }
            if (attackCounter <= 0)
            {
                // Purple attack
                for (int i = 0; i < rightPurple.GetLength(0); i++)
                {
                    int id = rightPurple[i];
                    if (attack_purple && staticEnemy.ContainsKey(id))    // 判断有无
                    {
                        if (!staticEnemy[id].move)
                        {
                            if (staticEnemy[id].pos.w - rightPort.w < -5)
                            {
                                staticEnemy[id].SetRightRoute(rightPort, (player.pos.w / (gw / 3)));
                                moveEnemy.Add(id, staticEnemy[id]);
                                staticEnemy[id] = new Enemy(staticEnemy[id]);
                                staticEnemy[id].move = true;
                                //attackCounter += 144;
                                attack_purple = false;
                                break;
                            }
                            break;
                        }
                    }
                }

                for (int i = 0; i < leftPurple.GetLength(0); i++)
                {
                    int id = leftPurple[i];
                    if (attack_purple && staticEnemy.ContainsKey(id))       // 判断有无
                    {
                        if (!staticEnemy[id].move)
                        {
                            if (staticEnemy[id].pos.w - leftPort.w > 5)
                            {
                                staticEnemy[id].SetLeftRoute(leftPort, (player.pos.w / (gw / 3)));
                                moveEnemy.Add(id, staticEnemy[id]);
                                staticEnemy[id] = new Enemy(staticEnemy[id]);
                                staticEnemy[id].move = true;
                                //attackCounter += 144;
                                attack_purple = false;
                                break;
                            }
                            break;
                        }
                    }
                }

                // Green attack
                for (int i = 0; i < rightGreen.GetLength(0); i++)
                {
                    int id = rightGreen[i];
                    if (attack && staticEnemy.ContainsKey(id))    // 判断有无
                    {
                        if (!staticEnemy[id].move)
                        {
                            if (staticEnemy[id].pos.w - rightPort.w < -5)
                            {
                                staticEnemy[id].SetRightRoute(rightPort, (player.pos.w / (gw / 3)));
                                moveEnemy.Add(id, staticEnemy[id]);
                                staticEnemy[id] = new Enemy(staticEnemy[id]);
                                staticEnemy[id].move = true;
                                //attackCounter += 144;
                                attack = false;
                                break;
                            }
                            break;
                        }
                    }
                }

                for (int i = 0; i < leftGreen.GetLength(0); i++)
                {
                    int id = leftGreen[i];
                    if (attack && staticEnemy.ContainsKey(id))       // 判断有无
                    {
                        if (!staticEnemy[id].move)
                        {
                            if (staticEnemy[id].pos.w - leftPort.w > 5)
                            {
                                staticEnemy[id].SetLeftRoute(leftPort, (player.pos.w / (gw / 3)));
                                moveEnemy.Add(id, staticEnemy[id]);
                                staticEnemy[id] = new Enemy(staticEnemy[id]);
                                staticEnemy[id].move = true;
                                //attackCounter += 144;
                                attack = false;
                                break;
                            }
                            break;
                        }
                    }
                }

                #region
                /*
                if (onChangeDirect)
                {
                    if (rightDirect)
                    {
                        for (int i = 0; i < onChangeOrder_Right.Length; i++)
                        {
                            int moveIndex = onChangeOrder_Right[i];
                            if (staticEnemy.ContainsKey(moveIndex))   // 判断有没有
                            {
                                if (!staticEnemy[moveIndex].move)     // 判断动不动
                                {
                                    staticEnemy[moveIndex].SetRightRoute(player.pos);
                                    moveEnemy.Add(moveIndex, staticEnemy[moveIndex]);
                                    staticEnemy[moveIndex] = new Enemy(staticEnemy[moveIndex]);       // 新建 Enemy, 保留位置信息，方便找回
                                    staticEnemy[moveIndex].move = true;
                                    attackCounter = 100;
                                    onChangeDirect = false;
                                    break;
                                }
                            }
                        }
                    }
                }*/
                #endregion

            }

            // 刷新 敌人移动
            List<int> removeArray = new List<int>();
            foreach (var enemy in moveEnemy)
            {
                if (enemy.Value.move)
                {
                    if (enemy.Value.moveCounter > 0)
                        enemy.Value.moveCounter--;
                    else
                    {
                        enemy.Value.moveCounter = 1;
                        if (enemy.Value.route.Count != 0)      // 行进中
                        {
                            Pos nexPos = enemy.Value.route[0];
                            enemy.Value.route.RemoveAt(0);

                            // 加入子弹
                            if (enemy.Value.bullets >= 1)
                            {
                                if (enemy.Value.pos.w >= player.pos.w - 1 && enemy.Value.pos.w <= player.pos.w + 3)
                                {
                                    enemy.Value.bullets--;
                                    bullets.Add(enemy.Value.Shoot(enemy.Value.pos.h + 3, enemy.Value.pos.w + 1));
                                }
                            }

                            if (enemy.Value.route.Count == 0)
                            {
                                nexPos.h = ((nexPos.h % gh) + gh) % gh;
                                nexPos.w = ((nexPos.w % gw) + gw) % gw;
                            }
                            enemy.Value.SetPos(nexPos);
                        }
                        else if (!enemy.Value.back)   // 还没回来
                        {
                            enemy.Value.GoBack(staticEnemy[enemy.Key].pos);
                        }
                        else          // 回来了
                        {
                            enemy.Value.move = false;
                            removeArray.Add(enemy.Key);
                            enemy.Value.pos = staticEnemy[enemy.Key].pos;
                            staticEnemy[enemy.Key] = enemy.Value;

                        }
                    }
                }
            }
            foreach (int key in removeArray)
            {
                moveEnemy.Remove(key);
            }
            removeArray.Clear();

            // 打光了， 游戏结束
            if(staticEnemy.Count == 0 && moveEnemy.Count == 0)
            {
                gameover = true;
            }
        }

        static void Refresh()
        {
            canvas.ClearBuffer_DoubleBuffer();
            DrawBorder();
            DrawGamepad();


            canvas.Refresh_DoubleBuffer();
        }


        static void DrawBorder()
        {
            for (int i = go.w - 1; i <= gw + go.w; i++)
            {
                buffer[go.h - 1, i] = '-';   //
                buffer[gh + go.h, i] = '-';   //
            }
            for (int i = go.h; i < gh + go.h; i++)
            {
                buffer[i, go.w - 1] = '|';
                buffer[i, gw + go.w] = '|';
            }
        }



        static void DrawGamepad()
        {
            if(!gameover)
                DrawPlayer();
            DrawEnemy();
            DrawEnemyBullets();
            DrawRuins();

        }


        static void DrawPlayer()
        {
            int sh = player.pos.h + go.h;
            int sw = player.pos.w + go.w;
            buffer[sh, sw] = ' '; buffer[sh, sw + 1] = ' '; buffer[sh, sw + 2] = ' ';
            buffer[sh + 1, sw] = ' '; buffer[sh + 1, sw + 1] = '='; buffer[sh + 1, sw + 2] = ' ';
            buffer[sh + 2, sw] = '/'; buffer[sh + 2, sw + 1] = '='; buffer[sh + 2, sw + 2] = '\\';
            color_buffer[sh + 1, sw + 1] = ConsoleColor.Red;
            color_buffer[sh + 2, sw + 1] = ConsoleColor.Blue;
            buffer[player.bullet.pos.h + go.h, player.bullet.pos.w + go.w] = '|';
            color_buffer[player.bullet.pos.h + go.h, player.bullet.pos.w + go.w] = ConsoleColor.Red;
        }


        static void DrawEnemy()
        {
            foreach (var pair in enemies)
            {
                // value 可为负数
                // result = ((value % m) + m) % m
                int sh0 = (pair.Value.pos.h % gh + gh) % gh + go.h;
                int sh1 = ((pair.Value.pos.h + 1) % gh + gh) % gh + go.h;
                int sh2 = ((pair.Value.pos.h + 2) % gh + gh) % gh + go.h;
                int sw0 = (pair.Value.pos.w % gw + gw) % gw + go.w;
                int sw1 = ((pair.Value.pos.w + 1) % gw + gw) % gw + go.w;
                int sw2 = ((pair.Value.pos.w + 2) % gw + gw) % gw + go.w;

                if (pair.Value is GreenEnemy)
                {
                    buffer[sh0, sw0] = ' '; buffer[sh0, sw1] = '_'; buffer[sh0, sw2] = ' ';
                    buffer[sh1, sw0] = '>'; buffer[sh1, sw1] = '='; buffer[sh1, sw2] = '<';
                    buffer[sh2, sw0] = ' '; buffer[sh2, sw1] = '^'; buffer[sh2, sw2] = ' ';
                    color_buffer[sh1, sw0] = ConsoleColor.Blue;
                    color_buffer[sh1, sw2] = ConsoleColor.Blue;
                    color_buffer[sh0, sw1] = ConsoleColor.Green;
                    color_buffer[sh1, sw1] = ConsoleColor.Green;
                    color_buffer[sh2, sw1] = ConsoleColor.Green;
                }
                if (pair.Value is PurpleEnemy)
                {
                    buffer[sh0, sw0] = ' '; buffer[sh0, sw1] = '_'; buffer[sh0, sw2] = ' ';
                    buffer[sh1, sw0] = '>'; buffer[sh1, sw1] = '='; buffer[sh1, sw2] = '<';
                    buffer[sh2, sw0] = ' '; buffer[sh2, sw1] = '^'; buffer[sh2, sw2] = ' ';
                    color_buffer[sh1, sw0] = ConsoleColor.Blue;
                    color_buffer[sh1, sw2] = ConsoleColor.Blue;
                    color_buffer[sh0, sw1] = ConsoleColor.DarkMagenta;
                    color_buffer[sh1, sw1] = ConsoleColor.DarkMagenta;
                    color_buffer[sh2, sw1] = ConsoleColor.DarkMagenta;
                }
                if (pair.Value is RedEnemy)
                {
                    buffer[sh0, sw0] = ' '; buffer[sh0, sw1] = '_'; buffer[sh0, sw2] = ' ';
                    buffer[sh1, sw0] = '>'; buffer[sh1, sw1] = '='; buffer[sh1, sw2] = '<';
                    buffer[sh2, sw0] = ' '; buffer[sh2, sw1] = '^'; buffer[sh2, sw2] = ' ';
                    color_buffer[sh1, sw0] = ConsoleColor.Blue;
                    color_buffer[sh1, sw2] = ConsoleColor.Blue;
                    color_buffer[sh0, sw1] = ConsoleColor.Red;
                    color_buffer[sh1, sw1] = ConsoleColor.Red;
                    color_buffer[sh2, sw1] = ConsoleColor.Red;
                }
                if (pair.Value is Boss)
                {
                    buffer[sh0, sw0] = ' '; buffer[sh0, sw1] = '_'; buffer[sh0, sw2] = ' ';
                    buffer[sh1, sw0] = '\\'; buffer[sh1, sw1] = '='; buffer[sh1, sw2] = '/';
                    buffer[sh2, sw0] = '\\'; buffer[sh2, sw1] = '|'; buffer[sh2, sw2] = '/';
                    color_buffer[sh1, sw0] = ConsoleColor.Blue;
                    color_buffer[sh1, sw2] = ConsoleColor.Blue;
                    color_buffer[sh2, sw0] = ConsoleColor.Blue;
                    color_buffer[sh2, sw2] = ConsoleColor.Blue;
                    color_buffer[sh1, sw1] = ConsoleColor.Yellow;
                    color_buffer[sh2, sw1] = ConsoleColor.Yellow;
                    color_buffer[sh0, sw1] = ConsoleColor.Red;
                }
            }
        }


        static void DrawEnemyBullets()
        {
            foreach (var bullet in bullets)
            {
                int sh = bullet.pos.h + go.h;
                int sw = bullet.pos.w + go.w;
                buffer[sh, sw] = '|';
                color_buffer[sh, sw] = ConsoleColor.White;
            }
        }


        static void DrawRuins()
        {
            foreach (var ruin in ruins)
            {
                int sh = ruin.pos.h + go.h;
                int sw = ruin.pos.w + go.w;
                buffer[sh, sw] = '\\'; buffer[sh, sw + 1] = '|'; buffer[sh, sw + 2] = '/';
                buffer[sh + 1, sw] = '-'; buffer[sh + 1, sw + 1] = '*'; buffer[sh + 1, sw + 2] = '-';
                buffer[sh + 2, sw] = '/'; buffer[sh + 2, sw + 1] = '|'; buffer[sh + 2, sw + 2] = '\\';
                color_buffer[sh, sw] = ConsoleColor.Red;
                color_buffer[sh, sw + 2] = ConsoleColor.Red;
                color_buffer[sh + 2, sw] = ConsoleColor.Red;
                color_buffer[sh + 2, sw + 2] = ConsoleColor.Red;
                color_buffer[sh, sw + 1] = ConsoleColor.Blue;
                color_buffer[sh + 1, sw] = ConsoleColor.Blue;
                color_buffer[sh + 2, sw + 1] = ConsoleColor.Blue;
                color_buffer[sh + 1, sw + 2] = ConsoleColor.Blue;
                color_buffer[sh + 1, sw + 1] = ConsoleColor.Yellow;
            }
        }


        static void TimeControl()
        {
            Thread.Sleep(25);
        }


        static void StartScreen()
        {
            Console.CursorVisible = false;

            // GALAXIAN
            Console.SetCursorPosition(0, 4);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("    _---__      -        -        -       -    -    -     -       -     -");
            Console.WriteLine("   -          -  -      -       -  -      -  -     -    -  -     --    - ");
            Console.WriteLine("  -    ____  -___-     -       -___-       -      -    -___-    - -   -  ");
            Console.WriteLine(" -      -   -    -    -       -    -     -  -    -    -    -   -  -  -   ");
            Console.WriteLine("  -____-___-     -___-_______-     -___-    -___-____-     -__-   -_-    ");

            // made by blackrice
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(47, 11);
            Console.Write("-- made by Blackrice\n");

            // start & exit
            Console.SetCursorPosition(35, 19);
            Console.Write("START\n");
            Console.SetCursorPosition(35, 22);
            Console.Write("EXIT\n");

            // select
            Console.ForegroundColor = ConsoleColor.Red;
            int sel_pos = to_start ? 19 : 22;
            Console.SetCursorPosition(30, sel_pos);
            Console.Write(">>");
            Console.SetCursorPosition(30, 41 - sel_pos);
            Console.Write("  ");

            // control tips
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.SetCursorPosition(0, 30);
            Console.WriteLine("                              < - Move Left");
            Console.WriteLine("                              > - Move Right");
            Console.WriteLine("                          Space - Fire");
            Console.WriteLine("                          Enter - Select");

            if (Console.KeyAvailable)
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.UpArrow:
                        to_start = true;
                        break;
                    case ConsoleKey.DownArrow:
                        to_start = false;
                        break;
                    case ConsoleKey.Enter:
                        if (to_start)
                        {
                            skipSS = true;
                            Console.Clear();
                        }
                        else
                            ExitGame();
                        break;
                    default:
                        break;
                }
            }


        }

        static void EndScreen()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Wanna try again?");
            Console.WriteLine("           (Y/N)");

        }

        static void PlayAgain()
        {
            if (Console.KeyAvailable)
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.Y:
                        gameover = false;
                        Console.Clear();
                        break;
                    case ConsoleKey.N:
                        ExitGame();
                        break;
                    default:
                        break;
                }
            }
        }

        static void ExitGame()
        {
            Environment.Exit(0);
        }

        static void Main(string[] args)
        {
            while (!skipSS)
            {
                StartScreen();
            }

            Init();
            while (true)
            {
                switch (gameover)
                {
                    case false:
                        Input();
                        Logic();
                        Refresh();
                        TimeControl();
                        if (gameover)
                            EndScreen();
                        break;

                    case true:
                        PlayAgain();
                        if (!gameover)
                            Init();
                        break;

                    default:
                        break;
                }
            }

        }
    }
}
