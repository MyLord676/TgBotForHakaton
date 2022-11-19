using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartUp.FileUtilities
{
    public static class FileNameBuilder
    {
        public static string constructUniqueFileName(string fileParticularName, string resultFileDirectoryPath, string fileFormat)
        {
            var resNumber = 0;
            var startDT = DateTime.Now.ToString("dd-MM-yy HH-mm-ss");
            var resultFileName = Path.Combine(resultFileDirectoryPath, string.Format(@"{0}{1}({2}).{3}", fileParticularName, startDT, resNumber, fileFormat));
            while ((new FileInfo(resultFileName)).Exists)
            {
                resNumber++;
                resultFileName = Path.Combine(resultFileDirectoryPath, string.Format(@"{0}{1}({2}).{3}", fileParticularName, startDT, resNumber, fileFormat));
            }
            return resultFileName;
        }
    }
}
