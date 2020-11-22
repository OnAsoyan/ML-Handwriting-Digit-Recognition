using Meta.Numerics.Matrices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetework
{
    public static class Helper
    {
        public static RectangularMatrix ToMatrix(this double[] source)
        {
            var matrix = new RectangularMatrix(source.Length, 1);
            for (int i = 0; i < matrix.RowCount; i++)
            {
                matrix[i, 0] = source[i];
            }
            return matrix;
        }

        public static RectangularMatrix ProcessWithFunction(this RectangularMatrix source, Func<double, double> func)
        {
            for (int i = 0; i < source.RowCount; i++)
            {
                for (int j = 0; j < source.ColumnCount; j++)
                {
                    source[i, j] = func(source[i, j]);
                }
            }
            return source;
        }

        public static void PrintMatrix(RectangularMatrix matrix)
        {
            for (int i = 0; i < matrix.RowCount; i++)
            {
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    Console.Write(matrix[i, j] + "  ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public static MNISTDataSet ProcessSingleImageToDataSet(string path)
        {
            return null;
        }
    }
}
