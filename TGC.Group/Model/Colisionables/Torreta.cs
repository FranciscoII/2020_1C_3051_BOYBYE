﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using Microsoft.DirectX.Direct3D;
using TGC.Core.BoundingVolumes;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Text;

namespace TGC.Group.Model
{
    class Torreta : Destruible
    {
        private readonly string mediaDir;
        private readonly TGCVector3 posicionInicial;
        private TGCMatrix baseScaleRotation;
        private TGCMatrix baseQuaternionTranslation;
        private float tiempo = 0f;
        private TGCQuaternion quaternionAuxiliar;

        public Torreta(string mediaDir, TGCVector3 posicionInicial,Nave nave) : base(nave)
        {
            this.mediaDir = mediaDir;
            this.posicionInicial = posicionInicial;

        }
        public override void Init()
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene2 = loader.loadSceneFromFile(mediaDir + "Xwing\\Turbolaser-TgcScene.xml");

            //Solo nos interesa el primer modelo de esta escena (tiene solo uno)
            mainMesh = scene2.Meshes[0];
            mainMesh.Position = posicionInicial;
            baseQuaternionTranslation = TGCMatrix.Translation(posicionInicial);
            baseScaleRotation = TGCMatrix.Scaling(new TGCVector3(0.14f, 0.16f, 0.14f));
            mainMesh.Transform = TGCMatrix.Scaling(0.1f, 0.1f, 0.1f);
        }

        public override void Update(float elapsedTime)
        {
            if (GameManager.Instance.Pause)
                return;
            base.Update(elapsedTime);
            TGCMatrix matrizTransformacion;
            TGCQuaternion rotationX = TGCQuaternion.RotationAxis(new TGCVector3(0.0f, 1.0f, 0.0f), Geometry.DegreeToRadian(90f/* + anguloEntreVectores*15*/));
            TGCVector3 PosicionA = posicionInicial;
            TGCVector3 PosicionB = naveDelJugador.GetPosicion();
            TGCVector3 DireccionA = new TGCVector3(0, 0, -1);
            TGCVector3 DireccionB = PosicionB - PosicionA;
            if (DireccionB.Length() >= 15f && PosicionA.Z > PosicionB.Z + 10f)
            {
                DireccionB.Normalize();
                // anguloEntreVectores = (float)Math.Acos(TGCVector3.Dot(DireccionA, DireccionB));

                var cross = TGCVector3.Cross(DireccionA, DireccionB);
                var newRotation = TGCQuaternion.RotationAxis(cross, FastMath.Acos(TGCVector3.Dot(DireccionA, DireccionB)));
                quaternionAuxiliar = rotationX * newRotation;
                matrizTransformacion = baseScaleRotation *
                   TGCMatrix.RotationTGCQuaternion(rotationX * newRotation) *
                   baseQuaternionTranslation;
            }
            else
            {
                matrizTransformacion = baseScaleRotation *
                        TGCMatrix.RotationTGCQuaternion(quaternionAuxiliar) *
                        baseQuaternionTranslation;
            }
            mainMesh.Transform = matrizTransformacion;
            mainMesh.BoundingBox.transform(matrizTransformacion);
            //codigo de prueba------
            tiempo += .1f + elapsedTime;
            if(tiempo > 15f)
            {
                Disparar(PosicionB);
                tiempo = 0f;
            }
            //--------
        }

        public override void Render()
        {
            if (GameManager.Instance.esVisibleParaLaCamara(this.mainMesh))
            {
                mainMesh.Render();
                //mainMesh.BoundingBox.Render();
            }
            else
            {
                GameManager.Instance.QuitarRenderizable(this);
                //Logger.Loggear("Se borro torreta posicionada en Z: " + posicionInicial.Z.ToString());
            }
            /* ----------No borrar aun este comentario.-------------------
            TGCVector3 PosicionB = jugador.GetPosicion();
            TGCVector3 DireccionA = new TGCVector3(0, 0, -1);
            TGCVector3 DireccionB = PosicionB - posicionInicial;
            bool dada = posicionInicial.Z > PosicionB.Z;
            new TgcText2D().drawText("Distancia: " + DireccionB.Length().ToString(), 5, 20, Color.White);
            new TgcText2D().drawText("\n Condicion: " + dada.ToString(), 5, 20, Color.White);
            */

        }

        public void Disparar(TGCVector3 posicionNave)
        {
                posicionNave.Z += 15f; //TODO modificar este valor respecto la velocidad de la nave
                TGCVector3 direccionDisparo = posicionNave - posicionInicial;
                TGCVector3 posicionLaser = posicionInicial;
                posicionLaser.Y += 0.5f;
                GameManager.Instance.AgregarRenderizable(new LaserEnemigo(mediaDir, posicionLaser, direccionDisparo, naveDelJugador));
        }

        public override TgcBoundingAxisAlignBox GetBoundingBox()
        {
            return mainMesh.BoundingBox;
        }

    }
}
