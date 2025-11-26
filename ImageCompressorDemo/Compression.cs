using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Cryptography;
using System.Text;

namespace ImageCompressorDemo
{
    public class Compression
    {
        public const string IMAGE_COMPRESSION = "ImageCompresion";
        public const string PRE_CONVERTED = "PreConverted";

        public string GetPreConvertedPath(string inputPath)
        {

            string extension = Path.GetExtension(inputPath);
            string tempPath = GetTempPathForPreConversion(PRE_CONVERTED, inputPath);
            Console.WriteLine($"Input Path : {inputPath} \r\nOutput Path : {tempPath}");
            string fileName = Path.GetFileNameWithoutExtension(inputPath);
            string preConvertedOutputPath = tempPath + "\\" + fileName + ".jpeg";
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }

            if (extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
               extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase))
            {
                File.Copy(inputPath, preConvertedOutputPath, true);
            }
            else
            {
                using (Image image = Image.FromFile(inputPath))
                {
                    SaveJpeg(image, preConvertedOutputPath);
                }
            }
            return preConvertedOutputPath;
        }



        public void SaveJpeg(Image image, string outputPath, long quality = 100L)
        {

            ImageCodecInfo jpegCodec = ImageCodecInfo.GetImageEncoders()
                .First(codec => codec.FormatID == ImageFormat.Jpeg.Guid);
            var encoder = System.Drawing.Imaging.Encoder.Quality;
            var encoderParameter = new EncoderParameter(encoder, quality); 
            var encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = encoderParameter;

            image.Save(outputPath, jpegCodec, encoderParameters);

        }
        private void SaveJpeg(Image img, Stream output, long quality)
        {
            ImageCodecInfo jpegCodec = ImageCodecInfo.GetImageEncoders()
                .First(codec => codec.FormatID == ImageFormat.Jpeg.Guid);
            var encoder = System.Drawing.Imaging.Encoder.Quality;
            var encoderParameter = new EncoderParameter(encoder, quality); 
            var encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = encoderParameter;
            img.Save(output, jpegCodec, encoderParameters);
        }
        private string GetHashFromPath(string input)
        {
            using (var sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
        private string GetTempPathForPreConversion(string stage, string inputPath)
        {
            return Path.Combine(Path.GetTempPath(), IMAGE_COMPRESSION, stage, GetHashFromPath(inputPath));
        }
        public Result CompressImage(ImageRequest request)
        {
            Result result = new Result()
            {
                IsSucess = false,
                Message = string.Empty,
                Quality = 0
            };
            string path = request.InputPath;
            if (File.Exists(path))
            {
                if (Path.HasExtension(path))
                {

                    string outputPath = Path.Combine(Directory.GetParent(request.InputPath).FullName, Path.GetFileNameWithoutExtension(request.InputPath) + "_compressed") + ".jpeg";

                    request.InputPath = GetPreConvertedPath(request.InputPath);

                    using (var image = Image.FromFile(request.InputPath))
                    {
                        int targetWidth = request.Width > 0 ? request.Width : image.Width;
                        int targetHeight = request.Height > 0 ? request.Height : image.Height;

                        using (var resized = new Bitmap(image, new Size(targetWidth, targetHeight)))
                        {
                            if (request.MaxSizeKB > 0)
                            {
                                long quality = FindQualityForSize(resized, request.MaxSizeKB, outputPath);
                                if(quality > 0)
                                {
                                    result.IsSucess = true;
                                    result.Quality = (int)quality;
                                }
                                else
                                {
                                    result.Message = "Failed to compress image to desired size";
                                }
                                return result;
                            }
                            else
                            {
                                //TODO:error throw
                                result.Message = "Max Size not specified";
                                return result;
                            }
                        }
                    }
                }
                else
                {
                    //Get Extension Any How
                    result.Message = "Invalid File";
                    return result;
                }
            }
            else
            {
                //File not found
                result.Message = "File Not Found";
                return result;
            }
        }

        private long FindQualityForSize(Bitmap img, long maxSizeKB, string outputPath)
        {
            long minQ = 5;   
            long maxQ = 100;
            long bestQ = 75;
            while (minQ <= maxQ)
            {
                Console.WriteLine($"Searching for quality to fit under {maxSizeKB} KB...");
                Console.WriteLine($"Current Quality Min - {minQ} , Max - {maxQ}");
                long midQ = (minQ + maxQ) / 2;

                using (var ms = new MemoryStream())
                {
                    SaveJpeg(img, ms, midQ);

                    long sizeKB = ms.Length / 1024;

                    if (sizeKB > maxSizeKB)
                    {
                        maxQ = midQ - 1;
                    }
                    else
                    {
                        bestQ = midQ;
                        minQ = midQ + 1;
                    }
                }
            }

            if (File.Exists(outputPath))
            {
                for (int i = 1; true; i++)
                {
                    string tmpTryPath = Path.Combine(Directory.GetParent(outputPath).FullName, Path.GetFileNameWithoutExtension(outputPath).Replace("_compressed", $"_compressed ({i})")) + ".jpeg";
                    if (!File.Exists(tmpTryPath))
                    {
                        outputPath = tmpTryPath;
                        break;
                    }
                }
            }
            SaveJpeg(img, outputPath, bestQ);
            return bestQ;
        }

    }
}
