using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Clases2D
{
    class SpriteGodMode
    {
        private Drawer2D drawer;
        private CustomSprite sprite;
        public SpriteGodMode (string mediaDir)
        {
            drawer = new Drawer2D();
            sprite = new CustomSprite
            {
                Bitmap = new CustomBitmap(mediaDir + "godmode.png", D3DDevice.Instance.Device)
            };

            sprite.Position = new TGCVector2(D3DDevice.Instance.Width * 0.015f, D3DDevice.Instance.Height / 1.55f);
            sprite.Scaling = new TGCVector2(D3DDevice.Instance.Width * 0.00035f, D3DDevice.Instance.Height * 0.00092f);
        }

        public void Render()
        {
            drawer.BeginDrawSprite();
            drawer.DrawSprite(sprite);
            drawer.EndDrawSprite();
        }
    }
}
