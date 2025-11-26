namespace ImageCompressorDemo
{
    internal class Program
    {

        static void Main(string[] args)
        {
           ImageRequest request = new ImageRequest()
           {
               InputPath = @"E:\Vasu\StockImages\1.png",
               MaxSizeKB = 5000
           };
            Compression compression = new Compression();
            var result = compression.CompressImage(request);
        }
        
    }
}
