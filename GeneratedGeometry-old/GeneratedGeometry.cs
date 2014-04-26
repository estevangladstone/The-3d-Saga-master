#region File Description
//-----------------------------------------------------------------------------
// GeneratedGeometry.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
#endregion

namespace GeneratedGeometry
{
    /// <summary>
    /// Sample showing how to use geometry that is programatically
    /// generated as part of the content pipeline build process.
    /// </summary>
    public class GeneratedGeometryGame : Microsoft.Xna.Framework.Game
    {
        #region Fields

        GraphicsDeviceManager graphics;
        Ship.Camera camera;
        float aspectRatio;

        Ship.CObject terrain;
        Ship.CObject untitled;
        Ship.CObject floor;
        Ship.Ring ring;
        List<Ship.CObject> walls;
        Sky sky;
        Ship.LirouShip ship;
        List<Ship.CObject> collidableObjects;

        #endregion

        #region Initialization


        public GeneratedGeometryGame()
        {
            graphics = new GraphicsDeviceManager(this);
            //graphics.IsFullScreen = true;
            //graphics.PreferredBackBufferWidth = 1920;
            //graphics.PreferredBackBufferHeight = 1080;
            //IsMouseVisible = true;

            floor = new Ship.CObject();
            floor.ID = "floor";
            floor.Position = new Vector3(0.0f, -30.0f, 0.0f);
            floor.Scale = 1.0f;
            floor.Specs = new Vector3(40.0f, 2.0f, 40.0f);


            walls = new List<Ship.CObject>();
            for (int i = 0; i < 4; i++)
            {
                walls.Add(new Ship.CObject());
                walls[i].ID = "wall" + i;
                walls[i].Scale = 1.0f;
                walls[i].Rotation = new Vector3(0.0f, MathHelper.PiOver2 * (i % 2), 0.0f);
                if (walls[i].Rotation.Y == 0.0f)
                {
                    walls[i].Specs = new Vector3(40.0f, 40.0f, 2.0f);
                }
                else
                {
                    walls[i].Specs = new Vector3(2.0f, 40.0f, 40.0f);
                }
            }
            walls[0].Position = new Vector3(0.0f, 0.0f, -30.0f);
            walls[1].Position = new Vector3(30.0f, .0f, 0.0f);
            walls[2].Position = new Vector3(0.0f, 0.0f, 30.0f);
            walls[3].Position = new Vector3(-30.0f, 0.0f, 0.0f);

            terrain = new Ship.CObject();
            terrain.ID = "terrain";

            untitled = new Ship.CObject();
            untitled.ID = "untitled";

            untitled.Position = Vector3.Zero;
            untitled.Scale = 1.0f;
            
            aspectRatio = (float)graphics.PreferredBackBufferWidth / (float)graphics.PreferredBackBufferHeight;
            Content.RootDirectory = "Content";
            ship = new Ship.LirouShip(new Vector3(0, 10, 0), new Vector3(0, 0, 0), 0.1f, (float)(Math.PI / 50), 0.002f);

            ring = new Ship.Ring(new Vector3(30, 0, 30), (float)(Math.PI / 50), 0.08f);

            camera = new Ship.Camera(25, ship.Position);

            collidableObjects = new List<Ship.CObject>();
            collidableObjects.Add(untitled);
            collidableObjects.Add(floor);
            collidableObjects.AddRange(walls);
            //collidableObjects.Add(ship);

            #if WINDOWS_PHONE
                        // Frame rate is 30 fps by default for Windows Phone.
                        TargetElapsedTime = TimeSpan.FromTicks(333333);

                        graphics.IsFullScreen = true;
            #endif
        }


        /// <summary>
        /// Load your graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            terrain.Model = Content.Load<Model>("Terrain/terrain");
            untitled.Model = Content.Load<Model>("Models/untitled");
            floor.Model = Content.Load<Model>("Models/floor");
            ring.Model = Content.Load<Model>("Models/ring");

            for (int i = 0; i < 4; i++)
            {
                walls[i].Model = Content.Load<Model>("Models/wall");
            }
            sky = Content.Load<Sky>("sky");
            ship.LoadContent(this.Content);
        }


        #endregion

        #region Update and Draw

        /// <summary>
        /// Allows the game to run logic.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState ks = Keyboard.GetState(); 
            HandleInput();
            ship.Update(ks, camera, collidableObjects);
            ring.Animate();
            if (ks.IsKeyDown(Keys.G))
            {
                Console.WriteLine("LOL" + ship.Position);
            }

            camera.Update(Mouse.GetState(), gameTime, ship.Position, ship.Rotation, graphics);
            
            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice device = graphics.GraphicsDevice;

            device.Clear(Color.Black);
            
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, device.Viewport.AspectRatio, 1, 10000);
            Matrix view = Matrix.CreateLookAt(camera.Position, ship.Position, Vector3.Up);
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            // Draw the terrain first, then the sky. This is faster than
            // drawing the sky first, because the depth buffer can skip
            // bothering to draw sky pixels that are covered up by the
            // terrain. This trick works because the code used to draw
            // the sky forces all the sky vertices to be as far away as
            // possible, and turns depth testing on but depth writes off.

            terrain.DrawTerrain(view, projection);
            terrain.Draw(view, projection, aspectRatio);

            foreach (Ship.CObject cobject in collidableObjects)
            {
                cobject.Draw(view, projection, aspectRatio);
            }

            ship.Draw(aspectRatio, view);

            ring.Animate();

            sky.Draw(view, projection);

            // If there was any alpha blended translucent geometry in
            // the scene, that would be drawn here, after the sky.

            base.Draw(gameTime);
        }
        
        /// <summary>
        /// Helper for drawing the terrain model.
        /// </summary>
        
        #endregion

        #region Handle Input


        /// <summary>
        /// Handles input for quitting the game.
        /// </summary>
        private void HandleInput()
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();
            GamePadState currentGamePadState = GamePad.GetState(PlayerIndex.One);

            // Check for exit.
            if (currentKeyboardState.IsKeyDown(Keys.Escape) ||
                currentGamePadState.Buttons.Back == ButtonState.Pressed)
            {
                Exit();
            }
        }


        #endregion
    }


    #region Entry Point

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static class Program
    {
        static void Main()
        {
            using (GeneratedGeometryGame game = new GeneratedGeometryGame())
            {
                game.Run();
            }
        }
    }

    #endregion
}
