using TGC.Core.Terrain;
using TGC.Core.Mathematica;
using TGC.Examples.Camara;
using TGC.Core.Direct3D;
using System;
using TGC.Core.Camara;

namespace TGC.Group.Model
{
	public class Skybox : IRenderizable
	{
		private readonly string mediaDir;
		private TgcSkyBox skyBox;
		private TgcCamera camara;


		public Skybox(string mediaDir, TgcCamera camara)
		{
			this.mediaDir = mediaDir;
			this.camara = camara;
		}

		public void Init()
		{
			skyBox = new TgcSkyBox
			{
				Center = TGCVector3.Empty,
				Size = new TGCVector3(9000, 9000, 9000)
			};
			
			string texturesPath = mediaDir + "\\Skybox\\";
			skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "phobos_lf.jpg");
			skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "phobos_rt.jpg");
			skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "phobos_bk.jpg");

			//Caras que no se ven
			skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "azul.jpg");
			skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "azul.jpg");
			skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "phobos_bk.jpg");
			

			skyBox.SkyEpsilon = 25f;
			skyBox.Init();

		}

		public void Update(float elapsedTime)
		{

			//Se cambia el valor por defecto del farplane para evitar cliping de farplane. [Copiado del ejemplo]
			D3DDevice.Instance.Device.Transform.Projection = TGCMatrix.PerspectiveFovLH(D3DDevice.Instance.FieldOfView, D3DDevice.Instance.AspectRatio,
					D3DDevice.Instance.ZNearPlaneDistance, D3DDevice.Instance.ZFarPlaneDistance * 2f).ToMatrix();

			
			skyBox.Center = camara.Position;
		}

		public void Render()
		{
			skyBox.Render();
		}
		public void Dispose()
		{
			skyBox.Dispose();
		}
	}
}

