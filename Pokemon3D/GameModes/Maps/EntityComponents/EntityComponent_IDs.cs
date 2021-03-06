﻿namespace Pokemon3D.GameModes.Maps.EntityComponents
{
    partial class EntityComponent
    {
        /// <summary>
        /// A collection of EntityComponent IDs.
        /// </summary>
        public static class IDs
        {
            // All IDs have to be lower case.
            public const string Billboard = "isbillboard";
            public const string Static = "isstatic";
            public const string Floor = "isfloor";
            public const string AnimateTextures = "animatetextures";

            // collision group:
            public const string Collision = "hascollision";
            public const string CollisionOffset = "collisionoffset";
            public const string CollisionSize = "collisionsize";
        }
    }
}
