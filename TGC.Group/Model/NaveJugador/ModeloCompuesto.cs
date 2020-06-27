using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DirectX.Direct3D;
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Shaders;

namespace TGC.Group.Model
{

    public class ModeloCompuesto
    {
        private readonly List<TgcMesh> meshes;
        private TGCMatrix baseScaleRotation;
        private TGCMatrix rotacionMesh;
        private Effect effect;

        public ModeloCompuesto(string direccionDelModelo, TGCVector3 posicionInicial)
        {
            effect = TGCShaders.Instance.LoadEffect("..\\..\\Shaders\\" + "Fran.fx");
            meshes = new TgcSceneLoader().loadSceneFromFile(direccionDelModelo).Meshes;
            this.CambiarPosicion(posicionInicial);
            TransformarModelo(delegate (TgcMesh unMesh) { unMesh.Effect = effect; unMesh.Technique = "Luzbelito"; });
        }

        private void TransformarModelo(Action<TgcMesh> funcionTransformacion)
        {
            meshes.ForEach(delegate (TgcMesh unMesh) { funcionTransformacion(unMesh); });
        }
        private void TransformarBoundingBox(Action<TgcMesh> funcionTransformacion)
        {
            meshes.ForEach(delegate (TgcMesh unMesh) {
                unMesh.BoundingBox.transform(unMesh.Transform);
                funcionTransformacion(unMesh);
            });
        }

        public void CambiarPosicion(TGCVector3 nuevaPosicion)
        {
            TransformarModelo(delegate (TgcMesh unMesh) { unMesh.Position = nuevaPosicion; });
            TransformarModelo(delegate (TgcMesh unMesh) { unMesh.Transform = TGCMatrix.Translation(unMesh.Position); });
        }

        public void CambiarRotacion(TGCVector3 nuevaRotacion)
        {

            TGCQuaternion rotationX = TGCQuaternion.RotationAxis(new TGCVector3(1.0f, 0.0f, 0.0f), nuevaRotacion.X);
            TGCQuaternion rotationY = TGCQuaternion.RotationAxis(new TGCVector3(0.0f, 1.0f, 0.0f), nuevaRotacion.Y);
            TGCQuaternion rotationZ = TGCQuaternion.RotationAxis(new TGCVector3(0.0f, 0.0f, 1.0f), nuevaRotacion.Z);

            TGCQuaternion rotation = rotationX * rotationY * rotationZ;

            baseScaleRotation = TGCMatrix.Scaling(new TGCVector3(0.15f, 0.15f, 0.15f)) * TGCMatrix.RotationY(FastMath.PI_HALF);

            TGCMatrix.RotationTGCQuaternion(rotation);
            //TransformarModelo(delegate (TgcMesh unMesh) { unMesh.Rotation = nuevaRotacion; });
            //TransformarModelo(delegate (TgcMesh unMesh) { unMesh.Transform = TGCMatrix.RotationTGCQuaternion(rotation); });
            rotacionMesh = TGCMatrix.RotationTGCQuaternion(rotation);
        }

        public void AplicarTransformaciones()
        {
            TransformarModelo(delegate (TgcMesh unMesh) { unMesh.Transform = baseScaleRotation * rotacionMesh * TGCMatrix.Translation(unMesh.Position); });
        }


        public void Render()
        {
            TransformarModelo(delegate (TgcMesh unMesh) { unMesh.Render(); });
            TransformarBoundingBox(delegate (TgcMesh unMesh) { unMesh.BoundingBox.GetType(); });
            //TODO: transformarBoundingBox no deberia recibir ningun parametro.
        }

        public void Dispose()
        {
            TransformarModelo(delegate (TgcMesh unMesh) { unMesh.Dispose(); });
        }

        public List<TgcBoundingAxisAlignBox> BoundingBoxesDelModelo()
        {
            List<TgcBoundingAxisAlignBox> boundingBoxes = new List<TgcBoundingAxisAlignBox>();
            meshes.ForEach(delegate (TgcMesh unMesh) { boundingBoxes.Add(unMesh.BoundingBox); });
            return boundingBoxes;
        }

        public Boolean ColisionaConColisionable(Colisionable unColisionable) 
        {
            return BoundingBoxesDelModelo().Any(bound => TgcCollisionUtils.testAABBAABB(bound, unColisionable.GetBoundingBox()));
        }
        public TgcMesh GetMesh()
        {
            return meshes[0];
        }
        private void UpdateEffect(Effect effect,TGCVector3 posicionSol,TGCVector3 posicionCamara,float tiempo)
        {
            if (meshes[0].Technique == "Blinn")
                return;
            effect.SetValue("ambientColor", ColorValue.FromColor(Color.White));
            effect.SetValue("diffuseColor", ColorValue.FromColor(Color.LightGray));
            effect.SetValue("specularColor", ColorValue.FromColor(Color.LightGray));
            effect.SetValue("KAmbient", 0.9f);
            effect.SetValue("KDiffuse", 0.75f);
            effect.SetValue("KSpecular", 0.5f);
            effect.SetValue("shininess",15f);
            effect.SetValue("posicionSol", TGCVector3.TGCVector3ToFloat3Array(posicionSol));
            effect.SetValue("eyePosition", TGCVector3.TGCVector3ToFloat3Array(posicionCamara));
            effect.SetValue("tiempo", tiempo);
            
        }
        public void UpdateShader(TGCVector3 posicionSol, TGCVector3 posicionCamara,float tiempo)
        {
            TransformarModelo(delegate (TgcMesh unMesh) { UpdateEffect(unMesh.Effect,posicionSol,posicionCamara,tiempo); });
        }
        public void CambiarEscala(TGCVector3 scale)
        {
            baseScaleRotation = TGCMatrix.Scaling(scale);
        }
        public void CambiarShader(Effect shader,String tech)
        {
            TransformarModelo(delegate (TgcMesh unMesh) { unMesh.Effect = shader; unMesh.Technique = tech; });
        }
        public List<TgcMesh> GetMeshes()
        {
            return meshes;
        }
    }
}
