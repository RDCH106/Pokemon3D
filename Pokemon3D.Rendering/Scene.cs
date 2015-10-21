﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon3D.Common;

namespace Pokemon3D.Rendering
{
    /// <summary>
    /// Representing a whole 3D Scene with all objects to display.
    /// </summary>
    public class Scene
    {
        private readonly List<SceneNode> _allNodes;
        private readonly List<Camera> _allCameras; 
        private readonly GraphicsDevice _device;
        private readonly SpriteBatch _spriteBatch;
        private readonly SceneRenderer _sceneRenderer;

        public bool EnableShadows { get; set; }
        public Vector3 LightDirection { get; set; }

        public Scene(GameContext context, SceneEffect effect)
        {
            _device = context.GraphicsDevice;
            _sceneRenderer = new DefaultSceneRenderer(context, effect);
            _allNodes = new List<SceneNode>();
            _allCameras = new List<Camera>();
            LightDirection = new Vector3(1, -1, -1);
            _spriteBatch = new SpriteBatch(_device);
        }

        public SceneNode CreateSceneNode()
        {
            var sceneNode = new SceneNode();
            _allNodes.Add(sceneNode);
            return sceneNode;
        }
        
        public Camera CreateCamera()
        {
            var camera = new Camera(_device.Viewport);
            _allCameras.Add(camera);
            _allNodes.Add(camera);
            return camera;
        }

        public void Update(float elapsedTime)
        {
            foreach (var sceneNode in _allNodes)
            {
                sceneNode.Update();
            }
        }

        public void Draw()
        {
            _sceneRenderer.Draw(_allNodes, _allCameras, LightDirection, EnableShadows);

#if DEBUG_RENDERING
            if (EnableShadows) _sceneRenderer.DrawDebugShadowMap(_spriteBatch, new Rectangle(0,0,128,128));
#endif
        }

        /// <summary>
        /// Clones a Scene node with its children and all attached Properties.
        /// Meshes will only be cloned when <see cref="cloneMeshs"/> is true.
        /// </summary>
        /// <param name="nodeToClone">Node to clone</param>
        /// <param name="cloneMeshs">Force cloning mesh data.</param>
        /// <returns>SceneNode cloned.</returns>
        public SceneNode CloneNode(SceneNode nodeToClone, bool cloneMeshs = false)
        {
            var cloned = nodeToClone.Clone(cloneMeshs);
            _allNodes.Add(cloned);
            CloneChildren(cloned, nodeToClone, cloneMeshs);
            return cloned;
        }

        private void CloneChildren(SceneNode parentCloned, SceneNode parentOriginal, bool cloneMeshs)
        {
            foreach (var childNode in parentOriginal.Children)
            {
                var clonedChild = childNode.Clone(cloneMeshs);
                _allNodes.Add(clonedChild);
                parentCloned.AddChild(clonedChild);

                CloneChildren(clonedChild, childNode, cloneMeshs);
            }
        }
    }
}
