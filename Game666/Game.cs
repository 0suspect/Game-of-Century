using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace Game666
{
    static class Game
    {
        static Timer timer = new Timer();
        static Timer timer2 = new Timer();
        static public Random rnd = new Random();
        static BaseObject[] objs;
        static Asteroid[] asteroids;
        static AsterCrash asterCrashes;
        static Ship ship; 
        static Bullet bullet;
        static BufferedGraphicsContext context;
        static public BufferedGraphics buffer;

        static public int Width { get; set; }
        static public int Height { get; set; }

        static Game()
        {
        }

        static public void Init(Form form)
        {
            Graphics gr;
            context = BufferedGraphicsManager.Current;
            gr = form.CreateGraphics();
            Width = form.Width;
            Height = form.Height;
            buffer = context.Allocate(gr, new Rectangle(0, 0, Width, Height));
            Load();
            timer.Interval = 20;
            timer.Tick += Timer_tick;
            timer.Start();
            form.KeyDown += Form_KeyDown;
            Ship.MessageDie += Finish;
        }

        private static void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)                
                bullet = new Bullet(new Point(ship.Rect.X + 15, ship.Rect.Y+27), new Point(12, 0), new Size(100, 18));           
            if (e.KeyCode == Keys.Up)            
                ship.Up();            
            if (e.KeyCode == Keys.Down)            
                ship.Down();            
            if (e.KeyCode == Keys.Left)            
                ship.Left();            
            if (e.KeyCode == Keys.Right)            
                ship.Right();            
            if (e.KeyCode == Keys.Up && e.Modifiers == Keys.Left)            
                ship.UpLeft();            
            if (e.KeyCode == Keys.Up && e.Modifiers == Keys.Right)            
                ship.UpRight();            
            if (e.KeyCode == Keys.Down && e.Modifiers == Keys.Left)            
                ship.DownLeft();
            if (e.KeyCode == Keys.Down && e.Modifiers == Keys.Right)            
                ship.DownRight();            
        }

        private static void Timer_tick(object sender, EventArgs e)
        {
            Draw();
            Update();
        }
        
        static public void Load()
        {
            int PlanetSize = rnd.Next(1, 4);
            int AsterSize = rnd.Next(5, 15);
            
            objs = new BaseObject[15];

            for (int i = 0; i < 10; i++)
                objs[i] = new Star(new Point(Width + 50, i*rnd.Next(10, 60)), new Point(2*i*i, 0), new Size(30, 30));
            for (int i = 10; i < 11; i++)
                objs[i] = new Pl1(new Point(Width + 550, rnd.Next(10, 400)), new Point(rnd.Next(1, 2) * i, 0), new Size(10*PlanetSize, 10 * PlanetSize));
            for (int i = 11; i < 12; i++)
                objs[i] = new Pl2(new Point(Width + 850, rnd.Next(100, 500)), new Point(rnd.Next(1, 5) * i , 0), new Size(200, 100));
            for (int i = 12; i < 13; i++)
                objs[i] = new Pl3(new Point(Width + 1550, rnd.Next(120, 580)), new Point(rnd.Next(1, 2) * i , 0), new Size(7 * PlanetSize, 7 * PlanetSize));
            for (int i = 13; i < 14; i++)
                objs[i] = new Pl4(new Point(Width + 450, rnd.Next(100, 600)), new Point(rnd.Next(1, 2) * i, 0), new Size(11 * PlanetSize, 11 * PlanetSize));
            for (int i = 14; i < 15; i++)
                objs[i] = new Pl5(new Point(Width + 200, rnd.Next(120, 500)), new Point(rnd.Next(1, 2) * i, 0), new Size(10 * PlanetSize, 10 * PlanetSize));

            asteroids = new Asteroid[10];
            for (int i = 0; i < asteroids.Length; i++)            
                asteroids[i] = new Asteroid(new Point(Width+1500+i, rnd.Next(120, 550)+i), new Point(rnd.Next(1, 7) + i, 3), new Size(30, 30));

            ship = new Ship(new Point(Width*3/80, Height*2/3), new Point(50, 50), new Size(150, 50));
        }        

        static public void Draw()
        {
           
            Image img = Image.FromFile(@"C:\Users\Белый Господин\source\repos\Game666\Game666\Models\Bg1.jpeg");
            //buffer.Graphics.Clear(Color.Black);
            buffer.Graphics.DrawImage(img,0,0);
            
                

            foreach (BaseObject obj in objs)
                obj.Draw();

            foreach (Asteroid a in asteroids)
                if (a != null)
                    a.Draw();

            if (asterCrashes != null)
                asterCrashes.Draw();

            
            if (bullet != null)
                bullet.Draw();
            



            ship.Draw();

            buffer.Graphics.DrawString("Energy" + ship.Energy, SystemFonts.DefaultFont, Brushes.White, 360, 10);

            buffer.Render();

        }

        static public void Update()
        {
           
            foreach (BaseObject obj in objs)
                obj.Update();           

            if (bullet != null)
                bullet.Update();
            

            for (int i = 0; i < asteroids.Length; i++)
            {
                if (asteroids[i] != null)
                {
                    asteroids[i].Update();
                    if (bullet != null && bullet.Collision(asteroids[i]))
                    {
                        System.Media.SystemSounds.Beep.Play();
                        asterCrashes = new AsterCrash(new Point(asteroids[i].Rect.X, asteroids[i].Rect.Y), new Point(0, 0), new Size(100, 100));
                        timer2.Interval = 1000;
                        timer2.Tick += Timer_tick2;
                        timer2.Start();
                        
                        asteroids[i] = null;                        
                        bullet = null;
                        continue;
                    }
                    if (ship.Collision(asteroids[i]))
                    {
                        ship.EnergyLow(rnd.Next(10, 50));
                        System.Media.SystemSounds.Hand.Play();
                        asterCrashes = new AsterCrash(new Point(asteroids[i].Rect.X, asteroids[i].Rect.Y), new Point(0, 0), new Size(100, 100));
                        timer2.Interval = 1000;
                        timer2.Tick += Timer_tick2;
                        timer2.Start();
                        asteroids[i] = null;
                        
                        if (ship.Energy <= 0)
                            ship.Die();
                    }
                }
            }
        }
        private static void Timer_tick2(object sender, EventArgs e)
        {
            asterCrashes = null;
            timer2.Stop();
        }


        static public void Finish()
        {
            timer.Stop();
            buffer.Graphics.DrawString("WASTED", new Font(FontFamily.GenericSansSerif, 60, FontStyle.Bold), Brushes.Tan, 250, 200);
            buffer.Render();
        }
    }
}
    

