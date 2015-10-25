﻿using Microsoft.Xna.Framework.Graphics;

namespace Pokémon3D.UI.Transitions
{
    interface ScreenTransition
    {
        void StartTransition(Texture2D source, Texture2D target);

        bool IsFinished { get; }

        void Update(float elapsedTimeSeconds);

        void Draw();
    }
}
