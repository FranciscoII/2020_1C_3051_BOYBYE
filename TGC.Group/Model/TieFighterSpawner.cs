using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class TieFighterSpawner
    {
        private String mediaDir;
        private Nave nave;
        private float tiempoTranscurrido;
        private float tiempoSiguienteSpawn;
        public List<TGCVector2> posicionesSpawn { get; set; }

        public TieFighterSpawner(String mediaDir, Nave nave)
        {
            this.mediaDir = mediaDir;
            this.nave = nave;
            this.tiempoSiguienteSpawn = 2.5f;
            this.tiempoTranscurrido = 0f;
            this.posicionesSpawn = new List<TGCVector2>();
            posicionesSpawn.Add(new TGCVector2(90, -3));
            posicionesSpawn.Add(new TGCVector2(90, -20));
            posicionesSpawn.Add(new TGCVector2(116, -3));
            posicionesSpawn.Add(new TGCVector2(116, -20));
        }

        public void Update(float elapsedTime)
        {
            if (GameManager.Instance.estaPausado)
                return;
            Random rnd = new Random();
            tiempoTranscurrido += elapsedTime;
            if (tiempoTranscurrido > tiempoSiguienteSpawn && posicionesSpawn.Count > 0)
            {
                tiempoTranscurrido = 0;
                SpawnTieFighter();
                tiempoSiguienteSpawn = rnd.Next(4, 16);
            }
        }
        private void SpawnTieFighter()
        {
            Random random = new Random();
            int index = random.Next(posicionesSpawn.Count);
            TGCVector2 posicionRandom = posicionesSpawn[index];

            TGCVector3 posicionTie = TGCVector3.Empty;
            posicionTie.X = posicionRandom.X;
            posicionTie.Y = posicionRandom.Y;
            posicionTie.Z = nave.GetPosicion().Z + 550f;

            TieFighter tieFighter = new TieFighter(mediaDir, posicionTie, nave, this);
            posicionesSpawn.Remove(posicionRandom);
            GameManager.Instance.AgregarRenderizable(tieFighter);
        }
        public void AgregarPosicionLibre(TGCVector2 posicionLibre)
        {
            posicionesSpawn.Add(posicionLibre); //No funciona pero no importa.
            //Solo spawnearan 4 tie fighters
        }
    }
}