using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class LaserDecorativo : Laser

    {
        public LaserDecorativo(string mediaDir,string direccionDeScene, TGCVector3 posicionInicial, TGCVector3 direccion):base(mediaDir,direccionDeScene,posicionInicial,direccion)
        {
        }
        public override void Update(float elapsedTime)
        {
            if (SuperoTiempoDeVida(2f))
            {
                Borrarse();
            }
            else
            {
                base.Update(elapsedTime);
            }
            
        }
        private void Borrarse()
        {
            GameManager.Instance.QuitarRenderizable(this);
        }
    }
}
