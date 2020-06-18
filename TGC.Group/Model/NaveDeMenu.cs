using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.KeyFrameLoader;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class NaveDeMenu : IRenderizable
    {
        private readonly ModeloCompuesto modeloNave;
        private TGCVector3 posicion;
        private float timer; //Mal nombreeee

        public NaveDeMenu(string mediaDir, TGCVector3 posicionInicial)
        {
            this.modeloNave = new ModeloCompuesto(mediaDir + "XWing\\X-Wing-TgcScene.xml", posicionInicial);
            posicion = posicionInicial;
            timer = 0;
        }

        public void Dispose()
        {
            modeloNave.Dispose();
        }

        public void Init()
        {
        }

        public void Render()
        {
            modeloNave.Render();
        }

        private void MoverseEnDireccion(TGCVector3 versorDirector, float elapsedTime)
        {
            TGCVector3 movimientoDelFrame = elapsedTime * versorDirector*4;

            posicion += movimientoDelFrame;

            modeloNave.CambiarPosicion(posicion);
        }

        public void Update(float elapsedTime)
        {
            timer += elapsedTime;
            MoverseEnDireccion(new TGCVector3(0,Math.Sign(Math.Sin(timer*2)),0),elapsedTime);
            updateShader();
        }
        public void updateShader()
        {
            modeloNave.UpdateShader(GameManager.Instance.PosicionSol, GameManager.Instance.EyePosition());
        }
    }
}
