﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon3D.GameCore;
using Pokemon3D.UI.Framework;
using Pokemon3D.Common;
using Pokemon3D.Common.Input;
using Pokemon3D.UI.Screens.Tablet;
using Pokemon3D.Common.Shapes;

namespace Pokemon3D.UI.Screens
{
    class TabletScreen : GameObject, Screen
    {
        private TextureProjectionQuad _quad;
        private Texture2D _sideTexture, _shineTexture, _backTexture, _emitterTexture, _circuitTexture;
        private int _flickerChance;
        private OffsetTransition _sideSlider;
        private ShapeRenderer _renderer;
        private int _introDelay;
        private bool _closing;
        private SpriteFont _font, _bigFont;
        private RenderTarget2D _mainTarget, _target;
        private TabletPlugin _plugin;
        private int _pluginTitleIntro = 0;

        public const int TABLET_TARGET_WIDTH = 1200;
        public const int TABLET_TARGET_HEIGHT = 800;

        // tablet rotation consts.
        private const float CAMERA_X_TARGET = 0f;
        private const float CAMERA_Y_TARGET = -0.1f;
        private const float CAMERA_X_MAX_OFFSET = 0.6f;
        private const float CAMERA_Y_MAX_OFFSET = 0.6f;
        private const float CAMERA_RETURN_SPEED = 0.05f;

        // if the tablet should move towards the mouse.
        private bool _doMouseTurnUpdate = true;
        // the last known position of the mouse on the screen
        private Point _lastMousePosition = Point.Zero;

        // current rotation of the tablet.
        private float _cameraX = CAMERA_X_TARGET;
        private float _cameraY = CAMERA_Y_TARGET;

        public TextureProjectionQuad ActiveQuad
        {
            get { return _quad; }
        }

        public void OnOpening(object enterInformation)
        {
            _quad = new TextureProjectionQuad();
            _quad.TextureOutputWidth = TABLET_TARGET_WIDTH;
            _quad.TextureOutputHeight = TABLET_TARGET_HEIGHT;

            _sideTexture = Game.Content.Load<Texture2D>(ResourceNames.Textures.UI.Tablet.Tablet_Side);
            _shineTexture = Game.Content.Load<Texture2D>(ResourceNames.Textures.UI.Tablet.Tablet_Shine);
            _backTexture = Game.Content.Load<Texture2D>(ResourceNames.Textures.UI.Tablet.Tablet_Back);
            _emitterTexture = Game.Content.Load<Texture2D>(ResourceNames.Textures.UI.Tablet.Tablet_Emitter);
            _circuitTexture = Game.Content.Load<Texture2D>(ResourceNames.Textures.UI.Tablet.Circuit);
            _font = Game.Content.Load<SpriteFont>(ResourceNames.Fonts.NormalFont);
            _bigFont = Game.Content.Load<SpriteFont>(ResourceNames.Fonts.BigFont);

            _sideSlider = new OffsetTransition(0f, 0.8f);
            _sideSlider.TargetOffset = 540f;

            _renderer = new ShapeRenderer(Game.SpriteBatch);

            _flickerChance = 80;
            _introDelay = 12;
            _closing = false;

            _target = new RenderTarget2D(Game.GraphicsDevice, TABLET_TARGET_WIDTH, TABLET_TARGET_HEIGHT);
            _mainTarget = new RenderTarget2D(Game.GraphicsDevice, TABLET_TARGET_WIDTH, TABLET_TARGET_HEIGHT);
            SetPlugin(new MainMenuPlugin(this));
        }

        public void OnDraw(GameTime gameTime)
        {
            bool flickerResult = (_flickerChance == 0 || GlobalRandomProvider.Instance.Rnd.Next(0, _flickerChance) <= 5) && _introDelay == 0;

            // rendering the plugin before setting the render target to the screen target.
            // this is to avoid removing the screen target to render to a potential plugin target, because they need to be set in the right order.
            Texture2D pluginResult = null;

            if (flickerResult && !_closing)
                pluginResult = _plugin.Draw();

            // set the target to the main render target.
            // this contains the inner parts of the tablet screen, meaning the blue area.
            Game.GraphicsDevice.SetRenderTarget(_mainTarget);
            Game.GraphicsDevice.Clear(Color.Transparent);

            Game.SpriteBatch.Begin(blendState: BlendState.Additive);

            // drawing once the flickering is over:
            if (flickerResult)
            {
                _renderer.DrawRectangle(new Rectangle((int)(_target.Width / 2 - _sideSlider.Offset), _target.Height / 2 - 280, (int)(_sideSlider.Offset * 2), 560), new Color(77, 186, 216));

                if (_sideSlider.TargetOffset == _sideSlider.Offset)
                {
                    DrawCircuit();
                }
            }

            // end and begin spritebatch to change blend state, so the transparency for the plugin result texture works correctly.
            Game.SpriteBatch.End();

            Game.SpriteBatch.Begin(blendState: BlendState.AlphaBlend);

            if (pluginResult != null)
                Game.SpriteBatch.Draw(pluginResult, Vector2.Zero, Color.White);

            Game.SpriteBatch.End();

            // set to the target that will get transformed.
            Game.GraphicsDevice.SetRenderTarget(_target);
            Game.GraphicsDevice.Clear(Color.Transparent);

            // change blend state to non premuliplied to render the result of the main target (tablet screen) with a slight transparency.
            Game.SpriteBatch.Begin(blendState: BlendState.NonPremultiplied);

            Game.SpriteBatch.Draw(_mainTarget, Vector2.Zero, new Color(255, 255, 255, 250));

            // render the bars left and right.
            Game.SpriteBatch.Draw(_sideTexture, new Rectangle((int)(_target.Width / 2f - 64 - _sideSlider.Offset), (int)(_target.Height / 2f - 320), 64, 640), Color.White);
            Game.SpriteBatch.Draw(_sideTexture, new Rectangle((int)(_target.Width / 2f + _sideSlider.Offset), (int)(_target.Height / 2f - 320), 64, 640), null, Color.White, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0f);

            Game.SpriteBatch.End();

            // reset render target to back buffer, transform target and draw to back buffer
            Game.GraphicsDevice.SetRenderTarget(null);

            _quad.CameraPosition = new Vector3(_cameraX, _cameraY, 1.3f);
            _quad.Texture = _target;
            var projected = _quad.GetProjected();

            Game.SpriteBatch.Begin();

            Game.SpriteBatch.Draw(projected, Game.ScreenBounds, Color.White);

            Game.SpriteBatch.End();
        }

        private void DrawCircuit()
        {
            if (!_closing)
            {
                // this extracts parts from a sprite sheet atlas and draws the repeatedly to create some kind of structure.
                // it starts at X, Y coordinates and sorta moves along them:

                int startX = 128;
                int startY = 128;

                Game.SpriteBatch.Draw(_circuitTexture, new Rectangle(startX, startY, 16, 16), new Rectangle(0, 0, 16, 16), Color.White);
                Game.SpriteBatch.Draw(_circuitTexture, new Rectangle(startX, startY + 16, 16, 16), new Rectangle(16, 16, 16, 16), Color.White);
                Game.SpriteBatch.Draw(_circuitTexture, new Rectangle(startX + 16, startY + 16, 16, 16), new Rectangle(16, 16, 16, 16), Color.White, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically, 0f);
                Game.SpriteBatch.Draw(_circuitTexture, new Rectangle(startX + 16, startY + 32, 16, 16), new Rectangle(16, 16, 16, 16), Color.White);
                Game.SpriteBatch.Draw(_circuitTexture, new Rectangle(startX + 32, startY + 32, 16, 16), new Rectangle(16, 16, 16, 16), Color.White, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically, 0f);
                Game.SpriteBatch.Draw(_circuitTexture, new Rectangle(startX + 32, startY + 48, 16, 16), new Rectangle(16, 16, 16, 16), Color.White);
                Game.SpriteBatch.Draw(_circuitTexture, new Rectangle(startX + 48, startY + 48, 16, 16), new Rectangle(0, 0, 16, 16), Color.White);

                for (int i = 0; i < 7; i++)
                    Game.SpriteBatch.Draw(_circuitTexture, new Rectangle(startX + 64 + 16 * i, startY + 48, 16, 16), new Rectangle(16, 0, 16, 16), Color.White);

                Game.SpriteBatch.Draw(_circuitTexture, new Rectangle(startX + 176, startY + 48, 16, 16), new Rectangle(0, 0, 16, 16), Color.White);

                for (int i = 0; i < 7; i++)
                    Game.SpriteBatch.Draw(_circuitTexture, new Rectangle(startX + 192 + 16 * i, startY + 48, 16, 16), new Rectangle(16, 0, 16, 16), Color.White);

                startX = 64;
                startY = 200;

                for (int i = 0; i < 4; i++)
                    Game.SpriteBatch.Draw(_circuitTexture, new Rectangle(startX + 16 * i, startY, 16, 16), new Rectangle(16, 0, 16, 16), Color.White);

                Game.SpriteBatch.Draw(_circuitTexture, new Rectangle(startX + 64, startY, 16, 16), new Rectangle(0, 0, 16, 16), Color.White);

                for (int i = 0; i < 2; i++)
                    Game.SpriteBatch.Draw(_circuitTexture, new Rectangle(startX + 80 + 16 * i, startY, 16, 16), new Rectangle(16, 0, 16, 16), Color.White);

                Game.SpriteBatch.Draw(_circuitTexture, new Rectangle(startX + 112, startY + 16, 16, 16), new Rectangle(16, 16, 16, 16), Color.White);
                Game.SpriteBatch.Draw(_circuitTexture, new Rectangle(startX + 112, startY, 16, 16), new Rectangle(16, 16, 16, 16), Color.White, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically, 0f);

                startX = 200;
                startY = 200;

                for (int i = 0; i < 8; i++)
                    Game.SpriteBatch.Draw(_circuitTexture, new Rectangle(startX + 16 * i, startY, 16, 16), new Rectangle(16, 0, 16, 16), Color.White);

                Game.SpriteBatch.Draw(_circuitTexture, new Rectangle(startX + 128, startY, 16, 16), new Rectangle(16, 16, 16, 16), Color.White, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0f);
                Game.SpriteBatch.Draw(_circuitTexture, new Rectangle(startX + 128, startY - 16, 16, 16), new Rectangle(16, 16, 16, 16), Color.White, 0f, Vector2.Zero, SpriteEffects.FlipVertically, 0f);

                Game.SpriteBatch.Draw(_circuitTexture, new Rectangle(startX + 128 + 16, startY - 16, 16, 16), new Rectangle(16, 16, 16, 16), Color.White, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0f);
                Game.SpriteBatch.Draw(_circuitTexture, new Rectangle(startX + 128 + 16, startY - 32, 16, 16), new Rectangle(16, 16, 16, 16), Color.White, 0f, Vector2.Zero, SpriteEffects.FlipVertically, 0f);

                // draws blue lines in the background:

                _renderer.DrawLine(64, 140, 300, 140, new Color(255, 255, 255, 100));
                _renderer.DrawLine(64, 170, 400, 170, new Color(255, 255, 255, 100));
                _renderer.DrawLine(64, 200, 400, 200, new Color(255, 255, 255, 100));
                _renderer.DrawLine(200, 200, 250, 170, new Color(255, 255, 255, 100));
                _renderer.DrawLine(100, 140, 150, 170, new Color(255, 255, 255, 100));

                // draws the small subtitle:
                Game.SpriteBatch.DrawString(_font, "Holo Tablet", new Vector2(224, 182), Color.White);

                // draws the slowly fading in title:
                int titleLength = (int)MathHelper.Clamp((float)Math.Ceiling((double)_plugin.Title.Length / 100 * _pluginTitleIntro), 0, _plugin.Title.Length);
                Game.SpriteBatch.DrawString(_bigFont, _plugin.Title.Substring(_plugin.Title.Length - titleLength, titleLength), new Vector2(180, 132), Color.White, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
            }
        }

        public void OnUpdate(float elapsedTime)
        {
            if (_introDelay > 0)
            {
                _introDelay--;
            }
            else
            {
                if (_flickerChance > 0)
                    _flickerChance--;

                _sideSlider.Update();

                if (!_closing)
                {
                    UpdateTurnTablet();

                    if (_pluginTitleIntro < 100 && _sideSlider.TargetOffset == _sideSlider.Offset)
                        _pluginTitleIntro += 4;

                    if (Game.InputSystem.Dismiss(DismissInputTypes.BButton | DismissInputTypes.EscapeKey))
                    {
                        _closing = true;
                        _sideSlider.TargetOffset = 0f;
                        _sideSlider.Speed = 0.5f;
                    }
                    else
                    {
                        _plugin.Update();
                    }
                }
                else
                {
                    if (_sideSlider.Offset == _sideSlider.TargetOffset)
                    {
                        Game.ScreenManager.PopScreen();
                    }
                }
            }
        }

        private void UpdateTurnTablet()
        {
            bool hasDirectionalInput = false;
            if (Game.InputSystem.Left(InputDetectionType.HeldDown, DirectionalInputTypes.RightThumbstick))
            {
                if (_cameraX < CAMERA_X_MAX_OFFSET)
                    _cameraX += 0.02f;
                hasDirectionalInput = true;
            }
            if (Game.InputSystem.Right(InputDetectionType.HeldDown, DirectionalInputTypes.RightThumbstick))
            {
                if (_cameraX > -CAMERA_X_MAX_OFFSET)
                    _cameraX -= 0.02f;
                hasDirectionalInput = true;
            }
            if (Game.InputSystem.Down(InputDetectionType.HeldDown, DirectionalInputTypes.RightThumbstick))
            {
                if (_cameraY < CAMERA_Y_MAX_OFFSET)
                    _cameraY += 0.02f;
                hasDirectionalInput = true;
            }
            if (Game.InputSystem.Up(InputDetectionType.HeldDown, DirectionalInputTypes.RightThumbstick))
            {
                if (_cameraY > -CAMERA_Y_MAX_OFFSET)
                    _cameraY -= 0.02f;
                hasDirectionalInput = true;
            }
            if (!hasDirectionalInput)
            {
                if (_cameraY < CAMERA_Y_TARGET)
                {
                    _cameraY += CAMERA_RETURN_SPEED;
                    if (_cameraY >= CAMERA_Y_TARGET)
                        _cameraY = CAMERA_Y_TARGET;
                }
                else if (_cameraY > CAMERA_Y_TARGET)
                {
                    _cameraY -= CAMERA_RETURN_SPEED;
                    if (_cameraY <= CAMERA_Y_TARGET)
                        _cameraY = CAMERA_Y_TARGET;
                }

                if (_cameraX < CAMERA_X_TARGET)
                {
                    _cameraX += CAMERA_RETURN_SPEED;
                    if (_cameraX >= CAMERA_X_TARGET)
                        _cameraX = CAMERA_X_TARGET;
                }
                else if (_cameraX > CAMERA_X_TARGET)
                {
                    _cameraX -= CAMERA_RETURN_SPEED;
                    if (_cameraX <= CAMERA_X_TARGET)
                        _cameraX = CAMERA_X_TARGET;
                }
            }
            else
            {
                _doMouseTurnUpdate = false;
                _lastMousePosition = Game.InputSystem.Mouse.Position;
            }

            if (_doMouseTurnUpdate || _lastMousePosition != Game.InputSystem.Mouse.Position)
            {
                _doMouseTurnUpdate = true;
                _lastMousePosition = Game.InputSystem.Mouse.Position;
                _cameraX = -((_lastMousePosition.X - (float)Game.ScreenBounds.Width / 2) / ((float)Game.ScreenBounds.Width / 2)) * (CAMERA_X_MAX_OFFSET / 4);
                _cameraY = ((_lastMousePosition.Y - (float)Game.ScreenBounds.Height / 2) / ((float)Game.ScreenBounds.Height / 2)) * (CAMERA_Y_MAX_OFFSET / 4);
            }
        }

        public void SetPlugin(TabletPlugin plugin)
        {
            _plugin = plugin;
            _pluginTitleIntro = 0;
        }

        public void OnClosing()
        {
            _target.Dispose();
        }
    }
}
