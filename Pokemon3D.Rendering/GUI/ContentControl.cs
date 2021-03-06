﻿using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Xml;

namespace Pokemon3D.Rendering.GUI
{
    public abstract class ContentControl : GuiElement
    {
        public GuiElement Child
        {
            get { return Children.FirstOrDefault(); } 
            set 
            {
                if (value == null)
                {
                    Children.Clear();
                }
                else
                {
                    if (Children.Count == 0)
                    {
                        Children.Add(value);
                    }
                    else
                    {
                        Children[0] = value;
                    }
                }
            }
        }

        protected ContentControl(GuiSystem guiSystem) : base(guiSystem)
        {
        }

        protected ContentControl(GuiSystem guiSystem, XmlElement element) : base(guiSystem, element)
        {
            if (element.HasChildNodes)
            {
                Child = guiSystem.CreateFromXmlType((XmlElement)element.FirstChild);
            }
        }

        public override void Update(float elapsedTime)
        {
            Child?.Update(elapsedTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Child?.Draw(spriteBatch);
        }
    }
}
