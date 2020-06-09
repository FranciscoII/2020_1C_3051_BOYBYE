using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class HUD
    {
        private String direccionMarco;
        private Drawer2D drawer;
        private CustomSprite marcoVida;
        private CustomSprite marcoGiro;
        private int posicionXBaseDeBarras;
        private int posicionYBarraVida;
        private int posicionYBarraGiro;
        public HUD (string mediaDir)
        {
            direccionMarco = mediaDir + "barra.png";
            drawer = new Drawer2D();
            posicionXBaseDeBarras = Convert.ToInt32(D3DDevice.Instance.Width * 0.015f);
            posicionYBarraVida = Convert.ToInt32(D3DDevice.Instance.Height / 1.35f);
            posicionYBarraGiro = Convert.ToInt32(D3DDevice.Instance.Height / 1.2f);

            int posicionXBaseDeMarcos = Convert.ToInt32(posicionXBaseDeBarras * 0.5f);
            int posicionYDeMarcoVida = Convert.ToInt32(posicionYBarraVida *0.963f);

            marcoVida = new CustomSprite
            {
                Bitmap = new CustomBitmap(direccionMarco, D3DDevice.Instance.Device)
            };
            marcoVida.Position = new TGCVector2(posicionXBaseDeMarcos, posicionYDeMarcoVida);
            marcoVida.Scaling = new TGCVector2(D3DDevice.Instance.Width* 0.00035f, D3DDevice.Instance.Height * 0.00092f);

            int posicionYDeMarcoGiro = Convert.ToInt32(posicionYBarraGiro * 0.963f);

            marcoGiro = new CustomSprite
            {
                Bitmap = new CustomBitmap(direccionMarco, D3DDevice.Instance.Device)
            };

            marcoGiro.Position = new TGCVector2(posicionXBaseDeMarcos, posicionYDeMarcoGiro);
            marcoGiro.Scaling = new TGCVector2(D3DDevice.Instance.Width * 0.00035f, D3DDevice.Instance.Height * 0.00092f);
        }

        private TGCVector2[] PosicionesDeBarra(int cantidadBarra, float posicionYBarra)
        {
            float coeficienteDeLongitudDeBarra = D3DDevice.Instance.Width / 600f; //Dejar el numero expresado como float, sino lo redondea a entero
            float longitudBarra = cantidadBarra * coeficienteDeLongitudDeBarra;

            TGCVector2 posicionBase = new TGCVector2(posicionXBaseDeBarras, posicionYBarra);
            TGCVector2 posicionFinal = new TGCVector2(posicionXBaseDeBarras + longitudBarra, posicionYBarra);

            return new TGCVector2[2] { posicionBase, posicionFinal};
        } 

        private TGCVector2[] PosicionesDeBarraVida(int cantidadVida)
        {
            return PosicionesDeBarra(cantidadVida, posicionYBarraVida);
        }

        private TGCVector2[] PosicionesDeBarraGiro(int cantidadGiro)
        {
            return PosicionesDeBarra(cantidadGiro, posicionYBarraGiro);
        }

        public void Render(int cantidadVida, int cantidadGiro)
        {

            DibujarBarra(PosicionesDeBarraVida(cantidadVida), Color.Red);
            DibujarBarra(PosicionesDeBarraGiro(cantidadGiro), Color.Blue);
            drawer.BeginDrawSprite();
            drawer.DrawSprite(marcoVida);
            drawer.DrawSprite(marcoGiro);
            drawer.EndDrawSprite();
        }

        private void DibujarBarra(TGCVector2[] posiciones, Color color)
        {
            Line barra = new Line(D3DDevice.Instance.Device)
            {
                Antialias = true,
                Width = Convert.ToInt32(0.035f * D3DDevice.Instance.Height),
            };
            barra.Draw(TGCVector2.ToVector2Array(posiciones), color);


        }

    }
}
