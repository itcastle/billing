using System;
using System.Management;

namespace GestionCommerciale.DomainModel.Validator
{
    public static class HhdMacSettings
    {

        public static string GetHddSerialNumber(string drive)
        {
           
            if (string.IsNullOrEmpty(drive))
            {
                drive = "C";
            }
            
            ManagementObject disk = new ManagementObject("win32_logicaldisk.deviceid=\"" + drive + ":\"");
            //bind our management object
            disk.Get();
            //return the serial number
            return disk["VolumeSerialNumber"].ToString();
        }

        public static string FindMacAddress()
        {
            
            ManagementClass mgmt = new ManagementClass("Win32_NetworkAdapterConfiguration");
            //create our ManagementObjectCollection to get the attributes with
            ManagementObjectCollection objCol = mgmt.GetInstances();
            string address = String.Empty;
            //loop through all the objects we find
            foreach (var obj in objCol)
            {
                if (address == String.Empty)  // only return MAC Address from first card
                {
                    
                    if ((bool)obj["IPEnabled"]) address = obj["MacAddress"].ToString();
                }
                //dispose of our object
                obj.Dispose();
            }
           
            address = address.Replace(":", "");
            
            return address;
        }
    }
}
