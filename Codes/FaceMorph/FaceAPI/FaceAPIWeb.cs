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

using FaceAPI;
using System.Net;

namespace FaceAPIWeb
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

    public class FaceAPIWeb
    {
        public static string Morph(string url1, string url2, double align)
        { 
            if(DownloadPic(url1, "pic1.jpg") && DownloadPic(url2, "pic2.jpg"))
            {
                FaceRectangle[] obamaRect, kimRect;
                FaceLandmarks[] obamaLandmarks, kimLandmarks;
                string obamaFile = "pic1.jpg";
                string kimFile = "pic2.jpg";

                runFaceAPI(obamaFile, out obamaRect, out obamaLandmarks);
                runFaceAPI(kimFile, out kimRect, out kimLandmarks);

                PointF[] obamaLandmarkArr = convertLandmarkFormation(ref obamaLandmarks[0], ref obamaRect[0]);
                PointF[] kimLandmarkArr = convertLandmarkFormation(ref kimLandmarks[0], ref kimRect[0]);

                Rectangle obamaRectangle = convertRectangleFormation(obamaRect[0]);
                Rectangle kimRectangle = convertRectangleFormation(kimRect[0]);

                Image<Bgr, byte> obamaFace = new Image<Bgr, byte>(obamaFile).GetSubRect(obamaRectangle);
                Image<Bgr, byte> kimFace = new Image<Bgr, byte>(kimFile).GetSubRect(kimRectangle);

                FaceIntegration faceIntegration = new FaceIntegration(
                    obamaFace,
                    kimFace,
                    obamaLandmarkArr,
                    kimLandmarkArr,
                    new Size(300, 300),
                    align);
                Image<Bgr, byte> dstFace = faceIntegration.integrateFace();
                dstFace.Save("result.jpg");
                return "result.jpg";
            }
            else
            {
                throw new ArgumentException("Download Error");
            }
            
        }

        private static bool DownloadPic(string url, string fileName)
        {
            try
            {
                WebRequest myre = WebRequest.Create(url);
                WebClient client = new WebClient();
                client.DownloadFile(url, fileName);
                return true;
            }
            catch (WebException exp)
            {
                return false;
            }
        } 


        private static void runFaceAPI(
            string _filePath, 
            out FaceRectangle[] _rects, 
            out FaceLandmarks[] _landmarks)
        {
            string key = "1cdb412565ae43879ea8133525e89040";
            FaceAPI.FaceAPI faceAPI = new FaceAPI.FaceAPI(key);
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
                convertPointFormation(_landmarks.UnderLipTop, _rectangle),
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
