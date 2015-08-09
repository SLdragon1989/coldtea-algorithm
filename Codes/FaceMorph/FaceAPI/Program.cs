using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;

using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.Structure;
using System.Drawing;

namespace FaceAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            FaceRectangle[] faceRectangles;
            FaceLandmarks[] faceLandmarks;
            string filePath = "D:\\Codes\\datasets\\face_morph\\result_bbt.jpg";
            runFaceAPI(filePath, out faceRectangles, out faceLandmarks);
            convertLandmarkFormation(ref faceLandmarks[0], ref faceRectangles[0]);
            Rectangle faceRect = convertRectangleFormation(faceRectangles[0]);
            Image<Bgr, byte> srcImg1 = new Image<Bgr, byte>(filePath);
            Image<Bgr, byte> faceImg1 = srcImg1.GetSubRect(faceRect);
        }

        private static void runFaceAPI(
            string _filePath, 
            out FaceRectangle[] _rects, 
            out FaceLandmarks[] _landmarks)
        {
            string key = "1cdb412565ae43879ea8133525e89040";
            FaceAPI faceAPI = new FaceAPI(key);
            var detectResult = faceAPI.detectFaces(_filePath);
            Task.WaitAll(detectResult);
            if (detectResult.Result)
            {
                _rects = faceAPI.getFaceRectangles();
                _landmarks = faceAPI.getFaceLandmarks();
            }
            else
            {
                _rects = new FaceRectangle[0];
                _landmarks = new FaceLandmarks[0];
            }
        }

        private static PointF[] convertLandmarkFormation(
            ref FaceLandmarks _landmarks,
            ref FaceRectangle _rectangle)
        {
            PointF[] retLandmarks = new PointF[3];
            retLandmarks[0] = convertPointFormation(_landmarks.EyeLeftInner, _rectangle);
            retLandmarks[1] = convertPointFormation(_landmarks.EyeRightInner, _rectangle);
            retLandmarks[2] = convertPointFormation(_landmarks.NoseTip, _rectangle);
            return retLandmarks;
        }

        private static PointF convertPointFormation(
            FeatureCoordinate _landmark,
            FaceRectangle _rectangle)
        {
            PointF retPoint = new PointF();
            retPoint.X = (float)((_landmark.X - _rectangle.Left) / _rectangle.Width);
            retPoint.Y = (float)((_landmark.Y - _rectangle.Top) / _rectangle.Height);
            return retPoint;
        }

        private static Rectangle convertRectangleFormation(
            FaceRectangle _rectangle)
        {
            return new Rectangle(
                _rectangle.Left,
                _rectangle.Top,
                _rectangle.Width,
                _rectangle.Height);
        }
    }
}
