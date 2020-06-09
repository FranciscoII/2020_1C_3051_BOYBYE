using System;
using System.Collections.Generic;
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

        public SkyboxMenu(string mediaDir, TGCVector3 posicion)
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene2 = loader.loadSceneFromFile(mediaDir + "Skybox\\skySphere-TgcScene.xml");
            skySphere = scene2.Meshes[0];
            skySphere.Position = posicion;
            skySphere.Transform = TGCMatrix.Scaling(10f, 10f, 10f);
        }

        public void Dispose()
        {
            skySphere.Dispose();
        }

        public void Init()
        {

        }

        public void Render()
        {
            skySphere.Render();
        }

        public void Update(float elapsedTime)
        {
            
        }
    }
}
