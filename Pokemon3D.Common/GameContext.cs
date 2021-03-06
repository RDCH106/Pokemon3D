﻿using System.Windows.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pokemon3D.Common.Input;
using Pokemon3D.Common.Shapes;

namespace Pokemon3D.Common
{
    public interface GameContext
    {
        InputSystem InputSystem { get; }
        ContentManager Content { get; }
        Rectangle ScreenBounds { get; }
        SpriteBatch SpriteBatch { get; }
        ShapeRenderer ShapeRenderer { get; }
        GraphicsDevice GraphicsDevice { get; }
        Localization.TranslationProvider TranslationProvider { get; }
        string VersionInformation { get; }
        Dispatcher MainThreadDispatcher { get; }
    }
}
