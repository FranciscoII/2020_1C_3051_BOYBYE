using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;

namespace TGC.Group.Model.Mundo
{
    class SkyboxMenu : IRenderizable
    {
        TgcMesh skySphere;
        private float giro;
        private TGCMatrix MatrizEscala;

        public SkyboxMenu(string mediaDir, TGCVector3 posicion)
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene2 = loader.loadSceneFromFile(mediaDir + "Skybox\\skySphere-TgcScene.xml");
            skySphere = scene2.Meshes[0];
            skySphere.Position = posicion;
            MatrizEscala = TGCMatrix.Scaling(10f, 10f, 10f);
            this.giro = 0f;
        }

        public void Dispose()
        {
            skySphere.Dispose();
        }

        public void Init()
        {
            skySphere.Transform = MatrizEscala;
        }

        public void Render()
        {
            skySphere.Render();
        }

        public void Update(float elapsedTime)
        {
            this.giro += elapsedTime * .3f;
            TGCQuaternion rotationY = TGCQuaternion.RotationAxis(new TGCVector3(0.0f, 1.0f, 0.0f), this.giro);
            skySphere.Transform = MatrizEscala * TGCMatrix.RotationTGCQuaternion(rotationY);
        }
    }
}
