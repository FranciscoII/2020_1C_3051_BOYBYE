using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualC;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class BloqueBuilder
    {
        private String mediaDir;
        private TGCVector3 posicion;
        private String nombreMapa;
        private List<TGCVector3> posicionOriginalTorretas;
        private List<TGCVector3> posicionesTorretas;
        private Nave nave;
        public BloqueBuilder(string mediaDir, TGCVector3 posicionInicial, String nombreMapa, List<TGCVector3> posiciones, Nave nave)
        {
            this.mediaDir = mediaDir;
            this.posicion = posicionInicial;
            this.nombreMapa = nombreMapa;
            this.posicionesTorretas = posiciones;
            this.posicionOriginalTorretas = posiciones;
            this.nave = nave;
        }

        public Bloque generarBloque()
        {
            return new Bloque(mediaDir, posicion, nombreMapa, posicionesTorretas, nave);
        }

        public void setPosicion(TGCVector3 nuevaPosicion)
        {
            this.posicion = nuevaPosicion;
            List<TGCVector3> nuevasPosiciones = new List<TGCVector3>();
            float nuevoZ;
            //posicionesTorretas.ForEach(delegate (TGCVector3 pos) {pos.Z += nuevaPosicion.Z;nuevasPosiciones.Add(new TGCVector3(pos)); });
            for(int i = 0; i < posicionesTorretas.Count; i++)
            {
                nuevoZ = nuevaPosicion.Z + posicionOriginalTorretas[i].Z - 1000f;
                nuevasPosiciones.Add(new TGCVector3(posicionesTorretas[i].X, posicionesTorretas[i].Y,nuevoZ));
            }
            this.posicionesTorretas = nuevasPosiciones;
        }
        public TGCVector3 getPosicion()
        {
            return this.posicion;
        }

    }
}
