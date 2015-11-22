using System;
using System.IO;

namespace GestionCommerciale.DomainModel.ClassesClients
{
    public class SaveBdd
    { public static void BackupBdd()
    {
        String defaultDirectory = Directory.GetCurrentDirectory();

        String nomBdd = "OpenMind" + DateTime.Now.DayOfYear + "_" + DateTime.Now.Year + ".mdb";
               
        
        const string fileName = "OpenMind.mdb";
        String sourcePath = Directory.GetCurrentDirectory();
        String targetPath = Directory.GetCurrentDirectory();

        // Use Path class to manipulate file and directory paths.
        String sourceFile = Path.Combine(sourcePath, fileName);
        String destFile = Path.Combine(targetPath, nomBdd);

        // To copy a folder's contents to a new location:
        // Create a new target folder, if necessary.
        if (!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
        }

        // To copy a file to another location and 
        // overwrite the destination file if it already exists.
        File.Copy(sourceFile, destFile, true);

        
    }

    }
}
