﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pokemon3D.Rendering
{
    /// <summary>
    /// Specialized Scene Node representing a camera.
    /// </summary>
    public class Camera : SceneNode
    {
        public float NearClipDistance { get; set; }
        public float FarClipDistance { get; set; }
        public float FieldOfView { get; set; }
        public Viewport Viewport { get; set; }

        public Matrix ViewMatrix { get; private set; }
        public Matrix ProjectionMatrix { get; private set; }
        public BoundingFrustum Frustum { get; }

        public Color? ClearColor { get; set; }
        public Skybox Skybox { get; set; }
        
        internal Camera(Viewport viewport): base(false, null)
        {
            Viewport = viewport;
            NearClipDistance = 0.1f;
            FarClipDistance = 1000.0f;
            FieldOfView = MathHelper.PiOver4;
            Frustum = new BoundingFrustum(Matrix.Identity);
            ClearColor = Color.CornflowerBlue;
        }

        internal void OnViewSizeChanged(Rectangle oldSize, Rectangle newSize)
        {
            Viewport = new Viewport(0,0,newSize.Width, newSize.Height);
        }

        protected override void HandleIsDirty()
        {
            base.HandleIsDirty();
            ViewMatrix = Matrix.Invert(GetWorldMatrix(0.0f));
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(FieldOfView, Viewport.AspectRatio, NearClipDistance, FarClipDistance);
            Frustum.Matrix = ViewMatrix*ProjectionMatrix;
        }
    }
}
