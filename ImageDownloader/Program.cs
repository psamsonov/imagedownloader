using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ImageDownloader
{
    class Program
    {
         const string USERDIR = @"C:\Users\Peter\Pictures\Article images\ISU-122S Data\";
         const string EXTENSION = ".jpg";
         const string SOURCE = "http://sovdoc.rusarchives.ru/Final_s/gko/op2/6430/%D0%A4644%D0%9E%D0%BF2%D0%94377%D0%9B";

        static void Main(string[] args)
        {

            DirectoryInfo userDir = new DirectoryInfo(USERDIR);
            if (!userDir.Exists)
            {
                Directory.CreateDirectory(USERDIR);
            }


            WebClient client = new WebClient();
            for (int i = 6; ; i++)
            {
                try
                {
                    Uri imageUri = new Uri(SOURCE + i.ToString("000") + ".jpg");
                    client.DownloadFile(imageUri, USERDIR + i + EXTENSION);
                    Console.WriteLine("Downloaded image #" + i);
                }
                catch (Exception e)
                {
                    break;
                }
            }

            FileInfo source = new FileInfo(USERDIR + "source.txt");
           
            StreamWriter writer = new StreamWriter(source.OpenWrite());
            writer.WriteLine(SOURCE);
            writer.Close();


            Console.ReadKey();
        }
    }
}