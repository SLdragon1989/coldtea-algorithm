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
        private PointF[,] triangularSetA, triangularSetB, triangularSetDst;
        private PointF[,] quadrangularSetA, quadrangularSetB, quadrangularSetDst;

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

            setTriangularSet();
            setQuadrangularSet();
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

        private void setTriangularSet()
        {
            triangularSetA = new PointF[10, 3]{
                {landmarkA[(int)Position.EyebrowLeftOuter],
                landmarkA[(int)Position.EyebrowLeftInner],
                landmarkA[(int)Position.PupilLeft]},
                {landmarkA[(int)Position.EyebrowRightOuter],
                landmarkA[(int)Position.EyebrowRightInner],
                landmarkA[(int)Position.PupilRight]},
            };
        }

        private void setQuadrangularSet()
        {

        }

    }
}
