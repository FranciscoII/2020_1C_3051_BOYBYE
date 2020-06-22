using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.KeyFrameLoader;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class NaveGameOver : IRenderizable
    {
        protected readonly ModeloCompuesto modeloNave;
        protected TGCVector3 posicion;
        protected float timer;
        private bool iniciarAnimacion;

        public NaveGameOver(string mediaDir, TGCVector3 posicionInicial)
        {
            this.modeloNave = new ModeloCompuesto(mediaDir + "XWing\\X-Wing-TgcScene.xml", posicionInicial);
            posicion = posicionInicial;
            timer = 0;
            iniciarAnimacion = false;
        }

        public void Dispose()
        {
            modeloNave.Dispose();
        }

        public void Init()
        {
            modeloNave.CambiarShader("NaveGameOver");
            modeloNave.CambiarRotacion(new TGCVector3(FastMath.ToRad(180), 0,0));
            modeloNave.CambiarEscala(new TGCVector3(0.8f, 0.8f, 0.8f));
            modeloNave.AplicarTransformaciones();
        }

        public void Render()
        {
            modeloNave.Render();
        }

        protected void MoverseEnDireccion(TGCVector3 versorDirector, float elapsedTime)
        {
            TGCVector3 movimientoDelFrame = elapsedTime * versorDirector * 3;

            posicion += movimientoDelFrame;

            modeloNave.CambiarPosicion(posicion);
        }

        public void Update(float elapsedTime)
        {
            var noRealizoGiro = iniciarAnimacion && FastMath.ToRad(180) - timer * 2 > 0;
            var noSubioLoSuficiente = iniciarAnimacion && posicion.Y <= 15;
            if (noRealizoGiro)
            {
                timer += elapsedTime;
                modeloNave.CambiarRotacion(new TGCVector3(FastMath.ToRad(180) - timer * 2, 0, 0));
                modeloNave.CambiarEscala(new TGCVector3(0.8f, 0.8f, 0.8f));
                
            }
            if (noSubioLoSuficiente)
            {
                MoverseEnDireccion(new TGCVector3(0, 1, 0), elapsedTime * 2.4f);
            }
            if (!noRealizoGiro && !noSubioLoSuficiente && iniciarAnimacion)
            {
                MoverseEnDireccion(new TGCVector3(1, 0, 0), elapsedTime * 40);
            }


            modeloNave.AplicarTransformaciones();
            updateShader();
        }
        public void updateShader()
        {
            modeloNave.UpdateShader(GameManager.Instance.PosicionSol, GameManager.Instance.EyePosition(),timer*1.3f);
        }
        public void IniciarAnimacion()
        {
            iniciarAnimacion = true;
        }
        public TGCVector3 GetPosicion()
        {
            return posicion;
        }
    }
}
