using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace annotationParser
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Begin parsing annotations!");

            using (StreamReader r = new StreamReader(@"C:\Users\cedri\Documents\Git\annotationParser\annotations.json"))
            {
                string json = r.ReadToEnd();
                List<Item> items = JsonConvert.DeserializeObject<List<Item>>(json);

                // class x(=absolute x / image width)
                // y (= absolute y / image height)  width(=absolute width / image width) height (=absolute height / image height)

                foreach (var item in items)
                {
                    Console.WriteLine(item.path);

                    string filenamefile = item.path.Substring(15);
                    filenamefile = filenamefile.Replace("png", "txt");
                    string filePath = @"C:\Users\cedri\Documents\ITProject\files_training\v2\ttjkejftmo\annotations\" +
                                      filenamefile;

                    try
                    {
                        using (StreamWriter fileWriter = File.AppendText(filePath))
                        {
                            for (int i = 0; i < item.annotations.Count; i++)
                            {
                                if (item.annotations.Count > 1)
                                {
                                    if (i + 1 == item.annotations.Count)
                                    {
                                        fileWriter.Write(item.annotations[i].ParseReadyToPrint());
                                    }
                                    else
                                    {
                                        fileWriter.WriteLine(item.annotations[i].ParseReadyToPrint());
                                    }
                                }
                                else
                                {
                                    fileWriter.Write(item.annotations[i].ParseReadyToPrint());
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }

            Console.WriteLine("End parsing annotations!");
        }
    }

    //[{"path": "TrainingImages/1.png", "annotations": [{"coordinates": {"height": 60, "width": 60, "x": 661.0, "y": 381.0},
    //"label": "corrosiveold7"}]},
    public class Item
    {
        public string path;
        public List<Annotation> annotations;
    }

    public class Annotation
    {
        public Coordinates coordinates;
        public float parsedX = 0.0f;
        public float parsedY = 0.0f;
        public float parsedHeight = 0.0f;
        public float parsedWidth = 0.0f;
        public string label;


        // class x(=absolute x / image width)
        // y (= absolute y / image height)  width(=absolute width / image width) height (=absolute height / image height)
        public void Parse()
        {
            // width: 1920
            // height: 1080

            parsedX = coordinates.x / 1920.0f;
            parsedY = coordinates.y / 1080.0f;
            parsedWidth = coordinates.width / 1920.0f;
            parsedHeight = coordinates.height / 1080.0f;


            if (parsedX + (parsedWidth / 2) > 1)
                throw new Exception();
            if (parsedY + (parsedHeight / 2) > 1)
                throw new Exception();

            if (parsedX - (parsedWidth / 2) < 0)
                throw new Exception();
            if (parsedY - (parsedHeight / 2) < 0)
                throw new Exception();
        }

        public int GetLabelToInt()
        {
            string charToCheckWichSubstrinToUse = label.Substring(0, 1);

            string newLabelName = "";
            int codeLabel = 99;


            if (charToCheckWichSubstrinToUse == "c")
            {
                newLabelName = label.Substring(0, 12);
            }
            else
            {
                newLabelName = label.Substring(0, 6);
            }

            switch (newLabelName)
            {
                case "corrosivenew":
                    codeLabel = 0;
                    break;
                case "corrosiveold":
                    codeLabel = 1;
                    break;
                case "envnew":
                    codeLabel = 2;
                    break;
                default:
                    throw new Exception("wrong label number");
                    break;
            }

            return codeLabel;
        }

        public string ParseReadyToPrint()
        {
            Parse();
            var numberLabel = GetLabelToInt();

            if (this.parsedX > 1.1)
                throw new Exception("parsedX above 1");

            if (this.parsedY > 1.1)
                throw new Exception("parsedY above 1");

            if (this.parsedWidth > 1.1)
                throw new Exception("parsedWidth above 1");

            if (this.parsedHeight > 1.1)
                throw new Exception("parsedHeight above 1");

            string output = numberLabel + " " + this.parsedX + " " + parsedY + " " + parsedWidth + " " + parsedHeight;
            output = output.Replace(',', '.');
            return output;
        }
    }

    public class Coordinates
    {
        public int height;
        public int width;
        public float x;
        public float y;
    }
}