using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace modelbuilder
{
    class Arffgen
    {
        int number;
        List<string> stringlist = new List<string>();
        int[][] inputs;
        int[] outputs;
        public Arffgen(int[][]_inputs, int[]_outputs, int arraycounter)
        {
            inputs = _inputs;
            outputs = _outputs;
            number = inputs.Count();
            for (int i = 0; i < arraycounter; i++)
            {
                read_file_data_values(inputs[i][0], inputs[i][1], inputs[i][2],outputs[i]);
            }
            write_arff_file();
        }
        public void read_file_data_values(int a, int b, int c, int classification)
        {
            string label;
            switch (classification)
            {
                case 0:
                    {
                        label = "ball";
                        break;
                    }
                case 1:
                    {
                        label = "empty";
                        break;
                    }
                case 2:
                    {
                        label = "error";
                        break;
                    }
                default:
                    {
                        label = "ignore";
                        break;
                    }
            }
            string line = (a.ToString() + "," + b.ToString() + "," + c.ToString() + "," + label);
            stringlist.Add(line);

        }


        public void write_arff_file()
        {
            StreamWriter writer = new StreamWriter("output2.arff");
            writer.WriteLine("@RELATION ball");
            writer.WriteLine(" ");
            writer.WriteLine("@ATTRIBUTE whitepixels NUMERIC");
            writer.WriteLine("@ATTRIBUTE blobcount NUMERIC");
            writer.WriteLine("@ATTRIBUTE cornercount NUMERIC");
            writer.WriteLine("@ATTRIBUTE class {ball, empty, error}");
            writer.WriteLine(" ");
            writer.WriteLine("@DATA");
            foreach (string line in stringlist)
            {
                writer.WriteLine(line);
            }


            writer.Close();
        }
    }
}
