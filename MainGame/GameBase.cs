﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using First.MainGame.GameObjects;
using First.MainGame;
using System.IO;
using Newtonsoft.Json;

namespace First.MainGame {
    public class Game1 : Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Viewport viewport;

        //TODO: text rendering
        //TODO: sound engine
        //TODO: particle system

        public Handler handler;

        public Game1() {

            GraphicalSettings.screensize = new Vector2(1280, 720);
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = (int) GraphicalSettings.screensize.X;
            graphics.PreferredBackBufferHeight = (int) GraphicalSettings.screensize.Y;
            graphics.SynchronizeWithVerticalRetrace = false;
            TargetElapsedTime = TimeSpan.FromSeconds(1f / 144f);
            Content.RootDirectory = "Content";
            IsFixedTimeStep = false;
            IsMouseVisible = false;
        }

        protected override void Initialize() {
            base.Initialize();
            viewport = graphics.GraphicsDevice.Viewport;
            Start();
        }

        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Sprite.SpriteDictionary = new Dictionary<string, Texture2D>();

            LoadSprites();
        }

        void LoadSprites() {
            //Define Sprites
            Sprite.AddSprite("Car", Content.Load<Texture2D>("Car"));
            Sprite.AddSprite("Player", Content.Load<Texture2D>("player"));
            Sprite.AddSprite("Grass", Content.Load<Texture2D>("grass"));
            Sprite.AddSprite("Selectable", Content.Load<Texture2D>("Selectable"));
            Sprite.AddSprite("NotSelectable", Content.Load<Texture2D>("NotSelectable"));
            Sprite.AddSprite("CursorRed", Content.Load<Texture2D>("CursorRed"));
            Sprite.AddSprite("CursorBlue", Content.Load<Texture2D>("CursorBlue"));
            Sprite.AddSprite("CanMove", Content.Load<Texture2D>("CanMove"));
            Sprite.AddSprite("Concrete", Content.Load<Texture2D>("Concrete"));
            Sprite.AddSprite("Blank", Content.Load<Texture2D>("Blank"));
            Sprite.AddSprite("Error", Content.Load<Texture2D>("Error"));
            Sprite.AddSprite("LongGrass", Content.Load<Texture2D>("LongGrass"));
            Sprite.AddSprite("White", Content.Load<Texture2D>("White"));
            Sprite.AddSprite("Black", Content.Load<Texture2D>("Black"));
            //Define Items
            Item.ItemDictionary.Add(ItemType.Air, new Sprite(Sprite.SpriteDictionary ["Blank"]));
            Item.ItemDictionary.Add(ItemType.Error, new Sprite(Sprite.SpriteDictionary ["Error"]));
            Item.ItemDictionary.Add(ItemType.Grass, new Sprite(Sprite.SpriteDictionary ["Grass"]));
            Item.ItemDictionary.Add(ItemType.LongGrass, new Sprite(Sprite.SpriteDictionary ["LongGrass"]));
        }

        protected override void UnloadContent() {
            Handler.savestate.Save();
        }


        void Start() {
            //Custom Additions
            Handler.addGameObject(new Player(new Vector2(0), new Sprite(Sprite.SpriteDictionary ["Player"])));
            Handler.addGameObject(new Selection());
            Light.addLight(new Light(Vector2.Zero, 5, .3f, Color.White));
            Light.addLight(new Light(Vector2.Zero + new Vector2(5, 0), 6, .3f, Color.White));
            Light.addLight(new Light(Vector2.Zero + new Vector2(0, 5), 5, .25f, Color.White));
            Handler.world.processVisibleTiles();
            Light.processVisibleLights();
        }

        private float elapsed = 0;
        List<float> avg = new List<float>();
        protected override void Update(GameTime gameTime) {
            Camera.matrix = Camera.get_transformation(GraphicsDevice);
            Time.deltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;
            elapsed += Time.deltaTime;
            avg.Add(Time.deltaTime);

            while(elapsed > 2f) {
                float t = 0;
                foreach(float a in avg) {
                    t += a;
                }
                t /= avg.Count;
                avg.Clear();


                elapsed -= 2f;
                Console.WriteLine(DateTime.Now.ToString("h:mm:ss tt"));
                Console.WriteLine("AVG FPS: " + (1 / t));
                Console.WriteLine("TILES: " + Handler.world.map.Count);
                Console.WriteLine("CURRENT TICK: " + Handler.world.tick);
                Console.WriteLine("DRAW CALLS: " + Sprite.drawcalls);
            }
            Sprite.drawcalls = 0;


            //emergency exit
            if(GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape)) {
                Exit();
            }

            //update
            Handler.Update();

        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, new RasterizerState { MultiSampleAntiAlias = true }, null, Camera.get_transformation(GraphicsDevice));
            Handler.Render(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
