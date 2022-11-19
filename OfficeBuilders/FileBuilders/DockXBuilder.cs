using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StartUp.FileUtilities;
using Word = Microsoft.Office.Interop.Word;

namespace StartUp.FileBuilders
{
    public static class DockXBuilder
    {
        public static FileInfo? Build(string templateFilePath, string fileParticularName, Dictionary<string, object> questions)
        {
            if (!File.Exists(templateFilePath))
                throw new FileNotFoundException("Template File Not Found by path " + templateFilePath);
            string resultFileName = String.Empty;
            Word.Application? wordApp = null;
            try
            {
                var templateFileInfo = new FileInfo(templateFilePath);
                string templateFileName = templateFileInfo.FullName;
                var parentDirectory = templateFileInfo.Directory?.Parent?.FullName;
                DirectoryInfo resultFileDirectory;
                string resultFileDirectoryPath;
                if (parentDirectory != null)
                {
                    resultFileDirectoryPath = Path.Combine(parentDirectory, "TempFiles");
                    resultFileDirectory = new DirectoryInfo(resultFileDirectoryPath);
                    if (!resultFileDirectory.Exists)
                        resultFileDirectory.Create();
                }
                else throw new Exception("Result file path construction error");
                resultFileName = FileNameBuilder.constructUniqueFileName(fileParticularName, resultFileDirectoryPath, "docx");
                File.Copy(templateFileName, resultFileName);
                wordApp = new Word.Application();
                wordApp.Documents.Open(resultFileName);
                object missing = Type.Missing;
                foreach (var q in questions)
                {
                    var ranges = getTagRanges(wordApp, q.Key);
                    if (q.Value is string)
                        replace(ranges, (string)q.Value);
                    if (q.Value is Image)
                        replace(wordApp, resultFileDirectoryPath, ranges, (Image)q.Value);
                }
                wordApp.ActiveDocument.Save();
            }
            catch{ }
            finally
            {
                wordApp?.Quit();
            }
            if (resultFileName != null && resultFileName != String.Empty)
            {
                var sumFile = new FileInfo(resultFileName);
                return sumFile.Exists ? sumFile : null;
            }
            return null;
        }

        private static List<Word.Range?> getTagRanges(Word.Application? wordApp, string tag)
        {
            object searchTagObj = tag;
            object missing = Type.Missing;
            var result = new List<Word.Range?>();
            var startsFrom = 0;
            while (true)
            {
                Word.Range? wordRange = wordApp?.ActiveDocument.Content;
                if (wordRange == null)
                    return result;
                wordRange.Start = startsFrom;
                wordRange.Find.Execute(
                ref searchTagObj, ref missing, ref missing, ref missing, ref missing,
                ref missing, ref missing, ref missing, ref missing, ref missing,
                ref missing, ref missing, ref missing, ref missing, ref missing);
                if (!wordRange.Find.Found)
                    break;
                startsFrom = wordRange.Start + 1;
                result.Add(wordRange);
            }
            return result;
        }

        private static void replace(List<Word.Range?> ranges, string replacementValue)
        {
            foreach (var range in ranges)
                if(range != null)
                    range.Text = replacementValue;
        }

        private static void replace(Word.Application? wordApp, string tempFilesDirectoryFullPath, List<Word.Range?> ranges, Image image)
        {
            var imagePath = FileNameBuilder.constructUniqueFileName("tempImage", tempFilesDirectoryFullPath, image.RawFormat.ToString().ToLower());
            image?.Save(imagePath, image.RawFormat);
            var imgFileInfo = new FileInfo(imagePath);
            if (!(imgFileInfo).Exists)
                throw new Exception("Creating image temp file error");
            foreach (var range in ranges)
            {
                if(range != null)
                {
                    range.Text = String.Empty;
                    wordApp?.Selection.InlineShapes.AddPicture(imagePath, Range: range);
                }
            }
            imgFileInfo.Delete();
        }
    }
}
