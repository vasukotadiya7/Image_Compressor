using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCompressorDemo
{

    public class ImageRequest
    {
        public string InputPath { get; set; }
        public int Width { get; set; } = 0;  
        public int Height { get; set; } = 0;
        public long Quality { get; set; } = 75;   
        public long MaxSizeKB { get; set; } = 0;  
    }
    public class Result
    {
        public bool IsSucess { get; set; }
        public string? Message { get; set; }
        public int Quality { get; set; }   
    }
}
