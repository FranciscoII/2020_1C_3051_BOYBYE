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

namespace TGC.Group.Model.Meta
{
    internal class EntornoGameOver : Entorno
    {

        public EntornoGameOver(GameModel gameModel, string mediaDir, InputDelJugador input) : base(gameModel, mediaDir, input)
        {
        }

        public override void Init()
        {
            var posicionInicialDeNave = new TGCVector3(105, -15, 420);
            Nave naveDelJuego = new Nave(mediaDir, posicionInicialDeNave, input);

            GameManager.Instance.AgregarRenderizable(naveDelJuego);
            CamaraDelJuego camaraDelJuego = new CamaraDelJuego(posicionInicialDeNave, 10, -50, naveDelJuego);
            gameModel.CambiarCamara(camaraDelJuego);
        }

        public override void Update(float elapsedTime)
        {
            if (input.HayInputDePausa())
            {
                GameManager.Instance.ReanudarOPausarJuego();
            }

            GameManager.Instance.Update(elapsedTime);

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
