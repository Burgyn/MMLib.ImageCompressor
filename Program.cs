using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using ImageMagick;
using Kros.Extensions;
using ShellProgressBar;

namespace MMLib.Utils.ImageCompressor
{
    class Program
    {
        static void Main(string[] args) =>
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(OnImagesProcessing);

        private static void OnImagesProcessing(Options o)
        {
            var files = GetFiles(o.SourceFolder, new []{ "jpg", "jpeg", "png", "gif", "tiff", "bmp"}).ToList();
            int totalTicks = files.Count;

            if (files.Any())
            {
                var options = new ProgressBarOptions
                {
                    ProgressCharacter = '─',
                    ProgressBarOnBottom = true
                };

                using(var pbar = new ProgressBar(totalTicks, "Image compressing", options))
                {
                    var destFolder = o.SourceFolder;
                    var copy = false;

                    if (!o.DestinationFolder.IsNullOrWhiteSpace())
                    {
                        destFolder = o.DestinationFolder;
                        copy = true;
                        if (!Directory.Exists(destFolder))
                        {
                            Directory.CreateDirectory(destFolder);
                        }
                    }

                    foreach (var fileName in files)
                    {
                        pbar.Tick(fileName);
                        OnImageProcessing(o, destFolder, copy, fileName);
                    }
                }
            }
        }

        private static void OnImageProcessing(Options o, string destFolder, bool copy, string fileName)
        {
            var source = Path.Combine(o.SourceFolder, fileName);
            var filePath = Path.Combine(destFolder, fileName);

            CopyFile(copy, source, filePath);

            var file = new FileInfo(filePath);
            var optimizer = new ImageOptimizer();
            optimizer.Compress(file);

            MagickImage image = new MagickImage(file);
            image.Resize(new Percentage(o.Percentage));

            image.Write(file);
        }

        private static void CopyFile(bool copy, string source, string filePath)
        {
            if (copy)
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                File.Copy(source, filePath);
            }
        }

        private static IEnumerable<string> GetFiles(string folder, IEnumerable<string> extensions)
        {
            foreach (var extension in extensions)
            {
                foreach (var file in Directory
                        .GetFiles(folder, $"*.{extension}", SearchOption.TopDirectoryOnly)
                        .Select(f => Path.GetFileName(f)))
                {
                    yield return file;
                }
            }
        }
    }
}