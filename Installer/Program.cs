using InstallerHelper;
using Microsoft.Win32;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
namespace Installer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Installer Helper Started ===");

            try
            {
                // Run As Administration
                EnsureAdmin();

                // Check Runtime Installed
                Console.WriteLine("Checking .NET Runtimes...");
                bool ok = await RuntimeInstaller.EnsureRuntimesAsync();

                if (!ok)
                {
                    Console.WriteLine("Runtime setup failed. Please check log...");
                    return;
                }

                Console.WriteLine("All Runtimes verified succesfully. Proceeding with installation...");

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.Message}");
                Console.ResetColor();
            }
            string tempFolder = Path.Combine(Path.GetTempPath(), "ImageCompressorInstallerUI");
            Directory.CreateDirectory(tempFolder);

            string uiExePath = Extract("Setup.exe", tempFolder);

            Console.WriteLine($"Launching UI installer...\n");

            Process.Start(new ProcessStartInfo(uiExePath)
            {
                UseShellExecute = true
            });

            Console.WriteLine("Installer UI launched. You may close this window.");
        }
        public static string Extract(string resourceName, string outputFolder)
        {
            var asm = Assembly.GetExecutingAssembly();

            using Stream resourceStream = asm.GetManifestResourceStream("Installer.EmbeddedFiles." + resourceName);
            if (resourceStream == null)
                throw new Exception($"Embedded resource not found: {resourceName}");

            Directory.CreateDirectory(outputFolder);

            string outPath = Path.Combine(outputFolder, resourceName);

            using FileStream outFile = File.Create(outPath);
            resourceStream!.CopyTo(outFile);

            return outPath;
        }
        static void EnsureAdmin()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                Console.WriteLine("Restarting with administrator privileges...");
                var psi = new ProcessStartInfo
                {
                    FileName = Process.GetCurrentProcess().MainModule.FileName,
                    UseShellExecute = true,
                    Verb = "runas"
                };
                Process.Start(psi);
                Environment.Exit(0);
            }
        }

    }
}
