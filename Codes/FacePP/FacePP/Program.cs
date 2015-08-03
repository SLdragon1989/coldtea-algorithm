using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

using FaceppSDK;

using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.Shape;

namespace FacePP
{
    class Program
    {
        enum FeaturePoint { LeftEye, RightEye, Nose, LeftMouth, RightMouth };

        static void Main(string[] args)
        {
            String apiKey = "847e6315f892e21449da5f4077c5104f";
            String apiSecret = "BmskojfFyrZVQhkLfNSnRzX-lK8musO6";
            FaceService faceService = new FaceService(apiKey, apiSecret);

            string filePath = "D:\\Codes\\datasets\\face_morph\\bbt.jpg";
            DetectResult detectResult = faceService.Detection_DetectImg(filePath);

            Image<Bgr, Byte> srcImg = new Image<Bgr, Byte>(filePath);
            
            for(int cnt=0; cnt < detectResult.face.Count; cnt++)
            {
                string pointFileName = String.Format("D:\\Codes\\datasets\\face_morph\\result_bbt_face_{0}.txt", cnt);
                FileStream fileStream = new FileStream(pointFileName, FileMode.Create);
                StreamWriter streamWriter = new StreamWriter(fileStream);

                Rectangle faceRect = new Rectangle(
                    (int)(detectResult.face[cnt].position.center.x * srcImg.Width /100 - detectResult.face[cnt].position.width * srcImg.Width * 0.5 / 100),
                    (int)(detectResult.face[cnt].position.center.y * srcImg.Height / 100 - detectResult.face[cnt].position.height * srcImg.Height * 0.5 / 100),
                    (int)detectResult.face[cnt].position.width * srcImg.Width / 100,
                    (int)detectResult.face[cnt].position.height * srcImg.Height / 100);
                
                Image<Bgr, byte> faceImg = srcImg.GetSubRect(faceRect);

                string fileName = String.Format("D:\\Codes\\datasets\\face_morph\\result_bbt_face_{0}.jpg", cnt);
                faceImg.Save(fileName);

                IList<FaceppSDK.Point> featurePoints = new List<FaceppSDK.Point>();
                //featurePoints.Add(detectResult.face[cnt].position.center);
                FaceppSDK.Point tempPoint1 = new FaceppSDK.Point();
                tempPoint1.x = (detectResult.face[cnt].position.eye_left.x
                    - detectResult.face[cnt].position.center.x) / detectResult.face[cnt].position.width;
                tempPoint1.y = (detectResult.face[cnt].position.eye_left.y
                    - detectResult.face[cnt].position.center.y) / detectResult.face[cnt].position.height;
                featurePoints.Add(tempPoint1);

                FaceppSDK.Point tempPoint2 = new FaceppSDK.Point();
                tempPoint2.x = (detectResult.face[cnt].position.eye_right.x
                    - detectResult.face[cnt].position.center.x) / detectResult.face[cnt].position.width;
                tempPoint2.y = (detectResult.face[cnt].position.eye_right.y
                    - detectResult.face[cnt].position.center.y) / detectResult.face[cnt].position.height;
                featurePoints.Add(tempPoint2);

                FaceppSDK.Point tempPoint3 = new FaceppSDK.Point();
                tempPoint3.x = (detectResult.face[cnt].position.mouth_left.x
                    - detectResult.face[cnt].position.center.x) / detectResult.face[cnt].position.width;
                tempPoint3.y = (detectResult.face[cnt].position.mouth_left.y
                    - detectResult.face[cnt].position.center.y) / detectResult.face[cnt].position.height;
                featurePoints.Add(tempPoint3);

                FaceppSDK.Point tempPoint4 = new FaceppSDK.Point();
                tempPoint4.x = (detectResult.face[cnt].position.mouth_right.x
                    - detectResult.face[cnt].position.center.x) / detectResult.face[cnt].position.width;
                tempPoint4.y = (detectResult.face[cnt].position.mouth_right.y
                    - detectResult.face[cnt].position.center.y) / detectResult.face[cnt].position.height;
                featurePoints.Add(tempPoint4);

                FaceppSDK.Point tempPoint5 = new FaceppSDK.Point();
                tempPoint5.x = (detectResult.face[cnt].position.nose.x
                    - detectResult.face[cnt].position.center.x) / detectResult.face[cnt].position.width;
                tempPoint5.y = (detectResult.face[cnt].position.nose.y
                    - detectResult.face[cnt].position.center.y) / detectResult.face[cnt].position.height;
                featurePoints.Add(tempPoint5);

                foreach (FaceppSDK.Point featurePoint in featurePoints)
                {
                    streamWriter.WriteLine(featurePoint.x.ToString());
                    streamWriter.WriteLine(featurePoint.y.ToString());

                    System.Drawing.PointF point = new System.Drawing.PointF((float)featurePoint.x * srcImg.Width / 100,
                        (float)featurePoint.y * srcImg.Height / 100);
                    Cross2DF cross = new Cross2DF(point, (float)3.0, (float)3.0);
                    srcImg.Draw(cross, new Bgr(0, 255, 0), 3);
                }

                streamWriter.Flush();
                streamWriter.Close();
                fileStream.Close();
                //srcImg.Save("D:\\Codes\\datasets\\face_morph\\result_bbt.jpg");
            }
        }
    }
}
