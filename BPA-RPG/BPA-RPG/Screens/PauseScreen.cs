﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using EtStellas.GameObjects;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace EtStellas.Screens
{
    public class PauseScreen : Screen
    {
        private Background background;
        private Texture2D overlay;

        private SpriteFont font;
        private List<DrawableString> options;

        private SoundEffectInstance select;

        public PauseScreen()
            : base("Pause")
        {
            translucent = true;
        }

        public override void LoadContent(ContentManager content)
        {
            background = new Background(Color.Black * 0.6f);
            overlay = content.Load<Texture2D>("Images/TitleScreen");

            font = content.Load<SpriteFont>("Fonts/TitleFont");
            options = new List<DrawableString>();

            //Resume option
            options.Add(CreateOption("Resume", 0, () => manager.Pop()));

            //Save option
            options.Add(CreateOption("Save", 1, () => PlayerData.SaveGame()));

            //Options option
            options.Add(CreateOption("Options", 2, () => manager.Push(new OptionsScreen())));

            //Exit option
            options.Add(CreateOption("Exit", 3, () =>
            {
                manager.Pop();
                manager.Pop();
            }));

            //Sounds
            select = SoundEffectManager.GetEffectInstance("Select1");

            base.LoadContent(content);
        }

        public override void Update(GameTime gameTime)
        {
            if (InputManager.newKeyState.IsKeyDown(Keys.Escape) && InputManager.oldKeyState.IsKeyUp(Keys.Escape))
                manager.Pop();

            foreach (DrawableString option in options)
                option.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spritebatch)
        {
            background.Draw(gameTime, spritebatch);
            spritebatch.Draw(overlay, Vector2.Zero, Color.White);

            foreach (DrawableString option in options)
                option.Draw(gameTime, spritebatch);

            base.Draw(gameTime, spritebatch);
        }

        private DrawableString CreateOption(string name, int index, Action onClick = null)
        {
            return new DrawableString(font, name, new Vector2(880, 280 + 60 * index) - font.MeasureString(name), Color.White,
                (() => select.Play()) + onClick,
                () =>
                {
                    options[index].text = name + "  ";
                    options[index].position.X = 880 - font.MeasureString(options[index].text).X;
                },
                () =>
                {
                    options[index].text = name;
                    options[index].position.X = 880 - font.MeasureString(options[index].text).X;
                });
        }
    }
}
