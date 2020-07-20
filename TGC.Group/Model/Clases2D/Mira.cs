using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Clases2D
{
    class Mira
    {
        private Drawer2D drawer;
        private CustomSprite spriteMira;
        public Mira (string mediaDir)
        {
            drawer = new Drawer2D();
            spriteMira = new CustomSprite
            {
                Bitmap = new CustomBitmap(mediaDir + "mira.png", D3DDevice.Instance.Device)
            };

            spriteMira.Position = new TGCVector2(D3DDevice.Instance.Width/2, D3DDevice.Instance.Height/2);
            spriteMira.Scaling = new TGCVector2(D3DDevice.Instance.Width * 0.00035f, D3DDevice.Instance.Height * 0.00092f);
        }

        public void Render()
        {
            drawer.BeginDrawSprite();
            drawer.DrawSprite(spriteMira);
            drawer.EndDrawSprite();
        }
    }
}
