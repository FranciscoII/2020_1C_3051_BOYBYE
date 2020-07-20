using TGC.Core.Mathematica;
using TGC.Group.Model;

namespace TGC.Examples.Camara
{
    class CamaraJugadorFija : TgcThirdPersonCamera
    {
        private readonly Nave NaveDelJuego;

        public CamaraJugadorFija(TGCVector3 target, float offsetHeight, float offsetForward, Nave nave) : base(target, offsetHeight, offsetForward)
        {
            this.NaveDelJuego = nave;
        }

        internal virtual void SeguirNaveParaAdelante()
        {
            float nuevaPosicionEnZ = NaveDelJuego.GetPosicion().Z;
            TGCVector3 nuevoTarget = new TGCVector3(Target.X, Target.Y, nuevaPosicionEnZ);
            Target = nuevoTarget;
        }


        public override void UpdateCamera(float elapsedTime)
        {
            SeguirNaveParaAdelante();
            base.UpdateCamera(elapsedTime);

        }
    }
}
