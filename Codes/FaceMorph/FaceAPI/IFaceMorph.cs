using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceAPI
{
    public interface IFaceMorph
    {
        List<string> Morph(string fileName1, string fileName2, string outputFolder, double p);
    }
}
