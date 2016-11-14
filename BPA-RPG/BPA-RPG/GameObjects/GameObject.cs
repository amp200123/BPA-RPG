﻿using BPA_RPG.GameItems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BPA_RPG.GameObjects
{
    /// <summary>
    /// Represents some object in the game
    /// </summary>
    public class GameObject
    {
        private Texture2D Texture;
        public Texture2D texture
        {
            get
            {
                return Texture;
            }

            set
            {
                //Set all values dependant on the texture

                Texture = value;
                Width = value.Width;
                Height = value.Height;

                colorData = new Color[Width * Height];
                value.GetData(colorData);
            }

        }

        public Vector2 position;
        public float rotation = 0;
        public float scale = 1;
        public bool visible = true;

        public int Width { get; private set; }
        public int Height { get; private set; }

        /// <summary>
        /// Color data for the pixels of this objects image
        /// </summary>
        public Color[] colorData { get; private set; }

        /// <summary>
        /// Matrix that represents all the transformations done on the object
        /// </summary>
        public Matrix transformMatrix
        {
            get
            {
                return
                Matrix.CreateTranslation(new Vector3(-Width / 2, -Height / 2, 0.0f)) *
                Matrix.CreateRotationZ(rotation) *
                Matrix.CreateScale(scale) *
                Matrix.CreateTranslation(new Vector3(position, 0.0f));
            }
        }

        /// <summary>
        /// Rectangle that represents bounds of the object
        /// </summary>
        public Rectangle boundingRectangle
        {
            get
            {
                Rectangle rectangle = new Rectangle(0, 0, Width, Height);
                Matrix transform = transformMatrix;

                //Get all four corners in local space
                Vector2 topLeft = new Vector2(rectangle.Left, rectangle.Top);
                Vector2 topRight = new Vector2(rectangle.Right, rectangle.Top);
                Vector2 botLeft = new Vector2(rectangle.Left, rectangle.Bottom);
                Vector2 botRight = new Vector2(rectangle.Right, rectangle.Bottom);

                //Transform corners into work space
                Vector2.Transform(ref topLeft, ref transform, out topLeft);
                Vector2.Transform(ref topRight, ref transform, out topRight);
                Vector2.Transform(ref botLeft, ref transform, out botLeft);
                Vector2.Transform(ref botRight, ref transform, out botRight);

                //Find minimum and maximum extents of the rectangle in world space
                Vector2 min = Vector2.Min(Vector2.Min(topLeft, topRight),
                    Vector2.Min(botLeft, botRight));
                Vector2 max = Vector2.Max(Vector2.Max(topLeft, topRight),
                    Vector2.Max(botLeft, botRight));

                return new Rectangle((int)min.X, (int)min.Y,
                    (int)(max.X - min.X), (int)(max.Y - min.Y));
            }
        }


        public GameObject(Texture2D texture)
        {
            this.texture = texture;
            position = new Vector2(0, 0);

            if (!(this is GameItem))
                MainGame.eventLogger.Log(this, "Loaded " + "\"" + texture.ToString() + "\"");
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gametime">Provides a snapshot of timing values.</param>
        public virtual void Update(GameTime gameTime)
        {
        }

        
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="spritebatch">Spritebatch object to draw objects with</param>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="color">Color to draw in.</param>
        public virtual void Draw(GameTime gameTime, SpriteBatch spritebatch, Color color)
        {
            if (visible)
            {
                Rectangle source = new Rectangle(0, 0, Width, Height);
                spritebatch.Draw(texture, position, source, color, rotation,
                    new Vector2(Width / 2, Height / 2), scale, SpriteEffects.None, 1);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="spritebatch">Spritebatch object to draw objects with</param>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Draw(GameTime gameTime, SpriteBatch spritebatch)
        {
            Draw(gameTime, spritebatch, Color.White);
        }

        /// <summary>
        /// Checks if this is colliding with the given GameObject
        /// </summary>
        public bool IntersectPixels(GameObject obj)
        {
            // Calculate a matrix which transforms from A's local space into
            // world space and then into B's local space
            Matrix transformAToB = transformMatrix * Matrix.Invert(obj.transformMatrix);

            //For each row of pixel in A
            for (int yA = 0; yA < Height; yA++)
            {
                //For each pixel in that row
                for (int xA = 0; xA < Width; xA++)
                {
                    //Calculate this pixel's location in B
                    Vector2 positionInB = Vector2.Transform(new Vector2(xA, yA), transformAToB);

                    int xB = (int)Math.Round(positionInB.X);
                    int yB = (int)Math.Round(positionInB.Y);

                    if (xB >= 0 && xB < obj.Width &&
                        yB >= 0 && yB < obj.Height)
                    {
                        //Get colors of the overlapping pixels
                        Color colorA = colorData[xA + yA * Width];
                        Color colorB = obj.colorData[xB + yB * obj.Width];

                        //If both pixels are not completely transparent
                        if (colorA.A * colorB.A != 0)
                        {
                            //Intersection found
                            return true;
                        }
                    }
                }
            }

            //No intersection
            return false;
        }

        public void ScaleTo(int width, int height, float scale = 1)
        {
            this.scale = width >= height
                ? (width * scale) / Width
                : (height * scale) / Height;
        }

        public void ScaleTo(Rectangle rect, float scale = 1)
        {
            ScaleTo(rect.Width, rect.Height, scale);
        }
    }
}