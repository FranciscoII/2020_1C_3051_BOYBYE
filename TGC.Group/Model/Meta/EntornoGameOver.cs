using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Camara;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.Text;
using TGC.Examples.Camara;
using TGC.Group.Model.Clases2D;

namespace TGC.Group.Model.Meta
{
    internal class EntornoGameOver : Entorno
    {
        private MenuGameOver menuGameOver;
        private TgcCamera camaraDeMenu;
        public EntornoGameOver(GameModel gameModel, string mediaDir, InputDelJugador input) : base(gameModel, mediaDir, input)
        {
            menuGameOver = new MenuGameOver(mediaDir);
            camaraDeMenu = new TgcCamera();
            camaraDeMenu.SetCamera(new TGCVector3(0, 20, 250), new TGCVector3(0, 20, 0));
        }

        public override void Init()
        {
            var posicionInicialDeNave = new TGCVector3(105, -15, 420);
            Nave naveDelJuego = new Nave(mediaDir, posicionInicialDeNave, input);

            GameManager.Instance.AgregarRenderizable(naveDelJuego);

            gameModel.CambiarCamara(camaraDeMenu);
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
            menuGameOver.DibujarMenu();

        }
        public override void Dispose()
        {
            GameManager.Instance.Dispose();
        }
    }
}
