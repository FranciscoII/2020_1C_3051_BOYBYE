using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model.Meta
{
    public abstract class Entorno
    {
        internal GameModel gameModel;
        internal string mediaDir;
        internal InputDelJugador input;

        public Entorno(GameModel gameModel,string mediaDir, InputDelJugador input)
        {
            this.gameModel = gameModel;
            this.mediaDir = mediaDir;
            this.input = input;
        }

        abstract public void Init();
        abstract public void Update(float elapsedTime);
        abstract public void  Render();
        abstract public void Dispose();
        private void CambiarEntorno(Entorno nuevoEntorno)
        {
            this.Dispose();
            gameModel.EntornoActual = nuevoEntorno;
        }


    }
}
