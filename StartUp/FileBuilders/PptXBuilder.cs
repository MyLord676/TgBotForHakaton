using Microsoft.Office.Core;
using StartUp.FileUtilities;
using System.Drawing;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace StartUp.FileBuilders
{
    public static class PptXBuilder
    {
        public static FileInfo? Build(string templateFilePath, string fileParticularName, Dictionary<string, object> questions)
        {
            if (!File.Exists(templateFilePath))
                throw new FileNotFoundException("Template File Not Found by path " + templateFilePath);
            string resultFileName = String.Empty;
            PowerPoint.Application? powerPointApp = null;
            PowerPoint.Presentation presentation = null;
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
                resultFileName = FileNameBuilder.constructUniqueFileName(fileParticularName, resultFileDirectoryPath, "pptx");
                File.Copy(templateFileName, resultFileName);
                powerPointApp = new PowerPoint.Application();
                //powerPointApp = new PowerPoint.Application();
                presentation = powerPointApp.Presentations.Open(resultFileName, MsoTriState.msoFalse, MsoTriState.msoFalse, MsoTriState.msoFalse);
                object missing = Type.Missing;
                foreach (PowerPoint.Slide slide in presentation.Slides)
                {
                    var count = slide.Shapes.Count;
                    for (int i = 1; i <= count; i++)
                    {
                        var shape = slide.Shapes[i];
                        foreach (var q in questions)
                        {
                            ReplaceTag(slide, shape, q.Key, q.Value, resultFileDirectoryPath);
                        }
                    }
                }
                presentation.Save();
            }
            catch (Exception e) { Console.WriteLine(e); }
            finally
            {
                try
                {
                    if (presentation != null)
                        presentation.Close();
                }
                catch (Exception e) { Console.WriteLine(e); }
                powerPointApp?.Quit();
            }
            if (resultFileName != null && resultFileName != String.Empty)
            {
                var sumFile = new FileInfo(resultFileName);
                return sumFile.Exists ? sumFile : null;
            }
            return null;
        }

        private static void ReplaceTag(PowerPoint.Slide slide, PowerPoint.Shape shape, string searchTag, object replaceValue, string tempFilesDirectoryFullPath)
        {
            MsoShapeType type;
            try
            {
                type = shape.Type;
            }
            catch
            {
                type = MsoShapeType.msoTextBox;
            }
            switch (type)
            {
                case MsoShapeType.msoTextBox:
                    {
                        if (replaceValue is string)
                            shape.TextFrame.TextRange.Text = shape.TextFrame.TextRange.Text.Replace(searchTag, (string)replaceValue);
                        else if (replaceValue is Image)
                        {
                            if (!shape.TextFrame.TextRange.Text.Contains(searchTag))
                                break;
                            shape.TextFrame.TextRange.Text = String.Empty;
                            var image = (Image)replaceValue;
                            var imagePath = FileNameBuilder.constructUniqueFileName("tempImage", tempFilesDirectoryFullPath, image.RawFormat.ToString().ToLower());
                            image?.Save(imagePath, image.RawFormat);
                            var imgFileInfo = new FileInfo(imagePath);
                            if (!(imgFileInfo).Exists)
                                throw new Exception("Creating image temp file error");
                            float imgWidth = 0;
                            float imgHeight = 0;
                            if ((float)image.Width / image.Height > shape.Width / shape.Height)
                            {
                                imgWidth = shape.Width;
                                imgHeight = imgWidth * image.Height / image.Width;
                            }
                            else
                            {
                                imgHeight = shape.Height;
                                imgWidth = imgHeight * image.Width / image.Height;
                            }
                            slide.Shapes.AddPicture(imagePath,
                                MsoTriState.msoFalse,
                                MsoTriState.msoCTrue,
                                shape.Left,
                                shape.Top,
                                imgWidth,
                                imgHeight);
                            imgFileInfo.Delete();
                        }
                        break;
                    }
                case MsoShapeType.msoTable:
                    {
                        for (int i = 1; i <= shape.Table.Columns.Count; i++)
                        {
                            for (int j = 1; j <= shape.Table.Rows.Count; j++)
                            {
                                ReplaceTag(slide, shape.Table.Cell(j, i).Shape, searchTag, replaceValue, tempFilesDirectoryFullPath);
                            }
                        }
                        break;
                    }
                default:
                    break;
            }
        }

        //private static List<PowerPoint.Range?> getTagRanges(PowerPoint.Application? powerPointApp, string tag)
        //{
        //    object searchTagObj = tag;
        //    object missing = Type.Missing;
        //    var result = new List<PowerPoint.Range?>();
        //    var startsFrom = 0;
        //    while (true)
        //    {
        //        PowerPoint.Range? wordRange = powerPointApp?.ActiveDocument.Content;
        //        if (wordRange == null)
        //            return result;
        //        wordRange.Start = startsFrom;
        //        wordRange.Find.Execute(
        //        ref searchTagObj, ref missing, ref missing, ref missing, ref missing,
        //        ref missing, ref missing, ref missing, ref missing, ref missing,
        //        ref missing, ref missing, ref missing, ref missing, ref missing);
        //        if (!wordRange.Find.Found)
        //            break;
        //        startsFrom = wordRange.Start + 1;
        //        result.Add(wordRange);
        //    }
        //    return result;
        //}

        //private static void replace(List<PowerPoint.Range?> ranges, string replacementValue)
        //{
        //    foreach (var range in ranges)
        //        if (range != null)
        //            range.Text = replacementValue;
        //}

        //private static void replace(PowerPoint.Application? powerPointApp, string tempFilesDirectoryFullPath, List<Word.Range?> ranges, Image image)
        //{
        //    var imagePath = FileNameBuilder.constructUniqueFileName("tempImage", tempFilesDirectoryFullPath, image.RawFormat.ToString().ToLower());
        //    image?.Save(imagePath, image.RawFormat);
        //    var imgFileInfo = new FileInfo(imagePath);
        //    if (!(imgFileInfo).Exists)
        //        throw new Exception("Creating image temp file error");
        //    foreach (var range in ranges)
        //    {
        //        if (range != null)
        //        {
        //            range.Text = String.Empty;
        //            powerPointApp?.Selection.InlineShapes.AddPicture(imagePath, Range: range);
        //        }
        //    }
        //    imgFileInfo.Delete();
        //}
    }
}