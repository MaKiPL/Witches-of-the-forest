using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using System.Collections.Generic;

namespace WitchesOfTheForest
{
//one class because you should kill all bad people
    public class Game1 : Game
    {
        static GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D forest;
        Texture2D raind;
        static Texture2D superCircle;
        static Random random;
        SpriteFont sf;
        GameTime delta;

        bool bShouldBlockInput = false;

        enum Modules
        {
            _0rain,
            _1snow,
            _2milk,
            _3musicBeat,
            _4smoke
        }

        List<Rain> rainCollection;
        List<Snow> snowCollection;

        List<object> classCollection;

        private class Rain
        {
            private int x = 0;
            private int y = 0;
            public bool bShouldDestroy = false;
            public Point getPosition => new Point(x, y);
            public Rain(int x, int y)
                {
                this.x = x;
                this.y = y;
                }

            public void Update()
            {
                x-=8;
                y+=8;
                if (x == 0 || y > 480)
                    bShouldDestroy = true;
            }
        }

        private class Milk
        {
            public Texture2D snowTex;
            private float x = 0;
            private float y = 0;
            public bool bShouldDestroy = false;
            public Point getPosition => new Point((int)x, (int)y);
            public int angle = 0;
            private float directionX = 0;
            private float directionY = 0;
            private float r = 2.0f;
            public Milk(int x, int y, float r = 2.0f)
            {
                this.x = x;
                this.y = y;
                this.r = r;
                this.angle = random.Next(0, 359);
                this.directionX = (float)(r * Math.Cos(MathHelper.ToRadians(angle)));
                this.directionY = (float)(r * Math.Sin(MathHelper.ToRadians(angle)));
                int randomJupiter = random.Next(1, 16);
                this.x += directionX*randomJupiter;
                this.y += directionY*randomJupiter;
                byte[] bf = new byte[16];
                snowTex = new Texture2D(graphics.GraphicsDevice, 2, 2, false, SurfaceFormat.Color);
                for (int i = 0; i < 16; i += 4)
                {
                    bf[i] = 255;
                    bf[i + 1] = 255;
                    bf[i + 2] = 255;
                    bf[i + 3] = (byte)random.Next(16, 240);
                }
                snowTex.SetData(bf);
            }

            public void Update()
            {
                x += directionX;
                y += directionY;
                if ((x < 0 || y > 480) || (x > 640 || y < 0))
                    bShouldDestroy = true;
            }
        }

        private class Snow
        {
            public Texture2D snowTex;
            private int x = 0;
            private int y = 0;
            private bool bShouldChangeDirection = false;
            private int speed = -1;
            public bool bShouldDestroy = false;
            public Point getPosition => new Point(x, y);
            public Snow(int x, int y, int speed = -1)
            {
                this.x = x;
                this.y = y;
                byte[] bf = new byte[16];
                snowTex = new Texture2D(graphics.GraphicsDevice, 2, 2, false, SurfaceFormat.Color);
                for (int i = 0; i < 16; i += 4) {
                    bf[i] = 255;
                    bf[i+1] = 255;
                    bf[i+2] = 255;
                    bf[i+3] = (byte)random.Next(16, 240);
                }
                snowTex.SetData(bf);
                if (this.speed == -1)
                    this.speed = random.Next(1, 3);
                else
                    this.speed = speed;
            }

            public void Update()
            {
                if (speed == -1) return;
                x += bShouldChangeDirection ? speed : speed * -1;
                if (random.Next(0, 30) == 0)
                    bShouldChangeDirection = bShouldChangeDirection ? false : true;
                y += speed;
                if (x == 0 || y > 480)
                    bShouldDestroy = true;
            }
        }

        private class Beat
        {
            Color color;
            public bool bshouldDestroy = false;
            int x;
            int y;
            int z;
            float mass;
            public Texture2D myTex;
            public float life = 1.0f;

            public Point GetPoint => new Point(x, y);
            public Point GetZ => new Point(z, z);
            public Beat(float fMass = 1.0f)
            {
                this.x = 640 / 2;
                this.mass = fMass;
                this.y = 360 / 2;
                this.z = 0;
                byte[] b = new byte[superCircle.Width*superCircle.Height*4];
                int r = random.Next(0, 255); int g = random.Next(0, 255); int bb = random.Next(0, 255);
                this.color = new Color((byte)r,(byte)g,(byte)bb);
                superCircle.GetData(b);
                myTex = new Texture2D(graphics.GraphicsDevice, 16, 16, false, SurfaceFormat.Color);
                for(int i = 0; i<b.Length; i+=4)
                {
                    b[i] = this.color.R;
                    b[i + 1] = this.color.G;
                    b[i + 2] = this.color.B;
                    //if(b[i+3] == 0xff)
                    //{
                    //    int spec = random.Next(0, 32);
                    //    if (spec == 0)
                    //        b[i + 3] = 0x00;
                    //}
                }
                myTex.SetData(b);
            }

            public void Update(GameTime gameTime)
            {
                x-=2;
                y-=2;
                z+=4;
                life -= gameTime.ElapsedGameTime.Milliseconds / (100.0f*this.mass);
                bshouldDestroy = life < 0.0f;
            }
        }

        private class Smoke
        {
            public bool bshouldDestroy = false;
            int x;
            int y;
            float mass = 1.0f;
            public Texture2D myTex;
            public float life = 1.0f;

            public Point GetPoint => new Point(x, y);
            public Smoke()
            {
                this.x = 640 / 2 - random.Next(-20,20);
                this.y = 360 - 360 / 4;
                this.mass = random.Next(1, 5) * 1f;
                byte[] b = new byte[4*4*4];
                myTex = new Texture2D(graphics.GraphicsDevice, 4, 4, false, SurfaceFormat.Color);
                for (int i = 0; i < b.Length; i += 4)
                {
                    byte grey = (byte)random.Next(16, 255);
                    b[i] = grey; b[i + 1] = grey; b[i + 2] = grey; b[i + 3] = (byte)random.Next(16, 32);
                }
                myTex.SetData(b);
            }

            public void Update(GameTime gameTime)
            {
                x -= random.Next(-10,10);
                y -= random.Next(1,3);
                life -= gameTime.ElapsedGameTime.Milliseconds / (1000.0f * mass);
                bshouldDestroy = life < 0.0f;
            }
        }

        private static Modules mod;
        private static Modules lastMod;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 640;
            graphics.PreferredBackBufferHeight = 360;
            graphics.ApplyChanges();
            Window.AllowUserResizing = true;
            random = new Random();
            base.Initialize();
        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            raind = Content.Load<Texture2D>("raind");
            superCircle = Content.Load<Texture2D>("flare");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();



            mod = Keyboard.GetState().IsKeyDown(Keys.F1) ? Modules._0rain :
                Keyboard.GetState().IsKeyDown(Keys.F2) ? Modules._1snow :
                Keyboard.GetState().IsKeyDown(Keys.F3) ? Modules._2milk :
                Keyboard.GetState().IsKeyDown(Keys.F4) ? Modules._3musicBeat :
                Keyboard.GetState().IsKeyDown(Keys.F5) ? Modules._4smoke

                : mod;

            if(lastMod!= mod)
            {
                classCollection = null;
                lastMod = mod;
            }

            switch (mod)
            {
                case Modules._0rain:
                    RainUpdate();
                    break;
                case Modules._1snow:
                    SnowUpdate();
                    break;
                case Modules._2milk:
                    MilkUpdate();
                    break;
                case Modules._3musicBeat:
                    BeatUpdate(gameTime);
                    break;
                case Modules._4smoke:
                    SmokeUpdate(gameTime);
                    break;
                    
            }


            base.Update(gameTime);
        }



        protected override void Draw(GameTime gameTime)
        {
            switch (mod)
            {
                case Modules._0rain:
                    RainDraw();
                    break;
                case Modules._1snow:
                    SnowDraw();
                    break;
                case Modules._2milk:
                    MilkDraw();
                    break;
                case Modules._3musicBeat:
                    BeatDraw();
                    break;
                case Modules._4smoke:
                    SmokeDraw();
                    break;
            }

            base.Draw(gameTime);
        }

        private void SmokeUpdate(GameTime gameTime)
        {
            int smokeCount = 1000;
            if (classCollection == null)
            {
                classCollection = new List<object>();
            }
            while(classCollection.Count() < smokeCount)
                for (int i = 0; i < smokeCount; i++)
                    classCollection.Add(new Smoke());
            
                for (int i = 0; i<classCollection.Count; i++)
            {
                (classCollection[i] as Smoke).Update(gameTime);
                if ((classCollection[i] as Smoke).bshouldDestroy)
                    classCollection.Remove(classCollection[i]);
            }
        }

        private void SmokeDraw()
        {
            if (classCollection == null)
                return;
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearClamp);
            foreach (Smoke smoke in classCollection)
                spriteBatch.Draw(smoke.myTex, new Rectangle(smoke.GetPoint, new Point(smoke.myTex.Width*4, smoke.myTex.Height*4)), Color.White * smoke.life);
            spriteBatch.End();
        }



        private void BeatUpdate(GameTime gameTime)
        {
            if (classCollection == null)
                classCollection = new List<object>();
            if(Keyboard.GetState().IsKeyDown(Keys.Space) && !bShouldBlockInput)
                classCollection.Add(new Beat(random.Next(10,50) * 1.0f));
            bShouldBlockInput = !Keyboard.GetState().IsKeyUp(Keys.Space);
            for (int i = 0; i < classCollection.Count; i++)
            {
                (classCollection[i] as Beat).Update(gameTime);
                if ((classCollection[i] as Beat).bshouldDestroy)
                    classCollection.Remove(classCollection[i]);
            }
            }

        private void BeatDraw()
        {
            if (classCollection == null)
                return;

            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearClamp);
            foreach (Beat beat in classCollection)
                spriteBatch.Draw(beat.myTex, new Rectangle(beat.GetPoint, beat.GetZ), Color.White * beat.life);
            //spriteBatch.Draw(beat.myTex, new Rectangle(beat.GetPoint, new Point(320 + (320 - beat.GetPoint.X), 360 / 2 + (360 / 2 - beat.GetPoint.Y))), Color.White * beat.life);
            spriteBatch.End();

        }

        private void MilkUpdate()
        {
            int starsCount = 500;
            int half = 640 / 2;
            int halfh = 360 / 2;
            if (classCollection == null)
            {
                classCollection = new List<object>();

                for (int i = 0; i < starsCount; i++)
                    classCollection.Add(new Milk(half, halfh));
            }
            while (classCollection.Count != starsCount)
                classCollection.Add(new Milk(half, halfh));
            for (int i = 0; i<classCollection.Count; i++)
            {
                (classCollection[i] as Milk).Update();
                if ((classCollection[i] as Milk).bShouldDestroy)
                    classCollection.Remove(classCollection[i]);
            }
        }

        private void MilkDraw()
        {
            if (classCollection == null)
                return;
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
            foreach (Milk milk in classCollection)
            {
                spriteBatch.Draw(milk.snowTex, new Rectangle(milk.getPosition, new Point(2, 2)), Color.White * (random.Next(0, 80) / 100.0f));
            }
            spriteBatch.End();
        }

        private void SnowUpdate()
        {
            int snowCount = 1500;
            if (snowCollection == null)
            {
                snowCollection = new List<Snow>();
                for(int i = 0; i<snowCount; i++)
                snowCollection.Add(new Snow(random.Next(0, 640), random.Next(0, 360)));
            }
            while (snowCollection.Count != snowCount)
            {
                snowCollection.Add(new Snow(random.Next(0, 640), random.Next(0,1)));
            }
            for (int i = 0; i < snowCollection.Count; i++)
            {
                snowCollection[i].Update();
                if (snowCollection[i].bShouldDestroy)
                    snowCollection.Remove(snowCollection[i]);
            }
        }

        private void SnowDraw()
        {
            if (snowCollection == null)
                return;
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
            foreach (Snow snow in snowCollection)
            {
                spriteBatch.Draw(snow.snowTex, new Rectangle(snow.getPosition, new Point(2, 2)), Color.White * (random.Next(5, 30) / 100.0f));
            }
            spriteBatch.End();
        }


        private void RainUpdate()
        {
            int raindrops = 1500;
            if (rainCollection == null)
            {
                rainCollection = new List<Rain>();
                for(int i = 0; i<raindrops; i++)
                rainCollection.Add(new Rain(random.Next(10, 640 * 2), random.Next(0, 360)));
            }
            while(rainCollection.Count != raindrops)
            {
                rainCollection.Add(new Rain(random.Next(10, 640*2), 0));
            }
            for(int i = 0; i<rainCollection.Count; i++)
            {
                rainCollection[i].Update();
                if (rainCollection[i].bShouldDestroy)
                    rainCollection.Remove(rainCollection[i]);
            }

        }



        private void RainDraw()
        {
            if (rainCollection == null)
                return;
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
            foreach (Rain rain in rainCollection)
            {
                spriteBatch.Draw(raind, new Rectangle(rain.getPosition, new Point(8, 8)), Color.White * (random.Next(10, 60) / 100.0f));
            }
            spriteBatch.End();
        }
    }
}
