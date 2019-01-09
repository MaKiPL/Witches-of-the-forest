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
        static Random random;
        SpriteFont sf;

        enum Modules
        {
            _0rain,
            _1snow,
            _2milk
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
                Keyboard.GetState().IsKeyDown(Keys.F3) ? Modules._2milk 

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
            }

            base.Draw(gameTime);
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
