using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Setup
{
    public class ServiceManager
    {
        private string serviceName = "ImageCompressorWorkerService";
        public bool RegisterService(string installDir)
        {
            bool isSuccess = false;
            string workerExePath = Path.Combine(installDir, "CompressionService.exe");
            try
            {
                ProcessUtils.RunProcess("sc.exe", $"create {serviceName} binPath= \"{workerExePath}\" start= auto");
                isSuccess = true;
            }
            catch (Exception ex)
            {
                Logger.Log("Service Creation Failed - " + ex.ToString());
            }

            return isSuccess;
        }
        public bool RemoveService()
        {
            bool isSuccess = false;
            try
            {
                ProcessUtils.RunProcess("sc.exe", $"delete {serviceName}");
                isSuccess = true;
            }
            catch (Exception ex)
            {
                Logger.Log("Service Deletion Failed - " + ex.ToString());
            }

            return isSuccess;

        }
        public bool StartService()
        {
            bool isSuccess = false;
            try
            {
                ServiceController sc = new ServiceController(serviceName);
                sc.Start();
                sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                isSuccess = true;
            }
            catch (Exception ex)
            {
                Logger.Log("Unable to start service - " + ex.ToString());
            }
            return isSuccess;
        }
        public bool StopService()
        {
            bool isSuccess = false;
            try
            {
                ServiceController sc = new ServiceController(serviceName);
                if (sc.Status != ServiceControllerStatus.Stopped &&
                    sc.Status != ServiceControllerStatus.StopPending)
                {
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
                }
                isSuccess = true;
            }
            catch (Exception ex)
            {
                Logger.Log("Unable to stop service - " + ex.ToString());
            }
            return isSuccess;
        }
    }
}
