using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
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
        static void Main(string[] args)
        {
            String apiKey = "847e6315f892e21449da5f4077c5104f";
            String apiSecret = "BmskojfFyrZVQhkLfNSnRzX-lK8musO6";
            FaceService faceService = new FaceService(apiKey, apiSecret);

            String filePath = "D:\\Codes\\datasets\\face\\king.jpg";
            DetectResult detectResult = faceService.Detection_DetectImg(filePath);

            for(int cnt=0; cnt < detectResult.face.Count; cnt++)
            {
                FaceppSDK.Point center  = detectResult.face[cnt].position.center;
                FaceppSDK.Point leftEye = detectResult.face[cnt].position.eye_left;
                FaceppSDK.Point rightEye = detectResult.face[cnt].position.eye_right;
                FaceppSDK.Point leftMouth = detectResult.face[cnt].position.mouth_left;
                FaceppSDK.Point rightMouth = detectResult.face[cnt].position.mouth_right;
                FaceppSDK.Point nose = detectResult.face[cnt].position.nose;
                FaceppSDK.Age age = detectResult.face[cnt].attribute.age;
                FaceppSDK.Gender gender = detectResult.face[cnt].attribute.gender;
                FaceppSDK.Race race = detectResult.face[cnt].attribute.race;

                Console.WriteLine("Center: " + center.x.ToString() + ", " + center.y.ToString());
                Console.WriteLine("Left Eye: " + leftEye.x.ToString() + ", " + leftEye.y.ToString());
                Console.WriteLine("Right Eye: " + rightEye.x.ToString() + ", " + rightEye.y.ToString());
                Console.WriteLine("Left Mouth: " + leftMouth.x.ToString() + ", " + leftMouth.y.ToString());
                Console.WriteLine("Right Mouth: " + rightMouth.x.ToString() + ", " + rightMouth.y.ToString());
                Console.WriteLine("Nose: " + nose.x.ToString() + ", " + nose.y.ToString());
                Console.WriteLine("Age: " + age.value.ToString());
                Console.WriteLine("Gender: " + gender.value.ToString());
                Console.WriteLine("Race: " + race.value.ToString());
            }
        }
    }
}
