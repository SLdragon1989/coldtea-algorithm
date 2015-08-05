using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System.IO;

namespace AzureFaceApiDemo
{
    class Program
    {
        private static IFaceServiceClient faceServiceClient = new FaceServiceClient("f64a23b414764e85af459dee5e50537f");
        static void Main(string[] args)
        {
            var test = new Test();
            var similarityResult = test.CalculateSimilarity();
            Task.WaitAll(similarityResult);

            Console.WriteLine();

            var detectionResult = test.DetectFaces();
            Task.WaitAll(detectionResult);
            Console.ReadKey();
        }

        public class Test
        {
            public async Task<VerifyResult> CalculateSimilarity()
            {
                var face1 = await faceServiceClient.DetectAsync("http://sh.sinaimg.cn/2009/0625/2009625184523.jpg");
                var face2 = await faceServiceClient.DetectAsync("http://www.5imp.com/5impFile/images/1392172110112%5B1%5D.jpg");
                var result = await faceServiceClient.VerifyAsync(face1[0].FaceId, face2[0].FaceId);
                Console.WriteLine("Confidence: ");
                Console.WriteLine(result.Confidence);
                Console.WriteLine("Isidentical: ");
                Console.WriteLine(result.IsIdentical);
                return result;
            }

            public async Task<Face[]> DetectFaces()
            {
                var result = await faceServiceClient.DetectAsync("http://news.xinhuanet.com/world/2013-06/09/124835868_11n.jpg", true, true, true, true);
                for (int i = 0; i < result.Length; ++i)
                {
                    Console.WriteLine("Person" + (i + 1).ToString());
                    Console.WriteLine("Gender: " + result[i].Attributes.Gender);
                    Console.WriteLine("Age: " + result[i].Attributes.Age);
                }
                    return result;
            }
        }
    }
}
