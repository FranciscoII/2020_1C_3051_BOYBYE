using TGC.Core.Mathematica;
using TGC.Examples.Camara;
using TGC.Group.Model;

namespace TGC.Group.Model.NaveJugador
{
    class CamaraJugadorDinamica : CamaraJugadorFija
    {
        private readonly Nave NaveDelJuego;

        public CamaraJugadorDinamica(TGCVector3 target, float offsetHeight, float offsetForward, Nave nave) : base(target, offsetHeight, offsetForward, nave)
        {
            this.NaveDelJuego = nave;
        }

        override internal void SeguirNaveParaAdelante()
        {
            TGCVector3 nuevoTarget = NaveDelJuego.GetPosicion();
            Target = nuevoTarget;
        }
    }
}
