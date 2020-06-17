using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Collision;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Shaders;

namespace TGC.Group.Model
{
    class Bloque:IRenderizable
    {
        private readonly string mediaDir;
        private readonly TGCVector3 posicionInicial;

        private TGCMatrix ScaleMatrix;
        private TGCMatrix TranslationMatrix;
        private TgcScene Scene;
        private String nombreMapa;
        private List<TGCVector3> posicionesTorretas;
        private Nave nave;
        private List<ObstaculoMapa> listaObstaculos;
        private Effect effect;

        public Bloque(string mediaDir, TGCVector3 posicionInicial,String nombreMapa,List<TGCVector3> posiciones,Nave nave)
        {
            this.mediaDir = mediaDir;
            this.posicionInicial = posicionInicial;
            this.nombreMapa = nombreMapa;
            this.posicionesTorretas = posiciones;
            this.nave = nave;
            this.listaObstaculos = new List<ObstaculoMapa>();
        }
        public void Init()
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            Scene = new TgcSceneLoader().loadSceneFromFile(mediaDir + nombreMapa);
            ScaleMatrix = TGCMatrix.Scaling(20f, 20f, 20f);
            TranslationMatrix = TGCMatrix.Translation(posicionInicial);
            instanciarObstaculosMapa();
            instanciarTorretas();
            effect = TGCShaders.Instance.LoadEffect("..\\..\\Shaders\\" + "Fran.fx");
            Scene.Meshes.ForEach(delegate (TgcMesh mesh) {
                mesh.Transform = ScaleMatrix * TranslationMatrix;
                mesh.BoundingBox.transform(ScaleMatrix * TranslationMatrix);
                mesh.Effect = effect; mesh.Technique = "Borrado";
            });
        }

        private void instanciarTorretas()
        {
            this.posicionesTorretas.
                ForEach(delegate (TGCVector3 posicion) {
                    Torreta torreta = new Torreta(mediaDir,posicion,nave);
                    GameManager.Instance.AgregarRenderizable(torreta); 
                });
        }
        private void instanciarObstaculosMapa()
        {
            for (int i = 3; i < Scene.Meshes.Count; i++)//hardcodeado esto deberia cambiar
            {
                ObstaculoMapa obstaculo = new ObstaculoMapa(nave, Scene.Meshes[i]);
                listaObstaculos.Add(obstaculo);
                GameManager.Instance.AgregarRenderizable(obstaculo);
            }
        }

        public void Update(float elapsedTime)
        {

            if (nave.GetPosicion().Z > posicionInicial.Z + 3000f) //este numero deberia corrseponder idealmente al tamaño del bloque
            {
                for (int i = 0; i < listaObstaculos.Count; i++)
                    GameManager.Instance.QuitarRenderizable(listaObstaculos[i]);

                GameManager.Instance.QuitarRenderizable(this);
                //Logger.Loggear("Se borro el bloque de posicion: " + this.posicionInicial.ToString() + " nombre: " + this.nombreMapa);
            }
        }

        public void Render()
        {
            //Scene.RenderAll();
            Scene.Meshes.ForEach(delegate (TgcMesh mesh) {
                if (GameManager.Instance.esVisibleParaLaCamara(mesh))
                {
                    mesh.Effect.SetValue("posicionNave", TGCVector3.TGCVector3ToFloat3Array(nave.GetPosicion()));
                    mesh.Render();
                }
            });


        }
        public void Dispose()
        {
            Scene.DisposeAll();
        }
    }
}
