using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Examples.Camara;
using TGC.Group.Model.Clases2D;

namespace TGC.Group.Model.Meta
{
    internal class EntornoJuego : Entorno
    {
        private EscenarioLoader escenarioLoader;
        private TieFighterSpawner tieFighterSpawner;
        

        public EntornoJuego(GameModel gameModel, string mediaDir, InputDelJugador input) : base(gameModel, mediaDir, input)
        {
        }

        public override void Init()
        {
            var posicionInicialDeNave = new TGCVector3(105, -15, -250);
            Nave naveDelJuego = new Nave(mediaDir, posicionInicialDeNave, input);
            //Debe empezar pausado
            GameManager.Instance.PausarJuego();
            
            GameManager.Instance.AgregarRenderizable(naveDelJuego);
            CamaraDelJuego camaraDelJuego = new CamaraDelJuego(posicionInicialDeNave, 10, -50, naveDelJuego);
            gameModel.CambiarCamara(camaraDelJuego);
            Skybox skybox = new Skybox(mediaDir, camaraDelJuego);
            GameManager.Instance.AgregarRenderizable(skybox);
            escenarioLoader = new EscenarioLoader(mediaDir, naveDelJuego);
            tieFighterSpawner = new TieFighterSpawner(mediaDir, naveDelJuego);
            
        }

        public override void Update(float elapsedTime)
        {
            if (input.HayInputDePausa())
                GameManager.Instance.ReanudarOPausarJuego();

            GameManager.Instance.Update(elapsedTime);
            escenarioLoader.Update(elapsedTime);
            tieFighterSpawner.Update(elapsedTime);
            
        }

        public override void Render()
        {
            GameManager.Instance.Render();
        }
        public override void Dispose()
        {
            GameManager.Instance.Dispose();
        }
    }
}
