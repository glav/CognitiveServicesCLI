using System.IO;

namespace azCogSvc.CommandLine
{
    public class ComputerVisionOptions : BaseOptions
    {
        public FileInfo Filename { get; set; }

        public override string ToString()
        {
            return $"-- ComputerVision --\n--> {base.ToString()}, Filename: {Filename}";
        }

    }
}
