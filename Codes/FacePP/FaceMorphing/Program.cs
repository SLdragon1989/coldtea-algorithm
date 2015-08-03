using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using System.Drawing;
using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using FaceppSDK;

namespace FaceMorphing
{
    class FaceMorph
    {
        enum LandMark { LeftEye, RightEye, Nose, LeftMouth, RightMouth };
        private Image<Bgr, byte> srcFaceImg, dstFaceImg, intermediateFaceImg;
        private string srcPath, dstPath;
        private Size imgSize;
        private string apiKey, apiSecret;
        private IList<System.Drawing.Point> srcLandmark, dstLandmark, itermediateLandmark;

        FaceService faceService;

        public FaceMorph()
        {
            apiKey = "847e6315f892e21449da5f4077c5104f";
            apiSecret = "BmskojfFyrZVQhkLfNSnRzX-lK8musO6";
            faceService = new FaceService(apiKey, apiSecret);

            srcLandmark = new List<System.Drawing.Point>();
            dstLandmark = new List<System.Drawing.Point>();
            itermediateLandmark = new List<System.Drawing.Point>();
        }

        public void setImgSize(int _imgWidth, int _imgHeight)
        {
            imgSize.Height = _imgHeight;
            imgSize.Width = _imgWidth;
        }

        public void setSrcFaceImg(string _filePath)
        {
            Image<Bgr, byte> inputImg = new Image<Bgr, byte>(_filePath);
            inputImg.Resize(imgSize.Width, imgSize.Height, Inter.Linear);
            srcFaceImg = inputImg.Clone();
            srcPath = _filePath;
        }

        public void setDstFaceImg(string _filePath)
        {
            Image<Bgr, byte> inputImg = new Image<Bgr, byte>(_filePath);
            inputImg.Resize(imgSize.Width, imgSize.Height, Inter.Linear);
            dstFaceImg = inputImg.Clone();
            dstPath = _filePath;
        }

        public void setItermediateFaceImg()
        {
            Image<Bgr, byte> inputImg = new Image<Bgr, byte>(imgSize);
            inputImg.SetZero();
            intermediateFaceImg = inputImg.Clone();
        }

        public void getLandmarks()
        {
            StreamReader srcReader = new StreamReader("D:\\Codes\\datasets\\face_morph\\result_bbt_face_1.txt");
            for(int cnt = 0; cnt < 5; cnt++)
            {
                System.Drawing.Point tempPoint = new System.Drawing.Point();
                tempPoint.X = (int)((Convert.ToSingle(srcReader.ReadLine()) + 0.5) * (imgSize.Width));
                tempPoint.Y = (int)((Convert.ToSingle(srcReader.ReadLine()) + 0.5) * (imgSize.Height));
                srcLandmark.Add(tempPoint);
            }

            StreamReader dstReader = new StreamReader("D:\\Codes\\datasets\\face_morph\\result_bbt_face_2.txt");
            for (int cnt = 0; cnt < 5; cnt++)
            {
                System.Drawing.Point tempPoint = new System.Drawing.Point();
                tempPoint.X = (int)((Convert.ToSingle(dstReader.ReadLine()) + 0.5) * (imgSize.Width));
                tempPoint.Y = (int)((Convert.ToSingle(dstReader.ReadLine()) + 0.5) * (imgSize.Height));
                dstLandmark.Add(tempPoint);
            }

            for(int cnt = 0; cnt < 5; cnt++)
            {
                System.Drawing.Point tempPoint = new System.Drawing.Point();
                tempPoint.X = (srcLandmark[cnt].X / 2 + dstLandmark[cnt].X / 2);
                tempPoint.Y = (srcLandmark[cnt].Y / 2 + dstLandmark[cnt].Y / 2);
                itermediateLandmark.Add(tempPoint);
            }
        }

        public void fuseRegion()
        {
            PointF[] srcSet = new PointF[] {srcLandmark[0], 
                srcLandmark[1],
                srcLandmark[2]
            };

            PointF[] dstSet = new PointF[] {dstLandmark[0], 
                dstLandmark[1],
                dstLandmark[2]
            };

            PointF[] itermediateSet = new PointF[] {itermediateLandmark[0], 
                itermediateLandmark[1],
                itermediateLandmark[2]
            };

            Mat srcRotMat = new Mat();
            Mat dstRotMat = new Mat();

            srcRotMat = CvInvoke.GetAffineTransform(srcSet, itermediateSet);
            dstRotMat = CvInvoke.GetAffineTransform(dstSet, itermediateSet);

            Image<Bgr, byte> srcWarp = new Image<Bgr, byte>(imgSize);
            srcWarp.SetZero();
            Image<Bgr, byte> dstWarp = new Image<Bgr, byte>(imgSize);
            dstWarp.SetZero();

            CvInvoke.WarpAffine(srcFaceImg, srcWarp, srcRotMat, imgSize);
            CvInvoke.WarpAffine(dstFaceImg, dstWarp, dstRotMat, imgSize);

            intermediateFaceImg = srcWarp / 2 + dstWarp / 2;

            srcWarp.Save("D:\\Codes\\datasets\\face_morph\\result_bbt_face_warp_1.jpg");
            dstWarp.Save("D:\\Codes\\datasets\\face_morph\\result_bbt_face_warp_2.jpg");
            intermediateFaceImg.Save("D:\\Codes\\datasets\\face_morph\\result_bbt_face_warp.jpg");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            FaceMorph faceMorph = new FaceMorph();
            faceMorph.setImgSize(140, 140);
            faceMorph.setSrcFaceImg("D:\\Codes\\datasets\\face_morph\\result_bbt_face_1.jpg");
            faceMorph.setDstFaceImg("D:\\Codes\\datasets\\face_morph\\result_bbt_face_2.jpg");
            faceMorph.setItermediateFaceImg();
            faceMorph.getLandmarks();
            faceMorph.fuseRegion();
        }
    }
}
