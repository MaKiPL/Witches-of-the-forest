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

            switch (mod)
            {
                case Modules._0rain:
                    RainUpdate();
                    break;
                case Modules._1snow:
                    SnowUpdate();
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
            }

            base.Draw(gameTime);
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
