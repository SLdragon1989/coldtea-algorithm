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
    enum Position { 
        EyebrowLeftOuter,
        EyebrowLeftInner,
        EyebrowRightOuter,
        EyebrowRightInner,
        EyeLeftOuter,
        EyeLeftTop,
        EyeLeftInner,
        EyeLeftBottom,
        PupilLeft,
        EyeRightOuter,
        EyeRightTop,
        EyeRightInner,
        EyeRightBottom,
        PupilRight,
        NoseRootLeft,
        NoseLeftAlarTop,
        NoseLeftAlarOutTip,
        NoseTip,
        NoseRightAlarOutTip,
        NoseRightAlarTop,
        NoseRootRight,
        MouthLeft,
        UpperLipTop,
        MouthRight,
        UnderLipBottom,
        UpperLipBottom,
        UnderLipTop
    };

    public class Program : IFaceMorph
    {
        static string picFolder = @"E:\SIE\";//"D:\\Codes\\datasets\\face_morph\\"; 

        public static void Main(string[] args)
        {
            IFaceMorph ifa = new Program();
            ifa.Morph(picFolder + "pic1.jpg", picFolder + "pic2.jpg", picFolder, 0.5);
        }

        public virtual List<string> Morph(string fileName1, string fileName2, string outputFolder, double p)
        {
            List<string> results = new List<string>() { outputFolder + "resul1.jpg", outputFolder + "resul2.jpg", outputFolder + "resul3.jpg" };

            FaceRectangle[] obamaRect, kimRect;
            FaceLandmarks[] obamaLandmarks, kimLandmarks;
            string obamaFile = fileName1;
            string kimFile = fileName2;

            runFaceAPI(obamaFile, out obamaRect, out obamaLandmarks);
            runFaceAPI(kimFile, out kimRect, out kimLandmarks);

            PointF[] obamaLandmarkArr = convertLandmarkFormation(ref obamaLandmarks[0], ref obamaRect[0]);
            PointF[] kimLandmarkArr = convertLandmarkFormation(ref kimLandmarks[0], ref kimRect[0]);

            Rectangle obamaRectangle = convertRectangleFormation(obamaRect[0]);
            Rectangle kimRectangle = convertRectangleFormation(kimRect[0]);

            Image<Bgr, byte> obamaFaceOriginal = new Image<Bgr, byte>(obamaFile);
            Image<Bgr, byte> kimFaceOriginal = new Image<Bgr, byte>(kimFile);

            Image<Bgr, byte> obamaFace = obamaFaceOriginal.GetSubRect(obamaRectangle);
            Image<Bgr, byte> kimFace = kimFaceOriginal.GetSubRect(kimRectangle);

            FaceIntegration faceIntegration = new FaceIntegration(
                obamaFace,
                kimFace,
                obamaLandmarkArr,
                kimLandmarkArr,
                new Size(300, 300),
                p);
            Image<Bgr, byte> dstFace = faceIntegration.integrateFace();
            CvInvoke.MedianBlur(dstFace, dstFace, 5);

            dstFace.Save(results[0]);
            
            dstFace.Resize(obamaRectangle.Width, obamaRectangle.Height, Emgu.CV.CvEnum.Inter.Linear).CopyTo(obamaFace);
            obamaFaceOriginal.Save(results[1]);
            dstFace.Resize(kimRectangle.Width, kimRectangle.Height, Emgu.CV.CvEnum.Inter.Linear).CopyTo(kimFace);
            kimFaceOriginal.Save(results[2]);
            return results; 
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
            PointF[] retLandmarks = new PointF[27]{
                convertPointFormation(_landmarks.EyebrowLeftOuter, _rectangle),
                convertPointFormation(_landmarks.EyebrowLeftInner, _rectangle),
                convertPointFormation(_landmarks.EyebrowRightOuter, _rectangle),
                convertPointFormation(_landmarks.EyebrowRightInner, _rectangle),
                convertPointFormation(_landmarks.EyeLeftOuter, _rectangle),
                convertPointFormation(_landmarks.EyeLeftTop, _rectangle),
                convertPointFormation(_landmarks.EyeLeftInner, _rectangle),
                convertPointFormation(_landmarks.EyeLeftBottom, _rectangle),
                convertPointFormation(_landmarks.PupilLeft, _rectangle),
                convertPointFormation(_landmarks.EyeRightOuter, _rectangle),
                convertPointFormation(_landmarks.EyeRightTop, _rectangle),
                convertPointFormation(_landmarks.EyeRightInner, _rectangle),
                convertPointFormation(_landmarks.EyeRightBottom, _rectangle),
                convertPointFormation(_landmarks.PupilRight, _rectangle),
                convertPointFormation(_landmarks.NoseRootLeft, _rectangle),
                convertPointFormation(_landmarks.NoseLeftAlarTop, _rectangle),
                convertPointFormation(_landmarks.NoseLeftAlarOutTip, _rectangle),
                convertPointFormation(_landmarks.NoseTip, _rectangle),
                convertPointFormation(_landmarks.NoseRightAlarOutTip, _rectangle),
                convertPointFormation(_landmarks.NoseRightAlarTop, _rectangle),
                convertPointFormation(_landmarks.NoseRootRight, _rectangle),
                convertPointFormation(_landmarks.MouthLeft, _rectangle),
                convertPointFormation(_landmarks.UpperLipTop, _rectangle),
                convertPointFormation(_landmarks.MouthRight, _rectangle),
                convertPointFormation(_landmarks.UnderLipBottom, _rectangle),
                convertPointFormation(_landmarks.UpperLipBottom, _rectangle),
                convertPointFormation(_landmarks.UnderLipTop, _rectangle)
            };
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
