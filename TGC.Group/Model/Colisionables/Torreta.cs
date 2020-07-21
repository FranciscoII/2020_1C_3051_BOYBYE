using System;
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
using TGC.Core.Shaders;
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
        private String modeloTorreta;

        public Torreta(string mediaDir,TGCVector3 posicionInicial,Nave nave, String texturaTorreta) : base(nave)
        {
            this.mediaDir = mediaDir;
            this.modeloTorreta = texturaTorreta;
            this.posicionInicial = posicionInicial;

        }
        public override void Init()
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene2 = loader.loadSceneFromFile(modeloTorreta);

            //Solo nos interesa el primer modelo de esta escena (tiene solo uno)
            mainMesh = scene2.Meshes[0];
            mainMesh.Position = posicionInicial;
            baseQuaternionTranslation = TGCMatrix.Translation(posicionInicial);
            baseScaleRotation = TGCMatrix.Scaling(new TGCVector3(0.14f, 0.16f, 0.14f));
            mainMesh.Transform = TGCMatrix.Scaling(0.1f, 0.1f, 0.1f);
            var effect = TGCShaders.Instance.LoadEffect("..\\..\\Shaders\\" + "Fran.fx");
            mainMesh.Effect = effect;
            mainMesh.Technique = "Luzbelito";
        }

        public override void Update(float elapsedTime)
        {
            if (GameManager.Instance.estaPausado)
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
            UpdateEffect();
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
        private void UpdateEffect()
        {
            var posicionSol = GameManager.Instance.PosicionSol;
            var posicionCamara = GameManager.Instance.EyePosition();
            mainMesh.Effect.SetValue("ambientColor", ColorValue.FromColor(Color.White));
            mainMesh.Effect.SetValue("diffuseColor", ColorValue.FromColor(Color.LightGray));
            mainMesh.Effect.SetValue("specularColor", ColorValue.FromColor(Color.LightGray));
            mainMesh.Effect.SetValue("KAmbient", 0.9f);
            mainMesh.Effect.SetValue("KDiffuse", 0.75f);
            mainMesh.Effect.SetValue("KSpecular", 0.25f);
            mainMesh.Effect.SetValue("shininess", 13f);
            mainMesh.Effect.SetValue("posicionSol", TGCVector3.TGCVector3ToFloat3Array(posicionSol));
            mainMesh.Effect.SetValue("eyePosition", TGCVector3.TGCVector3ToFloat3Array(posicionCamara));
        }
    }
}
