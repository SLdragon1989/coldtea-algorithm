using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.Structure;
using System.Drawing;

namespace FaceAPI
{
    class FaceIntegration
    {
        private Image<Bgr, byte> faceImgA, faceImgB, resultFace;
        private double integrationRatio;
        private Size srcSize, dstSize;
        private IList<PointF> landmarkA, landmarkB;
        private IList<Point> resultLandmark;

        public FaceIntegration(
            Image<Bgr, byte> _faceImgA,
            Image<Bgr, byte> _faceImgB,
            List<PointF> _landmarkA,
            List<PointF> _landmarkB,
            Size _size)
        {
            faceImgA = _faceImgA.Clone();
            faceImgB = _faceImgB.Clone();
            landmarkA = _landmarkA;
            landmarkB = _landmarkB;
            srcSize = _size;
        }
    }
}
