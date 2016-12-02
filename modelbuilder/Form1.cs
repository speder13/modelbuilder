using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Accord.Imaging;

namespace modelbuilder
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            //streaming = new StreamWriter("model.dat");
            trainingOutput = new int[2536];
            trainingInput = new int[2536][];
            InitializeComponent();
            run_shit();

        }
        #region Properties  
        StreamWriter streaming;
        int blobs = 0;
        int corners = 0;
        int whitePixels = 0;
        int counter;
        string[] emptyImages;
        string[] ballImages;
        string[] errors;
        Bitmap currentBitmap;
        Accord.Imaging.Converters.ImageToArray arrayMaker = new Accord.Imaging.Converters.ImageToArray();
        Accord.Imaging.Converters.ArrayToImage imageMaker = new Accord.Imaging.Converters.ArrayToImage(400, 200);
        Accord.MachineLearning.Bayes.NaiveBayesLearning naiveBayes;
        Accord.MachineLearning.Bayes.NaiveBayes nb;
        Dictionary<int, int> backgrounds = new Dictionary<int, int>();
        int[] trainingOutput;
        int[][] trainingInput;
        #endregion Properties
        public void run_shit()
        {
            naiveBayes = new Accord.MachineLearning.Bayes.NaiveBayesLearning();
            load_dictionary();
            //load_files();
            //load_ball_training();
            //load_empty_training();
            //load_error_training();
            //Arffgen arffgen = new Arffgen(trainingInput,trainingOutput,arrayCounter);
            load_model();
            build_model();
            //save_model();

        }

        public void load_dictionary()
        {
            StreamReader sr = new StreamReader("output.txt");
            List<int> liste = new List<int>();
            string val;
            while (!sr.EndOfStream)
            {
                val = sr.ReadLine();
                liste.Add(int.Parse(val));
            } 

            foreach (int value in liste)
            {
                backgrounds.Add(value,value);
            }
            sr.Close();
        }
        public void load_files()
        {
            emptyImages = Directory.GetFiles("images/Empty/", "*.*");
            ballImages = Directory.GetFiles("images/Ball/", "*.*");
            errors = Directory.GetFiles("images/Error/", "*.*");
            //MessageBox.Show(emptyImages[13] + " " + emptyImages[69]);
        }

        private System.Drawing.Bitmap clean_background(System.Drawing.Image inputImage)
        {
            int i = 0;
            Bitmap bitmap = new Bitmap(inputImage);
            Color[] colors = new Color[80000];
            Color[] newImage = new Color[80000];

            whitePixels = 0;
            arrayMaker.Convert(bitmap, out colors);

            foreach (Color farve in colors)
            {
                if (backgrounds.ContainsKey(farve.ToArgb())){
                    newImage[i] = Color.Black;
                }
                else{
                    newImage[i] = Color.White;
                    whitePixels++;
                }
                i++;
            }
            imageMaker.Convert(newImage, out currentBitmap);
            corners = cornerdetect(currentBitmap);
            blobs = blobdetect(currentBitmap);
            return currentBitmap;
        }

        public void load_ball_training()
        {
            foreach (string fil in ballImages)
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(fil);
                currentBitmap = clean_background(image);
                streaming.WriteLine(whitePixels + "|" + blobdetect(currentBitmap) + "|" + cornerdetect(currentBitmap) + "|0" );
            }
        }

        public void load_empty_training()
        {
            foreach (string fil in emptyImages)
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(fil);
                currentBitmap = clean_background(image);
                streaming.WriteLine(whitePixels + "|" + blobdetect(currentBitmap) + "|" + cornerdetect(currentBitmap) + "|1");

            }
        }

        public void load_error_training()
        {
            foreach (string fil in errors)
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(fil);
                currentBitmap = clean_background(image);
                streaming.WriteLine(whitePixels + "|" + blobdetect(currentBitmap) + "|" + cornerdetect(currentBitmap) + "|2");

            }
        }    
        public int blobdetect(Bitmap input)
            {
                Accord.Imaging.BlobCounter blobs = new BlobCounter();
                blobs.FilterBlobs = true;
                blobs.BlobsFilter = null;
                blobs.MinHeight = 35;
                blobs.MinWidth = 35;

                blobs.ProcessImage(input);

                return (blobs.ObjectsCount);
            }

        public int cornerdetect(Bitmap input)
            {
                Accord.Imaging.SusanCornersDetector susanCorners = new SusanCornersDetector(1,10);
                List<Accord.IntPoint> points = susanCorners.ProcessImage(input);
                return points.Count;
            }

        public void build_model()
        {
            nb = naiveBayes.Learn(trainingInput,trainingOutput);
        }
        public void save_model()
        {
            StreamWriter swOut = new StreamWriter("output.ara");
            swOut.WriteLine(trainingOutput.Count());
            for (int i = 0; i < trainingOutput.Count(); i++)
            {
                swOut.WriteLine(trainingOutput[i]);
            }
            StreamWriter swIn = new StreamWriter("input.ara");
            swIn.WriteLine(trainingInput.Count());
            for (int i = 0; i < trainingInput.Count(); i++)
            {
                swOut.WriteLine(trainingInput[i][0] +"|"+ trainingInput[i][1] + "|" + trainingInput[i][2]);
            }
        }
        public void load_model()
        {
            string sti = Directory.GetCurrentDirectory();
            StreamReader incstream = new StreamReader("model.dat");
            List<string> datastrings = new List<string>();
            while (!incstream.EndOfStream)
            {
                datastrings.Add(incstream.ReadLine());
            }
            trainingInput = new int[datastrings.Count()][];
            trainingOutput = new int[datastrings.Count()];
            string[] temp = new string[4];
            counter = 0;
            foreach (string linje in datastrings)
            {
                temp = linje.Split('|');
                MessageBox.Show(temp[0] +" "+ temp[1]);
                trainingInput[counter][0] = int.Parse(temp[0]);
                trainingInput[counter][1] = int.Parse(temp[1]);
                trainingInput[counter][2] = int.Parse(temp[2]);
                trainingOutput[counter] = int.Parse(temp[3]);
                counter++;
            }
            incstream.Close();
        }

        
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            //streaming.Close();
           
            Application.Exit();
        }
    }
}
