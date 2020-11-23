using NeuralNetework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DigitDecognisationWinForm
{
    public partial class Form1 : Form
    {
        public MNISTDataSet DataSetTrain = null;
        public MNISTDataSet DataSetTest = null;
        public MNISTDataSet SingleDataSet = null;
        NeuralNetwork NeuralNetework = null;
        public int Count { get; set; }
        public Form1()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }
        private void openFileDial_Click(object sender, EventArgs e)
        {
            using (openFileDialog1 = new OpenFileDialog())
            {
                var res = openFileDialog1.ShowDialog();
                if (res != DialogResult.OK)
                    return;
                textBox1.Text = openFileDialog1.FileName;
                DataSetTrain = new MNISTDataSet(textBox1.Text);
                var Form = new WaitForm();
                Form.Owner = this;
                Form.StartPosition = FormStartPosition.CenterScreen;
                flowLayoutPanel2.Enabled = false;
                Task.Run(new Action(() =>
                {
                    var processd = DataSetTrain.Process();
                    if (processd == null)
                    {
                        MessageBox.Show("INVALID DATA SET");
                        textBox2TestData.BeginInvoke(new Action(() =>
                        {
                            textBox1.Text = string.Empty;
                        }));
                        return;
                    }
                    var count = processd.Count();
                    testDatasetc.BeginInvoke(new Action(() =>
                    {
                        Form.Close();
                        flowLayoutPanel2.Enabled = true;
                        trainDataSetc.Text = count.ToString();
                    }));
                    Count += count;
                }));
                Form.Show();
            }

        }


        private void button1_Click(object sender, EventArgs e)
        {
            using (openFileDialog2 = new OpenFileDialog())
            {

                var res = openFileDialog2.ShowDialog();
                if (res != DialogResult.OK)
                    return;
                textBox2TestData.Text = openFileDialog2.FileName;
                DataSetTest = new MNISTDataSet(textBox2TestData.Text);
                var Form = new WaitForm();
                Form.Owner = this;
                Form.StartPosition = FormStartPosition.CenterScreen;
                flowLayoutPanel2.Enabled = false;
                Task.Run(new Action(() =>
                {
                    var processd = DataSetTest.Process();
                    if (processd == null)
                    {
                        MessageBox.Show("INVALID DATA SET");
                        textBox2TestData.BeginInvoke(new Action(() =>
                        {
                            textBox2TestData.Text = string.Empty;
                        }));
                        return;
                    }
                    testDatasetc.BeginInvoke(new Action(() =>
                    {
                        Form.Close();
                        flowLayoutPanel2.Enabled = true;
                        testDatasetc.Text = processd.Count().ToString();
                    }));
                }));
                Form.Show();


            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (DataSetTrain == null || DataSetTest == null)
            {
                MessageBox.Show("INVALID DATA");
                return;
            }
            var learningRate = Convert.ToDouble(numericUpDown1.Value);
            var Form = new WaitForm(true);
            Form.Owner = this;
            Form.StartPosition = FormStartPosition.CenterScreen;
            flowLayoutPanel2.Enabled = false;
            Task.Run(new Action(() =>
            {
                NeuralNetework = new NeuralNetwork(learningRate, 784, 100, 10);
                NeuralNetework.DataSetTrain = DataSetTrain;
                NeuralNetework.DataSetTest = DataSetTest;
                NeuralNetework.Train();
                NeuralNetework.GetOutput();
                var res = NeuralNetework.Result;
                var trueRes = NeuralNetework.Result.Count(x => x == 1);
                var allCount = NeuralNetework.Result.Count();
                var persent = ((double)trueRes / (double)allCount) * (double)100;
                result.BeginInvoke(new Action(() =>
                {
                    Form.Close();
                    flowLayoutPanel2.Enabled = true;
                    result.Text = $"{persent} %";
                    if (persent == 60)
                    {
                        result.ForeColor = Color.DarkOrange;
                    }
                    else if (persent < 60)
                    {
                        result.ForeColor = Color.Red;
                    }
                    else if (persent > 60)
                    {
                        result.ForeColor = Color.DarkGreen;
                    }
                    customIMagePanel.Visible = true;
                }));

            }));
            Form.Show();
        }

        private void chooseImage_Click(object sender, EventArgs e)
        {
            using (openFileDialog3 = new OpenFileDialog())
            {
                var res = openFileDialog3.ShowDialog();
                if (res != DialogResult.OK)
                    return;
                selectedImagePath.Text = openFileDialog3.FileName;
                var Form = new WaitForm();
                Form.Owner = this;
                Form.StartPosition = FormStartPosition.CenterScreen;
                flowLayoutPanel2.Enabled = false;
                SingleDataSet = new MNISTDataSet(selectedImagePath.Text);
                Task.Run(new Action(() =>
                {
                    var processd = SingleDataSet.ProcessImage();
                    if (processd == null)
                    {
                        MessageBox.Show("INVALID DATA");
                        selectedImagePath.BeginInvoke(new Action(() =>
                        {
                            selectedImagePath.Text = string.Empty;
                        }));
                        return;
                    }
                    selectedImagePath.BeginInvoke(new Action(() =>
                    {
                        selectedImage.Image = Image.FromFile(selectedImagePath.Text);
                        numberpreedivted.Visible = false;
                        numberPerdicted.Text = "_"; 
                        Form.Close();
                        flowLayoutPanel2.Enabled = true;
                    }));
                }));
                Form.Show();

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (NeuralNetework == null)
            {
                MessageBox.Show("THE NEURUAL NETWORK DOES NOT TRAINED");
                return;
            }
            if (SingleDataSet == null)
            {
                MessageBox.Show("INVALID DATA");
                return;
            }
            var Form = new WaitForm(true);
            Form.Owner = this;
            Form.StartPosition = FormStartPosition.CenterScreen;
            flowLayoutPanel2.Enabled = false;
            Task.Run(new Action(() =>
            {
                NeuralNetework.SingleImageDataSet = SingleDataSet;
                var res = NeuralNetework.GetSingleOutput();  
                var persantages = NeuralNetework.GetSinglePersatagedOutput();  
                result.BeginInvoke(new Action(() =>
                {
                    Form.Close();
                    numberpreedivted.Visible = true;
                    is0.Text = $"{persantages[0].ToString().Split('.')[0]} %";
                    is1.Text = $"{persantages[1].ToString().Split('.')[0]} %";
                    is2.Text = $"{persantages[2].ToString().Split('.')[0]} %";
                    is3.Text = $"{persantages[3].ToString().Split('.')[0]} %";
                    is4.Text = $"{persantages[4].ToString().Split('.')[0]} %";
                    is5.Text = $"{persantages[5].ToString().Split('.')[0]} %";
                    is6.Text = $"{persantages[6].ToString().Split('.')[0]} %";
                    is7.Text = $"{persantages[7].ToString().Split('.')[0]} %";
                    is8.Text = $"{persantages[8].ToString().Split('.')[0]} %";
                    is9.Text = $"{persantages[9].ToString().Split('.')[0]} %";
                    flowLayoutPanel2.Enabled = true;
                    numberPerdicted.Text = $"{res}"; 
                }));

            }));
            Form.Show();
        }
    }
}
