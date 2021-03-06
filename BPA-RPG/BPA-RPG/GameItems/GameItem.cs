﻿using EtStellas.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace EtStellas.GameItems
{
    /// <summary>
    /// Represents an item the player can buy, sell, and own
    /// </summary>
    public class GameItem : GameObject
    {
        //List of all items
        public static List<GameItem> items = new List<GameItem>();


        private static Texture2D infoBoxCap;
        private static Texture2D infoBox;
        private static SpriteFont font;


        public readonly string name;
        public string info { get; private set; }

        /// <summary>
        /// Creates a new GameItem
        /// </summary>
        /// <param name="name">The display name of the item</param>
        /// <param name="texture">The texture of the item</param>
        /// <param name="info">Optional item info/lore</param>
        protected GameItem(string name, Texture2D texture, string info = "")
            : base(texture)
        {
            this.name = name;
            this.info = info;

            items.Add(this);

            MainGame.eventLogger.Log(this, "Loaded \"" + name + "\"");
        }

        /// <summary>
        /// Draws an info box for this item at the mouse's location
        /// </summary>
        /// <param name="spritebatch"></param>
        public void DrawInfo(SpriteBatch spritebatch)
        {
            if (string.IsNullOrEmpty(info))
                return;
            
            Vector2 pos = InputManager.newMouseState.Position.ToVector2() + new Vector2(12, 0);

            for (int i = 0; i < info.Length; i++)
            {
                if (font.MeasureString(info.Substring(0, i)).X > 164)
                {
                    if (info.Contains(" "))
                    {
                        int k = info.Substring(0, i).LastIndexOf(" ");
                        string newLine = info.Remove(k, 1).Insert(k, "\n");

                        if (font.MeasureString(newLine.Substring(0, i)).X <= 164)
                        {
                            info = newLine;
                            i++;
                        }
                    }

                    //If that didn't fix it
                    if (font.MeasureString(info.Substring(0, i)).X > 164)
                    {
                        info = info.Insert(i - 2, "-\n");
                        i += 2;
                    }
                }
            }


            Vector2 boxPos = pos;
            spritebatch.Draw(infoBoxCap, boxPos, Color.White);
            boxPos += new Vector2(0, infoBoxCap.Height);

            for (int i = 0; i <= info.Count(c => c == '\n'); i++)
            {
                spritebatch.Draw(infoBox, boxPos, Color.White);
                boxPos += new Vector2(0, infoBox.Height);
            }
                
            Vector2 b = font.MeasureString("N\nN");

            spritebatch.Draw(infoBoxCap, boxPos, new Rectangle(0, 0, infoBoxCap.Width, infoBoxCap.Height), Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.FlipVertically, 1);
            spritebatch.DrawString(font, info, pos + new Vector2(6, infoBoxCap.Height), Color.White);
        }

        /// <summary>
        /// Loads all items
        /// </summary>
        public static void LoadContent(ContentManager content)
        {
            MainGame.eventLogger.Log(typeof(GameItem), "Begin loading game items.");

            infoBoxCap = content.Load<Texture2D>("Images/ItemInfoBoxCap");
            infoBox = content.Load<Texture2D>("Images/ItemInfoBox");
            font = content.Load<SpriteFont>("Fonts/KeyFont");

            new GameItem("Fuel", content.Load<Texture2D>("Images/Items/Fuel"), "Plutonium fuel for spacecrafts.");
            new GameItem("Roxol Quartz", content.Load<Texture2D>("Images/Items/RoxolQuartz"), "An uncommon crystal used in most modern machinery.");
            new GameItem("Naphtha", content.Load<Texture2D>("Images/Items/Naphtha"), "Unrefined oil for machinery and spacecrafts.");
            new GameItem("Red Eye", content.Load<Texture2D>("Images/items/RedEye"), "A dangerous drug highly sought after on the black market.");
            new GameItem("Waning Sun", content.Load<Texture2D>("Images/Items/WaningSun"), "A sun on the brink of a supernova, held in statis through quantum locking.");
            new GameItem("Korian Steel", content.Load<Texture2D>("Images/Items/KorianSteel"), "High quality steel alloy produced on Koria.");
            new GameItem("Mining Drill", content.Load<Texture2D>("Images/Items/MiningDrill"), "A small electronic hand drill used to extract ores and minerals.");
            new GameItem("Q Chip", content.Load<Texture2D>("Images/Items/QChip"), "A miniture quantum computer. Used in almost all modern electronics.");

            MainGame.eventLogger.Log(typeof(GameItem), "Finished loading game items.");
        }

        /// <summary>
        /// Parses a string for the name of an item
        /// </summary>
        /// <param name="line">String to parse</param>
        /// <returns>Gameitem the string represents</returns>
        public static GameItem Parse(string line)
        {
            return items.First(x => x.name.Replace(" ", "").ToLower() == line.ToLower());
        }
    }
}
