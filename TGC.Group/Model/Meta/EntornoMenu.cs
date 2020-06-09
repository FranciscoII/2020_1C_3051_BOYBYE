using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Camara;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Meta
{
    internal class EntornoMenu : Entorno
    {
        private TgcCamera camaraDeMenu;

        public EntornoMenu(GameModel gameModel, string mediaDir, InputDelJugador input) : base(gameModel, mediaDir, input)
        {
        }

        public override void Init()
        {
            camaraDeMenu = new TgcCamera();
            camaraDeMenu.SetCamera(new TGCVector3(105, -15, -250), new TGCVector3(105, -15, -249));
            gameModel.CambiarCamara(camaraDeMenu);
        }

        public override void Render()
        {
            GameManager.Instance.Render();
        }

        public override void Update(float elapsedTime)
        {
            if (input.HayInputDePausa())
            {
                CambiarEntorno(new EntornoJuego(gameModel, mediaDir, input));
            }
            else
            {
                camaraDeMenu.UpdateCamera(elapsedTime);
                GameManager.Instance.Update(elapsedTime);
            }

        }
        public override void Dispose()
        {
            GameManager.Instance.Dispose();
        }
    }

}