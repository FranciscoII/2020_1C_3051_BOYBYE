using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Clases2D
{
    class MenuPrincipal
    {
        private Drawer2D drawer;
        private CustomSprite menuPrincipalSprite;
        private CustomSprite textoEnterSprite;
        public MenuPrincipal(String mediaDir)
        {
            drawer = new Drawer2D();
            menuPrincipalSprite = new CustomSprite
            {
                Bitmap = new CustomBitmap(mediaDir + "logo.png", D3DDevice.Instance.Device),
                Position = new TGCVector2(D3DDevice.Instance.Width * 0.4f, D3DDevice.Instance.Height * 0.05f),
                Scaling = new TGCVector2(0.000195f* D3DDevice.Instance.Width, 0.00092f* D3DDevice.Instance.Height),
            };

            textoEnterSprite = new CustomSprite
            {
                Bitmap = new CustomBitmap(mediaDir + "textoEnter.png", D3DDevice.Instance.Device),
                Position = new TGCVector2(D3DDevice.Instance.Width * 0.3f, D3DDevice.Instance.Height * 0.8f),
                Scaling = new TGCVector2(0.0001953125f* D3DDevice.Instance.Width, 0.0004625f * D3DDevice.Instance.Height)
            };
        }
        public void DibujarMenu()
        {
            drawer.BeginDrawSprite();
            drawer.DrawSprite(menuPrincipalSprite);
            drawer.DrawSprite(textoEnterSprite);
            drawer.EndDrawSprite();
        }


        public void Dispose()
        {
            menuPrincipalSprite.Dispose();
            textoEnterSprite.Dispose();
        }
    }
}
