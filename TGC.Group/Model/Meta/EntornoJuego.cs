using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.Text;
using TGC.Examples.Camara;
using TGC.Group.Model.Clases2D;

namespace TGC.Group.Model.Meta
{
    internal class EntornoJuego : Entorno
    {
        private EscenarioLoader escenarioLoader;
        private TieFighterSpawner tieFighterSpawner;
        private Nave naveDelJuego;

        public EntornoJuego(GameModel gameModel, string mediaDir, InputDelJugador input) : base(gameModel, mediaDir, input)
        {
        }

        public override void Init()
        {
            var posicionInicialDeNave = new TGCVector3(105, -15, 420);
            naveDelJuego = new Nave(mediaDir, posicionInicialDeNave, input);
            //Debe empezar pausado
            //GameManager.Instance.PausarJuego();
            
            GameManager.Instance.AgregarRenderizable(naveDelJuego);
            CamaraDelJuego camaraDelJuego = new CamaraDelJuego(posicionInicialDeNave, 10, -50, naveDelJuego);
            gameModel.CambiarCamara(camaraDelJuego);
            Skybox skybox = new Skybox(mediaDir, camaraDelJuego);
            GameManager.Instance.AgregarRenderizable(skybox);
            escenarioLoader = new EscenarioLoader(mediaDir, naveDelJuego);
            tieFighterSpawner = new TieFighterSpawner(mediaDir, naveDelJuego);
            GameManager.Instance.ReanudarOPausarJuego();
        }

        public override void Update(float elapsedTime)
        {
            if (input.HayInputDePausa())
            {
                GameManager.Instance.ReanudarOPausarJuego();
            }

            GameManager.Instance.Update(elapsedTime);
            escenarioLoader.Update(elapsedTime);
            tieFighterSpawner.Update(elapsedTime);
            if (!naveDelJuego.estaVivo)
                CambiarEntorno(new EntornoGameOver(gameModel, mediaDir, input));
        }

        public override void Render()
        {
            if(GameManager.Instance.estaPausado)
            {
                string textoControles = "Controles:\nWASD: Moverse\nQ: Rollear\nE: Voltearse\nShift: Acelerar\nCtrl: Desacelerar\nEnter: Pausar/Despausar";
                TgcText2D textoDrawer = new TgcText2D();
                textoDrawer.Text = textoControles;
                textoDrawer.changeFont(new Font("Calibri", 0.009765625f* D3DDevice.Instance.Width));
                textoDrawer.drawText(textoControles, D3DDevice.Instance.Width/40, D3DDevice.Instance.Height / 20, Color.White);
            }

            GameManager.Instance.Render();
        }
        public override void Dispose()
        {
            GameManager.Instance.Dispose();
        }
        public Nave GetNave()
        {
            return naveDelJuego;
        }
    }
}
