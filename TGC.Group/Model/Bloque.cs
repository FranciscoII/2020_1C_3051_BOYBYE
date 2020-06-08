﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Collision;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

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

        public Bloque(string mediaDir, TGCVector3 posicionInicial,String nombreMapa,List<TGCVector3> posiciones,Nave nave)
        {
            this.mediaDir = mediaDir;
            this.posicionInicial = posicionInicial;
            this.nombreMapa = nombreMapa;
            this.posicionesTorretas = posiciones;
            this.nave = nave;
        }
        public void Init()
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            Scene = new TgcSceneLoader().loadSceneFromFile(mediaDir + nombreMapa);
            ScaleMatrix = TGCMatrix.Scaling(20f, 20f, 20f);
            TranslationMatrix = TGCMatrix.Translation(posicionInicial);
            instanciarObstaculosMapa();
            instanciarTorretas();
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
                var obstaculo = new ObstaculoMapa(nave, Scene.Meshes[i]);
                GameManager.Instance.AgregarRenderizable(obstaculo);
            }
        }

        public void Update(float elapsedTime)
        {
            Scene.Meshes.ForEach(delegate (TgcMesh mesh) { 
                mesh.Transform = ScaleMatrix*TranslationMatrix;
                mesh.BoundingBox.transform(ScaleMatrix * TranslationMatrix);
            });
        }

        public void Render()
        {
            //Scene.RenderAll();
            Scene.Meshes.ForEach(delegate (TgcMesh mesh) {
                var result = TgcCollisionUtils.classifyFrustumAABB(GameManager.Instance.Frustum, mesh.BoundingBox);
                if (result != TgcCollisionUtils.FrustumResult.OUTSIDE)
                {
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
