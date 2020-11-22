using Meta.Numerics.Matrices;
using NeuralNetework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace NeuralNetework
{
    public class NeuralNetwork
    {
        public double LearningRate { get; set; }
        public int InputNeuronsCount { get; set; }
        public int HiddenNeuronsCount { get; set; }
        public int OutputNeuronsCount { get; set; }
        public Func<double, double> ActivationFunction { get; set; }
        public RectangularMatrix Weights_input_hidden { get; set; }
        public RectangularMatrix Weights_hidden_output { get; set; }
        public NNDataSet<double[]> DataSetTrain { get; set; }
        public NNDataSet<double[]> DataSetTest { get; set; }
        public NNDataSet<double[]> SingleImageDataSet { get; set; }
        public int Bias { get; set; }
        public BackgroundWorker BackgroundWorker { get; set; }

        public NeuralNetwork(double learningRate, int inputNeuronsCount, int hiddenNeuronsCount, int outputNeuronsCount)
        {
            LearningRate = learningRate;
            InputNeuronsCount = inputNeuronsCount;
            HiddenNeuronsCount = hiddenNeuronsCount;
            OutputNeuronsCount = outputNeuronsCount;
            ActivationFunction = Sigmoid;
            Weights_input_hidden = GetWightsInputHiddenRandom(InputNeuronsCount, HiddenNeuronsCount);
            Weights_hidden_output = GetWightsInputHiddenRandom(HiddenNeuronsCount, OutputNeuronsCount);
        }

        private double Sigmoid(double arg)
        {
            return 1 / (1 + Math.Pow(Math.E, -arg));
        }
        public void Train()
        { 
            var dataSet = DataSetTrain.Process();
            foreach (var ds in dataSet)
            {
                var first_hidden_output = CalculateOutputsForLayer(ds.input, Weights_input_hidden);
                var hidden_out_output = CalculateOutputsForLayer(first_hidden_output.Column(0).ToArray(), Weights_hidden_output);
                var inputs_matrix = ds.input.ToMatrix();
                var target = ds.target.ToMatrix();
                var outputLayer_error = (target - hidden_out_output);
                //ProcessMatrix(outputLayer_error, (double x) => Math.Pow(x, 2));  
                var hidden_layer_error = Weights_hidden_output.Transpose * outputLayer_error;

                Weights_hidden_output = GetNewWeights(outputLayer_error, hidden_out_output, first_hidden_output, Weights_hidden_output);
                Weights_input_hidden = GetNewWeights(hidden_layer_error, first_hidden_output, inputs_matrix, Weights_input_hidden);
            }
        }

        List<double> GetErrorDelta(RectangularMatrix newErros, RectangularMatrix lastError)
        {
            List<double> list = new List<double>();
            for (int i = 0; i < newErros.RowCount; i++)
            {
                var delta = newErros[i, 0] - lastError[i, 0];
                list.Add(delta);
            }
            return list;
        }

        void ProcessMatrix(RectangularMatrix source, Func<double, double> func)
        {
            for (int i = 0; i < source.RowCount; i++)
            {
                for (int j = 0; j < source.ColumnCount; j++)
                {
                    source[i, j] = func(source[i, j]);
                }
            }
        }
        public RectangularMatrix GetNewWeights(RectangularMatrix errors, RectangularMatrix matrixForSigmoid, RectangularMatrix multiplicated_matrix, RectangularMatrix weights)
        {
            var sigmoid_scaliar_multiplicated_one_min_sigmoid = ScaliarMultiplication(matrixForSigmoid, matrixForSigmoid, (x) => 1 - x);
            var scaliar_multplicate_with_error = ScaliarMultiplication(errors, sigmoid_scaliar_multiplicated_one_min_sigmoid, null);
            var vectoral_multiplicate_with_matrix = scaliar_multplicate_with_error * multiplicated_matrix.Transpose;
            var with_learing_rate = (LearningRate * vectoral_multiplicate_with_matrix);
            return weights + with_learing_rate;
        }
        public RectangularMatrix ScaliarMultiplication(RectangularMatrix source, RectangularMatrix multyplicator, Func<double, double> func)
        {
            if (source.RowCount != multyplicator.RowCount || source.ColumnCount != multyplicator.ColumnCount)
                throw new InvalidOperationException("Cant mutiplicate");

            var sourceClone = source.Copy();
            for (int i = 0; i < sourceClone.RowCount; i++)
            {
                for (int j = 0; j < sourceClone.ColumnCount; j++)
                {
                    var multy = func == null ? multyplicator[i, j] : func(multyplicator[i, j]);
                    sourceClone[i, j] = sourceClone[i, j] * multy;
                }
            }
            return sourceClone;
        }
        public List<int> Result = new List<int>();
        public void GetOutput()
        {
            Result = new List<int>();
            var dataSet = DataSetTest.Process();
            foreach (var item in dataSet)
            {
                var first_hidden_output = CalculateOutputsForLayer(item.input, Weights_input_hidden);
                var hidden_out_output = CalculateOutputsForLayer(first_hidden_output.Column(0).ToArray(), Weights_hidden_output);
                var res = hidden_out_output.Column(0).ToArray();
                var max_result = res.Max();
                int max_res_index = -1;
                for (int i = 0; i < res.Length; i++)
                {
                    if (res[i] == max_result)
                        max_res_index = i;
                }
                Result.Add(max_res_index == item.label ? 1 : 0);
            }
        }

        public int GetSingleOutput()
        {
            var dataSet = SingleImageDataSet.ProcessImage();
            var first_hidden_output = CalculateOutputsForLayer(dataSet, Weights_input_hidden);
            var hidden_out_output = CalculateOutputsForLayer(first_hidden_output.Column(0).ToArray(), Weights_hidden_output);
            var res = hidden_out_output.Column(0).ToArray();
            var max_result = res.Max();
            int max_res_index = -1;
            for (int i = 0; i < res.Length; i++)
            {
                if (res[i] == max_result)
                    max_res_index = i;
            }
            return max_res_index;
        }

        public Dictionary<int, double> GetSinglePersatagedOutput()
        {
            Dictionary<int, double> persantages = new Dictionary<int, double>();
            var dataSet = SingleImageDataSet.ProcessImage();
            var first_hidden_output = CalculateOutputsForLayer(dataSet, Weights_input_hidden);
            var hidden_out_output = CalculateOutputsForLayer(first_hidden_output.Column(0).ToArray(), Weights_hidden_output);
            var res = hidden_out_output.Column(0).ToArray(); 
            for (int i = 0; i < res.Length; i++)
            {
                var pers = 100 - (100 * (1 - res[i])); 
                persantages.Add(i, pers); 
            }
            return persantages;
        }

        public RectangularMatrix CalculateOutputsForLayer(double[] inputs, RectangularMatrix weghts)
        {
            var inputs_matrix = inputs.ToMatrix();
            var nextLayerOutputs = weghts * inputs_matrix;
            var withSigmoid = (nextLayerOutputs).Copy();
            withSigmoid.ProcessWithFunction(ActivationFunction);
            return withSigmoid;
        }

        public RectangularMatrix GetWightsInputHiddenRandom(int lastNeuronsCount, int nextNeuronsCount)
        {
            var List = new List<decimal>();
            var diapason = (1 / Math.Sqrt(nextNeuronsCount));
            for (decimal i = -0.99999999M; i <= 0.99999999M; i += 0.00002M)
            {
                List.Add(i);
            }
            RectangularMatrix matrix = new RectangularMatrix(nextNeuronsCount, lastNeuronsCount);
            var rand = new Random();

            for (int i = 0; i < nextNeuronsCount; i++)
            {
                for (int j = 0; j < lastNeuronsCount; j++)
                {
                    var index = rand.Next(0, List.Count - 1);
                    matrix[i, j] = (double)List[index];
                    List.RemoveAt(index);
                }
            }
            return matrix;
        }
        public double GetRandomNumber(double minimum, double maximum)
        {
            Random random = new Random();
            return random.NextDouble() * (maximum - minimum) + minimum;
        }
    }
}
