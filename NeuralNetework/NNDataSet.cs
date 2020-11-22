using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetework
{
    public abstract class NNDataSet<TProccesd>
    {
        public abstract string Path { get; set; }
        public abstract List<(double[] input, double[] target, int label)> Process();
        public abstract double[] ProcessImage();
    } 
}
