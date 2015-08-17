using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Drawing;

namespace FaceAPI
{
    class FaceIntegration
    {
        private Image<Bgr, byte> faceImgA, faceImgB, dstFace;
        private double integrationRatio;
        private Size srcSize, dstSize;
        private PointF[] landmarkA, landmarkB;
        private PointF[] srcLandmarkA, srcLandmarkB, dstLandmark;
        private PointF[,] triangularSetA, triangularSetB;
        private PointF[,] quadrangularSetA, quadrangularSetB;

        private const int pointNum = 27;

        public FaceIntegration(
            Image<Bgr, byte> _faceImgA,
            Image<Bgr, byte> _faceImgB,
            PointF[] _landmarkA,
            PointF[] _landmarkB,
            Size _size,
            double _integrationRatio)
        {
            faceImgA = _faceImgA.Clone();
            faceImgB = _faceImgB.Clone();
            landmarkA = _landmarkA;
            landmarkB = _landmarkB;
            srcSize = _size;
            integrationRatio = _integrationRatio;

            setSrcFaceParam();     
            setDstFaceParam();

            setSrcTriangularSet();
            setSrcQuadrangularSet();
        }

        public Image<Bgr, byte> integrateFace()
        {
            Mat srcRotMatA = new Mat();
            Mat srcRotMatB = new Mat();

            srcRotMatA = CvInvoke.GetAffineTransform(srcLandmarkA, dstLandmark);
            srcRotMatB = CvInvoke.GetAffineTransform(srcLandmarkB, dstLandmark);
            Image<Bgr, byte> srcWarpA = new Image<Bgr, byte>(dstSize);
            Image<Bgr, byte> srcWarpB = new Image<Bgr, byte>(dstSize);
            srcWarpA.SetZero();
            srcWarpB.SetZero();
            CvInvoke.WarpAffine(faceImgA, srcWarpA, srcRotMatA, dstSize);
            CvInvoke.WarpAffine(faceImgB, srcWarpB, srcRotMatB, dstSize);

            dstFace = integrationRatio * faceImgA + (1 - integrationRatio) * faceImgB;

            return dstFace;
        }

        private void setSrcFaceParam()
        {
            faceImgA = faceImgA.Resize(srcSize.Width, srcSize.Height, Inter.Linear);
            faceImgB = faceImgB.Resize(srcSize.Width, srcSize.Height, Inter.Linear);

            srcLandmarkA = new PointF[pointNum];
            for (int cnt = 0; cnt < pointNum; cnt++)
            {
                srcLandmarkA[cnt] = new PointF(
                    (float)(landmarkA[cnt].X * srcSize.Width),
                    (float)(landmarkA[cnt].Y * srcSize.Height));
            }

            srcLandmarkB = new PointF[pointNum];
            for (int cnt = 0; cnt < pointNum; cnt++)
            {
                srcLandmarkB[cnt] = new PointF(
                    (float)(landmarkB[cnt].X * srcSize.Width),
                    (float)(landmarkB[cnt].Y * srcSize.Height));
            }
        }

        private void setDstFaceParam()
        {
            dstSize = srcSize;
            dstFace = new Image<Bgr, byte>(dstSize);
            dstFace.SetZero();

            dstLandmark = new PointF[pointNum];
            for (int cnt = 0; cnt < pointNum; cnt++)
            {
                dstLandmark[cnt] = new PointF(
                    (float)((landmarkA[cnt].X + landmarkB[cnt].X) * srcSize.Width * integrationRatio),
                    (float)((landmarkA[cnt].Y + landmarkB[cnt].Y) * srcSize.Height * (1 - integrationRatio)));
            }
        }

        private void setSrcTriangularSet()
        {
            triangularSetA = new PointF[10, 3]{
                {landmarkA[(int)Position.EyebrowLeftOuter],
                landmarkA[(int)Position.EyebrowLeftInner],
                landmarkA[(int)Position.PupilLeft]},
                {landmarkA[(int)Position.EyebrowRightOuter],
                landmarkA[(int)Position.EyebrowRightInner],
                landmarkA[(int)Position.PupilRight]},
                {landmarkA[(int)Position.MouthLeft],
                landmarkA[(int)Position.MouthRight],
                landmarkA[(int)Position.UnderLipBottom]},
                {new PointF(0, 0),
                landmarkA[(int)Position.EyebrowLeftInner],
                landmarkA[(int)Position.EyebrowLeftOuter]},
                {new Point(1, 0),
                landmarkA[(int)Position.EyebrowRightInner],
                landmarkA[(int)Position.EyebrowRightOuter]},
                {landmarkA[(int)Position.MouthLeft],
                landmarkA[(int)Position.UnderLipBottom],
                new PointF(0, 1)},
                {landmarkA[(int)Position.MouthRight],
                landmarkA[(int)Position.UnderLipBottom],
                new PointF(1, 1)},
                {landmarkA[(int)Position.UnderLipBottom],
                new PointF(1, 0),
                new PointF(1, 1)},
                {landmarkA[(int)Position.EyebrowLeftOuter],
                landmarkA[(int)Position.PupilLeft],
                landmarkA[(int)Position.MouthLeft]},
                {landmarkA[(int)Position.EyebrowRightOuter],
                landmarkA[(int)Position.PupilRight],
                landmarkA[(int)Position.MouthRight]},
            };

            triangularSetB = new PointF[10, 3]{
                {landmarkB[(int)Position.EyebrowLeftOuter],
                landmarkB[(int)Position.EyebrowLeftInner],
                landmarkB[(int)Position.PupilLeft]},
                {landmarkB[(int)Position.EyebrowRightOuter],
                landmarkB[(int)Position.EyebrowRightInner],
                landmarkB[(int)Position.PupilRight]},
                {landmarkB[(int)Position.MouthLeft],
                landmarkB[(int)Position.MouthRight],
                landmarkB[(int)Position.UnderLipBottom]},
                {new PointF(0, 0),
                landmarkB[(int)Position.EyebrowLeftInner],
                landmarkB[(int)Position.EyebrowLeftOuter]},
                {new Point(1, 0),
                landmarkB[(int)Position.EyebrowRightInner],
                landmarkB[(int)Position.EyebrowRightOuter]},
                {landmarkB[(int)Position.MouthLeft],
                landmarkB[(int)Position.UnderLipBottom],
                new PointF(0, 1)},
                {landmarkB[(int)Position.MouthRight],
                landmarkB[(int)Position.UnderLipBottom],
                new PointF(1, 1)},
                {landmarkB[(int)Position.UnderLipBottom],
                new PointF(1, 0),
                new PointF(1, 1)},
                {landmarkB[(int)Position.EyebrowLeftOuter],
                landmarkB[(int)Position.PupilLeft],
                landmarkB[(int)Position.MouthLeft]},
                {landmarkB[(int)Position.EyebrowRightOuter],
                landmarkB[(int)Position.PupilRight],
                landmarkB[(int)Position.MouthRight]},
            };
        }

        private void setSrcQuadrangularSet()
        {
            quadrangularSetA = new PointF[5, 4] {
                {landmarkA[(int)Position.EyebrowLeftInner],
                landmarkA[(int)Position.EyebrowRightInner],
                landmarkA[(int)Position.PupilLeft],
                landmarkA[(int)Position.PupilRight]},
                {landmarkA[(int)Position.PupilLeft],
                landmarkA[(int)Position.PupilRight],
                landmarkA[(int)Position.MouthRight],
                landmarkA[(int)Position.MouthLeft]},
                {landmarkA[(int)Position.EyebrowLeftInner],
                landmarkA[(int)Position.EyebrowRightInner],
                new PointF(1, 0),
                new PointF(0, 0)},
                {landmarkA[(int)Position.EyebrowRightOuter],
                landmarkA[(int)Position.MouthRight],
                new PointF(1, 1),
                new PointF(1, 0)},
                {landmarkA[(int)Position.EyeLeftOuter],
                landmarkA[(int)Position.MouthLeft],
                new PointF(0, 1),
                new PointF(0, 0)}
            };

            quadrangularSetB = new PointF[5, 4] {
                {landmarkB[(int)Position.EyebrowLeftInner],
                landmarkB[(int)Position.EyebrowRightInner],
                landmarkB[(int)Position.PupilLeft],
                landmarkB[(int)Position.PupilRight]},
                {landmarkB[(int)Position.PupilLeft],
                landmarkB[(int)Position.PupilRight],
                landmarkB[(int)Position.MouthRight],
                landmarkB[(int)Position.MouthLeft]},
                {landmarkB[(int)Position.EyebrowLeftInner],
                landmarkB[(int)Position.EyebrowRightInner],
                new PointF(1, 0),
                new PointF(0, 0)},
                {landmarkB[(int)Position.EyebrowRightOuter],
                landmarkB[(int)Position.MouthRight],
                new PointF(1, 1),
                new PointF(1, 0)},
                {landmarkB[(int)Position.EyeLeftOuter],
                landmarkB[(int)Position.MouthLeft],
                new PointF(0, 1),
                new PointF(0, 0)}
            };
        }

    }
}
