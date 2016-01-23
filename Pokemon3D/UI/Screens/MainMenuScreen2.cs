﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon3D.GameCore;
using Pokemon3D.Common.Input;
using Pokemon3D.UI.Transitions;
using Pokemon3D.UI.Framework;
using Pokemon3D.UI.Framework.Dialogs;

namespace Pokemon3D.UI.Screens
{
    class MainMenuScreen2 : GameObject, Screen
    {
        private ControlGroup _buttons = new ControlGroup();
        private HexagonBackground _hexagons = new HexagonBackground();

        private SelectionDialog _closeDialog;

        public void OnOpening(object enterInformation)
        {
            Game.GraphicsDeviceManager.PreferMultiSampling = true;
            Game.GraphicsDeviceManager.ApplyChanges();

            _buttons.Add(new LeftSideButton("Start new game", new Vector2(26, 45), b =>
            {
                Game.ScreenManager.SetScreen(typeof(GameModeLoadingScreen), typeof(BlendTransition));
            }));
            _buttons.Add(new LeftSideButton("Load game", new Vector2(26, 107), null)
            {
                Enabled = false
            });
            _buttons.Add(new LeftSideButton("GameJolt", new Vector2(26, 169), null));
            _buttons.Add(new LeftSideButton("Options", new Vector2(26, 231), null));
            _buttons.Add(new LeftSideButton("Exit game", new Vector2(26, 293), (b) =>
            {
                _closeDialog.Show();
            }));
            _buttons.Add(new LeftSideCheckbox("Checkbox test", new Vector2(26, 355), null));

            _closeDialog = new SelectionDialog("Do you reall want to exit?", "", new LeftSideButton[] 
            {
                new LeftSideButton("No", new Vector2(50, 50), null),
                new LeftSideButton("Yes", new Vector2(50, 100), null)
            }, 0);
        }
        
        public void OnClosing()
        {

        }

        public void OnDraw(GameTime gameTime)
        {
            Game.GraphicsDevice.Clear(Color.LightGray);
            
            Game.SpriteBatch.Begin(blendState: BlendState.NonPremultiplied);

            _hexagons.Draw(Game.SpriteBatch);

            Game.SpriteBatch.End();

            Game.SpriteBatch.Begin(samplerState: SamplerState.PointWrap, blendState: BlendState.AlphaBlend);

            _buttons.Draw();

            Game.ShapeRenderer.DrawFilledRectangle(0, Game.ScreenBounds.Height - 64, Game.ScreenBounds.Width, 64, Color.White);

            Game.SpriteBatch.Draw(Game.Content.Load<Texture2D>(ResourceNames.Textures.UI.GamePadButtons.Button_A), new Rectangle(11, Game.ScreenBounds.Height - 48, 32, 32), new Color(100, 193, 238));
            Game.SpriteBatch.DrawString(Game.Content.Load<SpriteFont>(ResourceNames.Fonts.BigFont), "Select", new Vector2(56, Game.ScreenBounds.Height - 48), new Color(100, 193, 238));

            Game.SpriteBatch.End();

            _closeDialog.Draw();
        }

        public void OnUpdate(float elapsedTime)
        {
            _buttons.Update();
            _hexagons.Update();
            _closeDialog.Update();
        }
    }
}
