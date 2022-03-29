using System;
using System.Collections.Generic;
using System.Text;

namespace 控制台游戏
{
    class Player
    {
        public Pos pos;
        public Bullet bullet = new Bullet();


        public void SetPos(int h, int w)
        {
            pos.h = h;
            pos.w = w;
            UpdateBullet();
        }

        public void UpdateBullet()
        {
            bullet.launch = false;
            bullet.SetPos(pos.h, pos.w + 1);
        }
    }


    class Bullet
    {
        public Pos pos;
        public bool launch = false;
        public int counter = 1;

        public void SetPos(int h, int w)
        {
            pos.h = h;
            pos.w = w;
        }

        public void Launch(Pos p, bool launch = true)
        {
            this.pos = p;
            this.launch = launch;
        }
    }

    class Enemy
    {
        public Pos pos;
        public bool move = false;
        public bool back = true;
        public List<Pos> route = new List<Pos>();
        public int moveCounter = 1;
        public int bullets = 1;

        /*public static int[,] route_toRightPort = new int[,]
                                           {{0,1 },{0,1 },{0,1 },{0,1 },{1,1 },{1,1 },{1,1},
                                            {1,1 },{1,0 },{1,0 },{1,0 },{1,-1 },{1,-1 },{1,-1 } };*/

        public static int[,] route_rightToLeft = new int[,]
                                           {{1,-1 },{1,-1 },{0,-2 },{1,-1 },{0,-2 },{0,-2 },
                                            {1,-1 },{0,-2 },{0,-2 },{1,-1 },{0,-2 },{0,-2 },
                                            {1,-1 },{0,-2 },{0,-2 },{1,-1 },{0,-2 },{0,-2 },
                                            {0,-2 },{1,-1 },{0,-2 },{0,-2 },{0,-2 },{1,-1 },
                                            {0,-2 },{0,-2 },{1,-1 },{0,-2 },{0,-1 },{1,-1 },
                                            {0,-2 },{1,-1 },{0,-1 },{1,-1 },{0,-1 },{1,-1 },
                                            {1,-1 },{1,0 },{1,0 },{1,1 },{1,1 }, };

        public static int[,] route_rightToMid = new int[,]
                                           {{1,-1 },{1,-1 },{1,-1 },{0,-2 },{1,-1 },{0,-2 },
                                            {1,-1 },{0,-2 },{1,-1 },{0,-2 },{1,-1 },{0,-2 },
                                            {1,-1 },{0,-2 },{1,-1 },{0,-2 },{1,-1 },{0,-2 },
                                            {1,-1 },{0,-1 },{1,-1 },{0,-1 },{1,-1 },{0,-1 },
                                            {1,-1 },{0,-1 },{1,-1 },{0,-1 },{1,-1 },{1,-1 },
                                            {1,0 },{1,1 } };

        public static int[,] route_rightToRight = new int[,] 
                                            {{1,-1 },{1,-1 },{1,-1 },{1,-2 },{1,-1 },{1,-1 },
                                             {1,-2 },{1,-1 },{1,-2 },{1,-1 },{1,-2 },{1,-1 },
                                             {1,-1 },{1,-1 },{1,0 },{1,0 },{1,1 },{1,1 },{1,1 }, };

        public static int[,] route_leftToRight = new int[,]
                                           {{1,+1 },{1,+1 },{0,+2 },{1,+1 },{0,+2 },{0,+2 },
                                            {1,+1 },{0,+2 },{0,+2 },{1,+1 },{0,+2 },{0,+2 },
                                            {1,+1 },{0,+2 },{0,+2 },{1,+1 },{0,+2 },{0,+2 },
                                            {0,+2 },{1,+1 },{0,+2 },{0,+2 },{0,+2 },{1,+1 },
                                            {0,+2 },{0,+2 },{1,+1 },{0,+2 },{0,+1 },{1,+1 },
                                            {0,+2 },{1,+1 },{0,+1 },{1,+1 },{0,+1 },{1,+1 },
                                            {1,+1 },{1,0 },{1,0 },{1,-1 },{1,-1 }, };

        public static int[,] route_leftToMid = new int[,]
                                           {{1,+1 },{1,+1 },{1,+1 },{0,+2 },{1,+1 },{0,+2 },
                                            {1,+1 },{0,+2 },{1,+1 },{0,+2 },{1,+1 },{0,+2 },
                                            {1,+1 },{0,+2 },{1,+1 },{0,+2 },{1,+1 },{0,+2 },
                                            {1,+1 },{0,+1 },{1,+1 },{0,+1 },{1,+1 },{0,+1 },
                                            {1,+1 },{0,+1 },{1,+1 },{0,+1 },{1,+1 },{1,+1 },
                                            {1,0 },{1,-1 } };

        public static int[,] route_leftToLeft = new int[,]
                                            {{1,+1 },{1,+1 },{1,+1 },{1,+2 },{1,+1 },{1,+1 },
                                             {1,+2 },{1,+1 },{1,+2 },{1,+1 },{1,+2 },{1,+1 },
                                             {1,+1 },{1,+1 },{1,0 },{1,0 },{1,-1 },{1,-1 },{1,-1 }, };




        public static int[,] route_rightPurple1 = new int[,]
                                            {{1,-1 },{0,-1 },{0,-1 },{0,-1 },{0,-2 },
                                             {0,-2 },{0,-2 },{0,-2 },{0,-2 },{0,-2 },{0,-2 },
                                             {0,-2 },{0,-2 },{0,-2 },{0,-2 },{0,-2 },{0,-2 },
                                             {0,-2 },{0,-2 },{0,-2 },{0,-2 },{0,-2 },{0,-2 },
                                             {0,-2 },{0,-2 },{0,-2 },{0,-2 },{0,-2 },{0,-1 },
                                             {1,-2},{1,-2},{1,-2},{1,-2 },{1,-2 },{1,-1 },{1,1 },
                                             {1,2 },{1,3 },{1,2 },{1,3 },{1,2 },{1,2 },{1,1 },
                                             {1,0 },{1,-1 },{1,-2 },{1,-2 }, };

        public static int[,] route_leftPurple1 = new int[,]
                                             {{1,+1 },{0,+1 },{0,+1 },{0,+1 },{0,+2 },
                                             {0,+2 },{0,+2 },{0,+2 },{0,+2 },{0,+2 },{0,+2 },
                                             {0,+2 },{0,+2 },{0,+2 },{0,+2 },{0,+2 },{0,+2 },
                                             {0,+2 },{0,+2 },{0,+2 },{0,+2 },{0,+2 },{0,+2 },
                                             {0,+2 },{0,+2 },{0,+2 },{0,+2 },{0,+2 },{0,+1 },
                                             {1,+2},{1,+2},{1,+2},{1,+2 },{1,+2 },{1,+1 },{1,-1 },
                                             {1,-2 },{1,-3 },{1,-2 },{1,-3 },{1,-2 },{1,-2 },{1,-1 },
                                             {1,0 },{1,+1 },{1,+2 },{1,+2 }, };

        public Enemy(int h, int w)
        {
            pos.h = h;
            pos.w = w;
        }

        public Enemy(Enemy enemy)
        {
            pos = enemy.pos;
        }

        public void SetPos(int h, int w)
        {
            pos.h = h;
            pos.w = w;
        }

        public void SetPos(Pos pos)
        {
            this.pos = pos;
        }

        #region
        /*
        // 给出下一步的 加值
        // 斜率k 由 函数 w(h) 得到
        public Pos GetNextPos(int h, int w)
        {
            int degree = 25;    // < 45
            double sin225 = Math.Sin(degree * Math.PI / 180);
            double sin675 = Math.Sin((90 - degree) * Math.PI / 180);

            Pos output;
            double k;
            if (h == 0)
                k = 10000.0d;
            else
                k = w / h;
            if(k >= -sin675 && k < -sin225)
            {
                output = new Pos(1, -2);
                if (h <= 0)
                    output = new Pos(-1, 2);
            }
            else if(k>=-sin225 && k < sin225)
            {
                output = new Pos(1, 0);
                if (h <= 0)
                    output = new Pos(-1, 0);
            }
            else if(k>= sin225 && k < sin675)
            {
                output = new Pos(1, 2);
                if (h <= 0)
                    output = new Pos(-1, -2);
            }
            else if(w >= 0)
            {
                output = new Pos(0, 2);
            }
            else
            {
                output = new Pos(0, -2);
            }
            return output;
        }
        */
        #endregion

        public virtual void SetRoute(Pos dest) { }
        public virtual void SetRightRoute(Pos dest, int choice) { }
        public virtual void SetLeftRoute(Pos dest, int choice) { }
        public virtual void GoBack(Pos dest) { }
        public virtual Bullet Shoot(Pos p) { return null; }
        public virtual Bullet Shoot(int h, int w) { return null; }
    }

    class GreenEnemy : Enemy
    {

        public GreenEnemy(int h, int w) : base(h, w) { }

        
        public override void SetRightRoute(Pos port,int routeSelected)    // route 0 - left, 1 - mid. 2 - right
        {
            move = true;
            back = false;
            bullets = 1;

            int curh = pos.h;
            int curw = pos.w;

            // to port
            while (port.w > curw)
            {
                if (port.w - curw > 5)
                    curw +=2 ;
                curw++;
                route.Add(new Pos(curh, curw));
            }
            while(port.h > curh)
            {
                curh++;
                route.Add(new Pos(curh, curw));
            }

            // to player
            int[,] r = null;
            switch (routeSelected)
            {
                case 0:
                    r = route_rightToLeft;
                    break;
                case 1:
                    r = route_rightToMid;
                    break;
                case 2:
                    r = route_rightToRight;
                    break;
                default:
                    break;
            }
            for (int i = 0; i < r.GetLength(0); i++)
            {
                curh += r[i, 0];
                curw += r[i, 1];
                route.Add(new Pos(curh, curw));
            }
        }

        public override void SetLeftRoute(Pos port, int routeSelected)    // route 0 - left, 1 - mid. 2 - right
        {
            move = true;
            back = false;
            bullets = 1;

            int curh = pos.h;
            int curw = pos.w;

            // to port
            while (port.w < curw)
            {
                if (port.w - curw < 5)
                    curw -= 2;
                curw--;
                route.Add(new Pos(curh, curw));
            }
            while (port.h > curh)
            {
                curh++;
                route.Add(new Pos(curh, curw));
            }

            // to player
            int[,] r = null;
            switch (routeSelected)
            {
                case 0:
                    r = route_leftToLeft;
                    break;
                case 1:
                    r = route_leftToMid;
                    break;
                case 2:
                    r = route_leftToRight;
                    break;
                default:
                    break;
            }
            for (int i = 0; i < r.GetLength(0); i++)
            {
                curh += r[i, 0];
                curw += r[i, 1];
                route.Add(new Pos(curh, curw));
            }
        }


        #region
        /*
        public override void SetLeftRoute(Pos dest)
        {
            move = true;
            back = false;

            // 出港 手动
            for (int i = 1; i < 8; i++)
            {
                route.Add(new Pos(pos.h, pos.w - i));
            }
            for (int i = 1; i < 10; i++)
            {
                route.Add(new Pos(pos.h + i, pos.w - 7));
            }

            int sh = pos.h + 9;
            int sw = pos.w - 7;
            int eh = dest.h + 3;     // 可能有问题
            int ew = dest.w + RandomHelper.GetRandom(0, 12) - 6;
            
            while (sh <= eh && sw <= ew)
            {
                 Pos nextPos = GetNextPos(eh - sh, ew - sw);
                 sh += nextPos.h;
                 sw += nextPos.w;
                route.Add(new Pos(sh, sw));
            }
        }*/
        #endregion

        public override void GoBack(Pos dest)
        {
            if(dest.w > pos.w)
            {
                if (dest.w - pos.w > 10)
                    pos.w += 2;
                pos.w++;
            }
            else if(dest.w < pos.w)
            {
                if (dest.w - pos.w < -10)
                    pos.w -= 2;
                pos.w--;
            }
            if(dest.w == pos.w)
            {
                if (dest.h > pos.h)
                {
                    pos.h++;
                }
            }
            
            if(pos.w == dest.w && pos.h == dest.h)
            {
                back = true;
            }
        }

        public override Bullet Shoot(Pos p)
        {
            Bullet b = new Bullet();
            b.SetPos(p.h, p.w);
            return b;
        }

        public override Bullet Shoot(int h, int w)
        {
            Bullet b = new Bullet();
            b.SetPos(h, w);
            return b;
        }

    }

    class PurpleEnemy : Enemy
    {
        public PurpleEnemy(int h, int w) : base(h, w) { }

        public override void SetRightRoute(Pos port, int routeSelected)    // route 0 - left, 1 - mid. 2 - right
        {
            move = true;
            back = false;
            bullets = 3;

            int curh = pos.h;
            int curw = pos.w;

            // to port
            while (port.w > curw)
            {
                if (port.w - curw > 5)
                    curw += 2;
                curw++;
                route.Add(new Pos(curh, curw));
            }
            while (port.h > curh)
            {
                curh++;
                route.Add(new Pos(curh, curw));
            }

            // to player
            int[,] r = null;
            /*
            switch (routeSelected)
            {
                case 0:
                    r = route_rightToLeft;
                    break;
                case 1:
                    r = route_rightToMid;
                    break;
                case 2:
                    r = route_rightToRight;
                    break;
                default:
                    break;
            }*/
            r = route_rightPurple1;
            for (int i = 0; i < r.GetLength(0); i++)
            {
                curh += r[i, 0];
                curw += r[i, 1];
                route.Add(new Pos(curh, curw));
            }
        }

        public override void SetLeftRoute(Pos port, int routeSelected)    // route 0 - left, 1 - mid. 2 - right
        {
            move = true;
            back = false;
            bullets = 3;

            int curh = pos.h;
            int curw = pos.w;

            // to port
            while (port.w < curw)
            {
                if (port.w - curw < 5)
                    curw -= 2;
                curw--;
                route.Add(new Pos(curh, curw));
            }
            while (port.h > curh)
            {
                curh++;
                route.Add(new Pos(curh, curw));
            }

            // to player
            int[,] r = null;
            /*
            switch (routeSelected)
            {
                case 0:
                    r = route_leftToLeft;
                    break;
                case 1:
                    r = route_leftToMid;
                    break;
                case 2:
                    r = route_leftToRight;
                    break;
                default:
                    break;
            }
            */
            r = route_leftPurple1;
            for (int i = 0; i < r.GetLength(0); i++)
            {
                curh += r[i, 0];
                curw += r[i, 1];
                route.Add(new Pos(curh, curw));
            }
        }

        public override void GoBack(Pos dest)
        {
            if (dest.w > pos.w)
            {
                if (dest.w - pos.w > 10)
                    pos.w += 2;
                pos.w++;
            }
            else if (dest.w < pos.w)
            {
                if (dest.w - pos.w < -10)
                    pos.w -= 2;
                pos.w--;
            }
            if (dest.w == pos.w)
            {
                if (dest.h > pos.h)
                {
                    pos.h++;
                }
            }

            if (pos.w == dest.w && pos.h == dest.h)
            {
                back = true;
            }
        }

        public override Bullet Shoot(Pos p)
        {
            Bullet b = new Bullet();
            b.SetPos(p.h, p.w);
            return b;
        }

        public override Bullet Shoot(int h, int w)
        {
            Bullet b = new Bullet();
            b.SetPos(h, w);
            return b;
        }


    }


    class RedEnemy : Enemy
    {
        public RedEnemy(int h, int w) : base(h, w) { }
    }

    class Boss : Enemy
    {
        public Boss(int h, int w) : base(h, w) { }
    }


    class Ruin
    {
        public Pos pos;
        public int time;      // 持续时间
        
        public Ruin(int h, int w, int t = 3)
        {
            pos.h = h;
            pos.w = w;
            time = t;
        }
    }
}
