using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace InstallerHelper
{
    public static class RuntimeInstaller
    {
        private static readonly string logFile = Path.Combine(Path.GetTempPath(), "runtime_installer.log");

        public static async Task<bool> EnsureRuntimesAsync()
        {
            Log("==== .NET Runtime Check Started ====");

            try
            {
                bool dotnetOk = IsRuntimeInstalled("Microsoft.NETCore.App", "8.");
                bool aspnetOk = IsRuntimeInstalled("Microsoft.AspNetCore.App", "8.");
                bool desktopnetOk = IsRuntimeInstalled("Microsoft.WindowsDesktop.App", "8.");

                if (dotnetOk && aspnetOk && desktopnetOk)
                {
                    Log(".NET 8 , DesktopApp .NET 8 And ASP.NET Core 8 runtimes are installed.");
                    CleanupLog();
                    return true;
                }

                Log($"Missing runtimes - .NET: {dotnetOk}, ASP.NET: {aspnetOk}, DESKTOP-RUNTIME: {desktopnetOk}");

                string arch = GetArchitecture();
                Log($"System architecture detected: {arch}");

                if (!IsInternetAvailable())
                {
                    Log("No internet connection available.");
                    return false;
                }

                // Download missing runtimes
                if (!dotnetOk)
                    await InstallRuntimeAsync(GetDotnetRuntimeUrl(arch), "dotnet-runtime-8-installer.exe");

                if (!aspnetOk)
                    await InstallRuntimeAsync(GetAspnetRuntimeUrl(arch), "aspnet-runtime-8-installer.exe");

                if(!desktopnetOk)
                    await InstallRuntimeAsync(GetDesktopnetRuntimeUrl(arch), "desktop-runtime-8-installer.exe");

                // Verify again
                dotnetOk = IsRuntimeInstalled("Microsoft.NETCore.App", "8.");
                aspnetOk = IsRuntimeInstalled("Microsoft.AspNetCore.App", "8.");
                desktopnetOk = IsRuntimeInstalled("Microsoft.WindowsDesktop.App", "8.");

                if (dotnetOk && aspnetOk && desktopnetOk)
                {
                    Log("All runtimes successfully installed.");
                    CleanupLog();
                    return true;
                }

                Log("Runtime installation failed. Check manually.");
                return false;
            }
            catch (Exception ex)
            {
                Log($"ERROR: {ex}");
                return false;
            }
        }

        private static bool IsRuntimeInstalled(string runtimeName, string versionPrefix)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "--list-runtimes",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                var proc = Process.Start(psi);
                string output = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit();

                return output.Contains($"{runtimeName} {versionPrefix}");
            }
            catch
            {
                return false;
            }
        }

        private static string GetArchitecture()
        {
            if (RuntimeInformation.OSArchitecture == Architecture.X64)
                return "x64";
            if (RuntimeInformation.OSArchitecture == Architecture.X86)
                return "x86";
            if (RuntimeInformation.OSArchitecture == Architecture.Arm64)
                return "arm64";
            return "x64"; 
        }

        private static string GetDotnetRuntimeUrl(string arch)
        {
            return arch switch
            {
                "x64" => "https://builds.dotnet.microsoft.com/dotnet/Runtime/8.0.20/dotnet-runtime-8.0.20-win-x64.exe",
                "x86" => "https://builds.dotnet.microsoft.com/dotnet/Runtime/8.0.20/dotnet-runtime-8.0.20-win-x86.exe",
                "arm64" => "https://builds.dotnet.microsoft.com/dotnet/Runtime/8.0.20/dotnet-runtime-8.0.20-win-arm64.exe",
                _ => ""
            };
        }

        private static string GetAspnetRuntimeUrl(string arch)
        {
            return arch switch
            {
                "x64" => "https://builds.dotnet.microsoft.com/dotnet/aspnetcore/Runtime/8.0.20/aspnetcore-runtime-8.0.20-win-x64.exe",
                "x86" => "https://builds.dotnet.microsoft.com/dotnet/aspnetcore/Runtime/8.0.20/aspnetcore-runtime-8.0.20-win-x86.exe",
                "arm64" => "https://builds.dotnet.microsoft.com/dotnet/aspnetcore/Runtime/8.0.20/aspnetcore-runtime-8.0.20-win-arm64.exe",
                _ => ""
            };
        }

        private static string GetDesktopnetRuntimeUrl(string arch)
        {
            return arch switch
            {
                "x64" => "https://builds.dotnet.microsoft.com/dotnet/WindowsDesktop/8.0.20/windowsdesktop-runtime-8.0.20-win-x64.exe",
                "x86" => "https://builds.dotnet.microsoft.com/dotnet/WindowsDesktop/8.0.20/windowsdesktop-runtime-8.0.20-win-x86.exe",
                "arm64" => "https://builds.dotnet.microsoft.com/dotnet/WindowsDesktop/8.0.20/windowsdesktop-runtime-8.0.20-win-arm64.exe",
                _ => ""
            };
        }

        private static async Task InstallRuntimeAsync(string url, string fileName)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                Log($"Runtime URL missing for {fileName}");
                return;
            }

            string filePath = Path.Combine(Path.GetTempPath(), fileName);
            Console.WriteLine($"Downloading: {url}");
            Log($"Downloading: {url}");

            using (var wc = new WebClient())
            {
                await wc.DownloadFileTaskAsync(url, filePath);
            }

            Console.WriteLine($"Installing: {filePath}");
            Log($"Installing: {filePath}");
            var proc = Process.Start(new ProcessStartInfo
            {
                FileName = filePath,
                //Arguments = "/quiet /norestart",
                UseShellExecute = true,
                Verb = "runas"
            });

            proc.WaitForExit();
            Log($"{fileName} exited with code {proc.ExitCode}");
        }

        private static bool IsInternetAvailable()
        {
            try
            {
                return NetworkInterface.GetIsNetworkAvailable();
            }
            catch { return false; }
        }

        private static void Log(string message)
        {
            File.AppendAllText(logFile, $"[{DateTime.Now}] {message}{Environment.NewLine}");
        }

        private static void CleanupLog()
        {
            try
            {
                if (File.Exists(logFile))
                {
                    Log("Installer succeeded. Deleting log file.");
                    File.Delete(logFile);
                }
            }
            catch
            {
            }
        }
    }
}
