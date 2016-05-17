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
        static void Main(string[] args)
        {
            if(args.Length < 2)
            {
                PrintUsage();
                return;
            }

            try
            {
                DownloadImages(args);
            }
            catch
            {
                PrintUsage();
            }
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage: imagedownloader prefix suffix [-p padding] [-o directory] [-s start] [-e end]");
            Console.WriteLine("\tprefix: the prefix string for files, usually the directory.");
            Console.WriteLine("\tsuffix: the suffix string for files, usually the file extension.");
            Console.WriteLine("\tpadding: amount of zeros to pad the index with. Default: 0.");
            Console.WriteLine("\tdirectory: the directory to save files to. If no directory is specified, files will be saved to the current directory.");
            Console.WriteLine("\tstart: start index. Default: 0.");
            Console.WriteLine("\tend: end index. If the end is set, the program will try to download every file in sequence in spite of errors. If not, the program will terminate when the first error is encountered");
            Console.WriteLine("Example: imageownloader example.com/images/ .jpg -p 3 -s 5 -e 10 will download images between 005.jpg and 010.jpg to the current directory.");
        }

        private static void DownloadImages(string[] args)
        {
            string prefix = args[0];
            string suffix = args[1];
            int padding = 0;
            string directory = string.Empty;
            int start = 0;
            int end = 0;
            bool defaultFlag = false;
            for (int i = 2; i < args.Length; i += 2)
            {
                switch(args[i])
                {
                    case "-p":
                        padding = int.Parse(args[i + 1]);
                        break;
                    case "-o":
                        directory = args[i + 1];
                        break;
                    case "-s":
                        start = int.Parse(args[i + 1]);
                        break;
                    case "-e":
                        end = int.Parse(args[i + 1]);
                        break;
                    default:
                        defaultFlag = true;
                        break;
                }
            }

            if (defaultFlag)
            {
                PrintUsage();
            }
            else
            {
                DownloadImages(prefix, suffix, padding, directory, start, end);
            }
        }

        private static void DownloadImages(string prefix, string suffix, int padding = 0, string directory = "", int start = 0, int end = 0)
        {
            if (String.IsNullOrWhiteSpace(directory))
            {
                directory = Directory.GetCurrentDirectory();
            }
            else
            {
                CreateDirectoryIfNecessary(directory);
            }

            String format = new String('0', padding);

            int downloaded = 0; 

            WebClient client = new WebClient();
            Uri imageUri = null;
            for (int i = start; end == 0 || i<=end; i++)
            {
                try
                {
                    imageUri = new Uri(prefix + (padding > 0 ? i.ToString(format) : i.ToString()) + ".jpg");
                    string destination =  new StringBuilder(directory).Append(Path.DirectorySeparatorChar).Append(i).Append(suffix).ToString();
                    client.DownloadFile(imageUri, destination);
                    Console.WriteLine("Downloaded " + imageUri + " to " + destination);
                    downloaded++;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not download " + imageUri);
                    if (end == 0)
                    {
                        break;
                    }
                }
            }

            if(downloaded == 0)
            {
                Console.WriteLine("No images downloaded. Please ensure that the parameters specified are valid.");
            } else
            {
                WriteSource(prefix, directory);
                Console.WriteLine(downloaded + " images downloaded");
            }
        }

        private static void CreateDirectoryIfNecessary(string directory)
        { 
            DirectoryInfo userDir = new DirectoryInfo(directory);
            if (!userDir.Exists)
            {
                Directory.CreateDirectory(directory);
            }
        }

        private static void WriteSource(string prefix, string directory)
        {
            FileInfo source = new FileInfo(new StringBuilder(directory).Append(Path.DirectorySeparatorChar).Append("source.txt").ToString());

            StreamWriter writer = new StreamWriter(source.OpenWrite());
            writer.WriteLine(prefix);
            writer.Close();
        }
    }
}