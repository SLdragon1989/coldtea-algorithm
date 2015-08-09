using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;

namespace FaceAPI
{
    class FaceAPI
    {
        private readonly IFaceServiceClient faceServiceClient;
        private FaceRectangle[] faceRectangles;
        private FaceLandmarks[] faceLandmarks;

        public FaceAPI(string _apiKey)
        {
            faceServiceClient = new FaceServiceClient(_apiKey);
        }

        public async Task<bool> detectFaces(string _imageFilePath)
        {
            try
            {
                using (Stream imageFileStream = File.OpenRead(_imageFilePath))
                {
                    var faces = await faceServiceClient.DetectAsync(imageFileStream, true);
                    var faceRectangles = faces.Select(face => face.FaceRectangle);
                    var faceLandmarks = faces.Select(face => face.FaceLandmarks);
                    this.faceRectangles = faceRectangles.ToArray();
                    this.faceLandmarks = faceLandmarks.ToArray();
                    return true;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return false;
            }
        }

        public FaceRectangle[] getFaceRectangles()
        {
            return this.faceRectangles;
        }

        public FaceLandmarks[] getFaceLandmarks()
        {
            return faceLandmarks;
        }
    }
}
