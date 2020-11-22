using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetework
{
    public class MNISTDataSet : NNDataSet<double[]>
    {
        public override string Path { get; set; }
        public MNISTDataSet(string path)
        {
            Path = path;
        }
        public override List<(double[] input, double[] target, int label)> Process()
        {
            try
            {
                List<(double[] input, double[] target, int label)> data = new List<(double[] input, double[] target, int label)>();
                var lines = File.ReadLines(Path).ToList();
                foreach (var l in lines)
                {
                    var target = new double[] { 0.01, 0.01, 0.01, 0.01, 0.01, 0.01, 0.01, 0.01, 0.01, 0.01 };
                    var label = int.Parse(l.FirstOrDefault().ToString());
                    target[label] = 0.99;
                    var imgData = l.Split(',').Skip(1).Select(x => double.Parse(x.ToString())).ToArray<double>();
                    var res = imgData.Select(x => (x / 255.0 * 0.99) + 0.01).ToArray<double>();
                    data.Add((res, target, label));
                }
                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public override double[] ProcessImage()
        {
            try
            {
                List<byte> bytes = new List<byte>();
                Bitmap img = new Bitmap(Path);
                for (int i = 0; i < img.Width; i++)
                {
                    for (int j = 0; j < img.Height; j++)
                    {
                        Color pixel = img.GetPixel(j, i);
                        var brg = pixel.R;
                        bytes.Add((byte)brg);
                    }
                }

                //Bitmap b = new Bitmap(28, 28);
                //var lines = File.ReadLines(Path).ToList();
                //int nameCounter = 0;
                //foreach (var l in lines)
                //{
                //    var num = l[0];
                //    var imgData = l.Split(',').Skip(1).Select(x => double.Parse(x.ToString())).ToArray<double>();
                //    var counter = 0;
                //    for (int i = 0; i < b.Width; i++)
                //    {
                //        for (int j = 0; j < b.Height; j++)
                //        {
                //            var counter1 = counter++;
                //            b.SetPixel(j, i, Color.FromArgb(255, (int)imgData[counter1], (int)imgData[counter1], (int)imgData[counter1]));
                //            var p = b.GetPixel(j, i);
                //        }
                //    }
                //    b.Save($@"E:\ML\GeneratedImages\{num.ToString()}-{nameCounter++}.jpg");
                //}


                var imgData = bytes;
                var res = imgData.Select(x => (x / 255.0 * 0.99) + 0.01).ToArray<double>();
                return res;
            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}
