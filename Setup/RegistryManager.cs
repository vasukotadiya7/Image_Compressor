using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Setup
{
    public class RegistryManager
    {
        private readonly string[] SupportedExtensions =
            { ".jpg", ".jpeg", ".png", ".bmp", ".webp", ".tiff", ".gif", ".ico" };
        private string registryKeyPath = @"SOFTWARE\\Vasu\\ImageCompressor";

        public bool Register(string installDir)
        {
            return AddProgramToRegistry(installDir) && AddContextMenuEntry(installDir);
        }
        public bool Unregister()
        {
            return RemoveProgramFromRegistry() && RemoveContextMenuEntry();
        }
        private bool AddProgramToRegistry(string installDir)
        {
            bool isSuccess = false;
            try
            {

                using (var key = Registry.LocalMachine.CreateSubKey("SOFTWARE\\\\Vasu\\\\ImageCompressor"))
                {
                    key.SetValue("Version", Assembly.GetExecutingAssembly().GetName().Version);
                    key.SetValue("InstallationPath", installDir);
                    key.SetValue("InstallDate", DateTime.Now.ToString());
                }

                //string uninstallExe = Path.Combine(installDir, "Uninstall.exe");

                //using (var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\ImageCompressor"))
                //{
                //    key.SetValue("DisplayName", "Image Compressor");
                //    key.SetValue("Publisher", "Vasu Kotadiya");
                //    key.SetValue("InstallLocation", installDir);
                //    key.SetValue("UninstallString", $"\"{uninstallExe}\"");
                //    key.SetValue("DisplayIcon", uninstallExe);
                //}
                isSuccess = true;
            }
            catch (Exception ex)
            {
                Logger.Log("Adding Registry Entry For Program Is Failed - " + ex.ToString());
            }
            return isSuccess;
        }
        private bool RemoveProgramFromRegistry()
        {
            bool isSuccess = false;
            try

            {
                Registry.LocalMachine.DeleteSubKeyTree(registryKeyPath, false);
                isSuccess = true;
            }
            catch (Exception ex)

            {
                Logger.Log("Program Entry Deletion Failed - " + ex.ToString());
            }
            return isSuccess;
        }
        private bool AddContextMenuEntry(string installDir)
        {
            bool isSuccess = false;
            try
            {
                string popupExePath = Path.Combine(installDir, "ImageCompressor.UI.exe");
                foreach (var ext in SupportedExtensions)
                {
                    using (var key = Registry.ClassesRoot.CreateSubKey($@"SystemFileAssociations\{ext}\shell\Compress Image"))
                    {
                        key!.SetValue("", "Compress Image");
                        key.SetValue("Icon", $"\"{popupExePath}\",0");
                        // in below line ! is very very important 
                        // DOT NOT REMOVE ! FROM BELOW LINE
                        using var cmd = key.CreateSubKey("command");
                        cmd!.SetValue("", $"\"{popupExePath}\" \"%1\"");
                    }
                }
                isSuccess = true;
            }
            catch (Exception ex)
            {
                Logger.Log("Context Menu Registry Entry Creation Failed - " + ex.ToString());
            }
            return isSuccess;
        }
        private bool RemoveContextMenuEntry()
        {
            bool isSuccess = false;
            try
            {

                foreach (var ext in SupportedExtensions)
                {
                    Registry.ClassesRoot.DeleteSubKeyTree(
                        $@"SystemFileAssociations\{ext}\shell\Compress Image", false);
                }
                isSuccess = true;
            }
            catch (Exception ex)
            {
                Logger.Log("Context Menu Entry Deletion Failed - " + ex.ToString());
            }
            return isSuccess;
        }
        public bool IsAlreadyInstalled()
        {
            try
            {
                return Registry.LocalMachine.OpenSubKey(registryKeyPath).GetValueNames().Length > 2;
            }
            catch (Exception ex)
            {
                Logger.Log("Registry Lookup Failed - " + ex.ToString());
                return false;
            }
        }
        public string GetInstalledPath()
        {
            try
            {
                return Convert.ToString(Registry.LocalMachine.OpenSubKey(registryKeyPath).GetValue("InstallationPath"));
            }
            catch (Exception ex)
            {
                Logger.Log("Registry Lookup Failed - " + ex.ToString());
                return string.Empty;
            }
        }

    }
}
