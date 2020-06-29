using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.Sound;

namespace TGC.Group.Model
{
    class LaserEnemigo : Colisionable
    {
        private readonly Laser modeloLaser;
        public LaserEnemigo(string mediaDir, TGCVector3 posicionInicial, TGCVector3 direccionDisparo, Nave naveDelJugador): base(naveDelJugador)
        {
            string direccionDeScene = mediaDir + "Xwing\\laser-TgcScene.xml";
            modeloLaser = new Laser(mediaDir, direccionDeScene,posicionInicial,direccionDisparo);
            GameManager.Instance.ReproducirSonido(mediaDir + "laser2.wav",posicionInicial);
        }

        public override void Init()
        {
            modeloLaser.Init();
            this.mainMesh = modeloLaser.GetMainMesh();
        }

        internal override void ColisionarConNave()
        {
            naveDelJugador.ChocarConLaser();
            Destruirse();
        }

        private void Destruirse()
        {
            GameManager.Instance.QuitarRenderizable(this);
            GameManager.Instance.QuitarRenderizable(modeloLaser);
        }

        public override void Update(float elapsedTime)
        {
            if (modeloLaser.SuperoTiempoDeVida(5) || modeloLaser.ColisionaConMapa())
            {
                Destruirse();
            }
            else
            {
                modeloLaser.Update(elapsedTime);
            }
        }

        public override void Render()
        {
            /* //Utilizado Para debug
            if (ColisionaConNave())
            {
                mainMesh.BoundingBox.setRenderColor(Color.Red);
            }
            */
            modeloLaser.Render();
        }

        public override void Dispose() { }

    }
}
