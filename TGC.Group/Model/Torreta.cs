﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Text;

namespace TGC.Group.Model
{
    class Torreta : IRenderizable
    {
        private readonly string mediaDir;
        private readonly TGCVector3 posicionInicial;
        private TgcMesh mainMesh;

        private TGCMatrix baseScaleRotation;
        private TGCMatrix baseQuaternionTranslation;
        private float giro = 0f;
        private Nave jugador;
        private float anguloEntreVectores;
        private TGCQuaternion quaternionAuxiliar;
        public Torreta(string mediaDir, TGCVector3 posicionInicial,Nave nave)
        {
            this.mediaDir = mediaDir;
            this.posicionInicial = posicionInicial;
            this.jugador = nave;

        }
        public void Init()
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene2 = loader.loadSceneFromFile(mediaDir + "Xwing\\Turbolaser-TgcScene.xml");

            //Solo nos interesa el primer modelo de esta escena (tiene solo uno)
            mainMesh = scene2.Meshes[0];
            mainMesh.Position = posicionInicial;
            baseQuaternionTranslation = TGCMatrix.Translation(new TGCVector3(0.0f, 0.01f, 0.0f));
            baseScaleRotation = TGCMatrix.Scaling(new TGCVector3(0.15f, 0.15f, 0.15f));
            mainMesh.Transform = TGCMatrix.Scaling(0.1f, 0.1f, 0.1f);
        }

        public void Update(float elapsedTime)
        {
            TGCQuaternion rotationX = TGCQuaternion.RotationAxis(new TGCVector3(0.0f, 1.0f, 0.0f), Geometry.DegreeToRadian(90f/* + anguloEntreVectores*15*/));
            TGCVector3 PosicionA = posicionInicial;
            TGCVector3 PosicionB = jugador.GetPosicion();
            TGCVector3 DireccionA = new TGCVector3(0, 0, -1);
            TGCVector3 DireccionB = PosicionB - PosicionA;
            if (DireccionB.Length() >= 15f && PosicionA.Z > PosicionB.Z)
            {
                DireccionB.Normalize();
                // anguloEntreVectores = (float)Math.Acos(TGCVector3.Dot(DireccionA, DireccionB));

                var cross = TGCVector3.Cross(DireccionA, DireccionB);
                var newRotation = TGCQuaternion.RotationAxis(cross, FastMath.Acos(TGCVector3.Dot(DireccionA, DireccionB)));
                quaternionAuxiliar = rotationX * newRotation;
                mainMesh.Transform = baseScaleRotation *
                   TGCMatrix.RotationTGCQuaternion(rotationX * newRotation) *
                   baseQuaternionTranslation;
            }
            else
            {
                mainMesh.Transform = baseScaleRotation *
                        TGCMatrix.RotationTGCQuaternion(quaternionAuxiliar) *
                        baseQuaternionTranslation;
            }
           // Disparar(DireccionB);
        }

        public void Render()
        {
            mainMesh.Render();
            TGCVector3 PosicionB = jugador.GetPosicion();
            TGCVector3 DireccionA = new TGCVector3(0, 0, -1);
            TGCVector3 DireccionB = PosicionB - posicionInicial;
            bool dada = posicionInicial.Z > PosicionB.Z;
            new TgcText2D().drawText("Distancia: " + DireccionB.Length().ToString(), 5, 20, Color.White);
            new TgcText2D().drawText("\n Condicion: " + dada.ToString(), 5, 20, Color.White);

        }
        public void Dispose()
        {
            mainMesh.Dispose();
        }

        public void Disparar(TGCVector3 VectorDireccionDisparo)
        {
            var direccionDisparo = VectorDireccionDisparo;
            direccionDisparo.Z += 40;
            TGCVector3 posicionLaser = posicionInicial;
            posicionLaser.Y += 10;
            GameManager.Instance.AgregarRenderizable(new Laser(mediaDir,posicionLaser,direccionDisparo));
        }



    }
}
