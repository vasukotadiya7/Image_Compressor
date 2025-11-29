using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;

namespace Setup
{
    public class ProcessUtils
    {
        public static void EnsureAdmin()
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
        public static void RunProcess(string fileName, string args)
        {
            Logger.Log($"[INFO] Running: {fileName} {args}");
            var psi = new ProcessStartInfo(fileName, args)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using var process = Process.Start(psi);
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (!string.IsNullOrWhiteSpace(output))
                Logger.Log($"[INFO] {output}");
            if (!string.IsNullOrWhiteSpace(error))
                Logger.Log($"[ERROR] Output: {error}");

            if (process.ExitCode != 0)
                throw new Exception($"Process {fileName} exited with code {process.ExitCode}");
        }
        public static bool ExtractAllResources(List<string> resName, string outputFolder)
        {
            bool isSuccess = false;
            try
            {
                Directory.CreateDirectory(outputFolder);

                var asm = Assembly.GetExecutingAssembly();
                foreach (string res in resName)
                {
                    using var stream = asm.GetManifestResourceStream("Setup.EmbeddedFiles." + res);
                    if (stream == null)
                    {
                        Logger.Log($"Resource File Not Found From Manifest ({res})");
                        return isSuccess;
                    }
                    string filePath = Path.Combine(outputFolder, res);

                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    using var outStream = File.Create(filePath);
                    stream.CopyTo(outStream);
                }
                isSuccess = true;
            }
            catch (Exception ex)
            {
                Logger.Log("Failed To Copy Files From Exe - " + ex.ToString());
            }
            return isSuccess;
        }
        public static bool DeleteAppFiles(string baseDir)
        {
            bool isSuccess = false;
            string currentExe = Process.GetCurrentProcess().MainModule!.FileName!;
            string[] filesToDelete = Directory.GetFiles(baseDir);

            try
            {
                foreach (string fileName in filesToDelete)
                {
                    try
                    {
                        string filePath = Path.Combine(baseDir, fileName);
                        if (File.Exists(filePath))
                        {
                            if (string.Equals(filePath, currentExe, StringComparison.OrdinalIgnoreCase))
                            {
                                Logger.Log($"Skipping self file: {fileName}");
                                continue;
                            }

                            File.Delete(filePath);
                            Logger.Log($"Deleted file: {fileName}");
                        }
                        else
                        {
                            Logger.Log($"File not found (skipped): {fileName}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Failed to delete {fileName}: {ex.Message}");
                    }
                }
                isSuccess = true;

                //string[] remainingFiles = Directory.GetFiles(baseDir);
                //if (remainingFiles.Length <= 1) // only this exe left
                //{
                //    string deleteCmd = $"cmd /c timeout 2 && del \"{Path.Combine(baseDir,"Uninstaller.exe")}\"";
                //    Process.Start("cmd.exe", deleteCmd);
                //    Logger.Log("Scheduled self deletion after exit.");
                //}
            }
            catch (Exception ex)
            {
                Logger.Log($"Failed to delete file - {ex.Message}");
            }
            return isSuccess;
        }

    }
}
