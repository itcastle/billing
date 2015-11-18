using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GestionCommerciale.DomainModel.Entities;

namespace GestionCommerciale.Helpers
{
    public static class Helper
    {
        public static byte[] Compress(byte[] data)
        {
            if (data == null) return null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream gzStream = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    gzStream.Write(data, 0, data.Length);
                }
                return ms.ToArray();
            }
        }

        public static byte[] Decompress(byte[] data)
        {
            if (data == null) return null;
            using (GZipStream gzStream = new GZipStream(new MemoryStream(data), CompressionMode.Decompress, true))
            {
                const int bufferSize = 4096;
                int bytesRead = 0;

                byte[] buffer = new byte[bufferSize];

                using (MemoryStream ms = new MemoryStream())
                {
                    while ((bytesRead = gzStream.Read(buffer, 0, bufferSize)) > 0)
                    {
                        ms.Write(buffer, 0, bytesRead);
                    }
                    return ms.ToArray();
                }
            }
        }

        public static T DeepClone<T>(this T obj)
        {
            using (var ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, obj);
                ms.Position = 0;
                return (T)bf.Deserialize(ms);
            }
        }
        public static void ShowUserControlInMainWindow(UserControl uc)
        {
            Window mainWindow = Application.Current.MainWindow;
            var rootGrid = FindVisualChildByName<Grid>(mainWindow, "RootGrid_Grid");
            if (rootGrid != null)
            {
                rootGrid.Children.Add(uc);
            }
            else
            {
                throw new ResourceReferenceKeyNotFoundException();
            }
        }

        public static DependencyObject GetTopParent(DependencyObject current)
        {
            while (VisualTreeHelper.GetParent(current) != null)
            {
                current = VisualTreeHelper.GetParent(current);
            }
            return current;
        }
        public static T FindVisualChildByName<T>(DependencyObject parent, string name) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                var controlName = child.GetValue(FrameworkElement.NameProperty) as string;
                if (controlName == name)
                {
                    return child as T;
                }
                T result = FindVisualChildByName<T>(child, name);
                if (result != null)
                    return result;
            }
            return null;
        }

        public static int GetAgeFromdateNaissance(DateTime birthDate)
        {
            return DateTime.Now.Year - birthDate.Year;
        }

        public static BitmapImage ByteArrayToBitmap(object bytesArray)
        {
            if (bytesArray == null || bytesArray.GetType() != typeof(Byte[]))
                return null;

            var binaryData = (byte[])bytesArray;

            var bmp = new BitmapImage();

            using (var stream = new MemoryStream(binaryData))
            {
                bmp.BeginInit();
                bmp.StreamSource = stream;
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.EndInit();
            }

            if (bmp.CanFreeze)
                bmp.Freeze();

            return bmp;
        }

        public static byte[] ImageToByteArray(object bitmap)
        {
            if (bitmap == null)
                return null;

            var bmp = (BitmapSource)bitmap;

            int stride = bmp.PixelWidth * ((bmp.Format.BitsPerPixel + 7) / 8);

            var binaryData = new byte[bmp.PixelHeight * stride];

            bmp.CopyPixels(binaryData, stride, 0);

            return binaryData;
        }
        
        public static string GetDocsPath(string subDirectories)
        {
            //server file system for docs 
            string serverPath = "";//Properties.Settings.Default.ServerDocsPath;
            string[] drs = subDirectories.Split('|');
            foreach (string directory in drs)
            {
                serverPath += @"\" + directory;
            }
            serverPath += @"\";


            if (Directory.Exists(serverPath))
            {
                return serverPath;

            }

            //local file system for docs 
            string localPath = "";//Properties.Settings.Default.LocalDocsPath;
            drs = subDirectories.Split('|');
            foreach (string directory in drs)
            {
                localPath += @"/" + directory;
            }
            localPath += @"/";


            if (Directory.Exists(localPath))
            {
                return localPath;
            }

            //this directory dont exist
            return string.Empty;


        }
        public static List<string> GetModelsNamesList(List<DocModel> models)
        {
            var fileNames = new List<string>();
            foreach (var model in models)
            {
                string name = model.Name;
                fileNames.Add(name);
            }
            return fileNames;
        }
        public static string GetDocFile(string subDirectories, string fileName)
        {
            //server file system for docs 
            string serverPath = "";//Properties.Settings.Default.ServerDocsPath;
            string[] drs = subDirectories.Split('|');
            foreach (string directory in drs)
            {
                serverPath += @"\" + directory;
            }
            serverPath += @"\" + fileName;


            if (File.Exists(serverPath))
            {
                return serverPath;

            }

            //local file system for docs 
            string localPath = "";//Properties.Settings.Default.LocalDocsPath;
            drs = subDirectories.Split('|');
            foreach (string directory in drs)
            {
                localPath += @"/" + directory;
            }
            localPath += @"/" + fileName;


            if (File.Exists(localPath))
            {
                return localPath;
            }

            //this file dont exist
            return string.Empty;


        }

        public static List<FileInfo> GetModels(string subDirectories)
        {
            //server file system for docs 
            string serverPath = "";//Properties.Settings.Default.ServerDocsPath;
            string[] drs = subDirectories.Split('|');
            foreach (string directory in drs)
            {
                serverPath += @"\" + directory;
            }
            serverPath += @"\";


            if (Directory.Exists(serverPath))
            {
                string[] files = Directory.GetFiles(serverPath, "*.doc");
                var filesInfos = new List<FileInfo>();
                foreach (string f in files)
                {
                    filesInfos.Add(new FileInfo(f));
                }
                return filesInfos;
            }

            //local file system for docs 
            string localPath = "";// Properties.Settings.Default.LocalDocsPath;
            drs = subDirectories.Split('|');
            foreach (string directory in drs)
            {
                localPath += @"/" + directory;
            }
            localPath += @"/";


            if (Directory.Exists(localPath))
            {
                string[] files = Directory.GetFiles(localPath, "*.doc");
                var filesInfos = new List<FileInfo>();
                foreach (string f in files)
                {
                    filesInfos.Add(new FileInfo(f));
                }
                return filesInfos;
            }

            //this directory dont exist
            return null;


        }

        

        public static string GetColmnValueFromFilter(string filter)
        {
            string[] tab = filter.Split(',');

            if (tab.Length > 1)
            {
                string target = string.Empty;
                for (int i = 1; i < tab.Length; i++)
                {
                    target += ',';
                    target += tab[i];

                }


                target = target.Remove(0, 3);
                target = target.Remove(target.Length - 2, 2);
                target = target.Replace("''", "'");
                return target;
            }
            return string.Empty;
        }


        /*public static MediaInfo GetPlusMediaInfo ( DateTime groupeDate )
        {
            var info = new MediaInfo
                           {
                                     MediaId = -1,
                                     DateInsertion = groupeDate,
                                     Caption = "",
                                     MediaType = "C",
                                     ViewSize = new Size(150, 100),
                                     IsItemBtnsEventsReady = false
                                 };

            string file = GetDocFile("Medias", "plus.png");
            if (file != string.Empty)
            {
                var bmImage = new BitmapImage();
                bmImage.BeginInit();
                bmImage.UriSource = new Uri(file, UriKind.Absolute);
                bmImage.EndInit();

                info.Source = bmImage;
                info.FilePath = file;
                info.FileExt = "png";
            }

            return info;
        }*/

        public static bool IsStringContainAnyStringArrayElement(string str, List<char> tab)
        {
            bool result = false;

            foreach (char c in tab)
            {
                if (str.Contains(c.ToString()))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        /*public static string GetGalleryItemXaml(MediaInfo info)
        {
            string xaml = @"<GalleryItem xmlns=""http://schemas.devexpress.com/winfx/2008/xaml/bars"">";

            xaml += @"<MediaInfo
                    Caption=""" + info.Caption + @""" 
                    SizeInfo=""" + info.SizeInfo + @""" 
                    Source=""" + info.Source.ToString() + @"""
                    ViewSize=""150,100""
                    MediaType=""" + info.MediaType + @""" 
                    xmlns=""clr-namespace:UserInterface.Views.DoctorUCs;assembly=UserInterface"" xmlns:assembly=""http://schemas.microsoft.com/winfx/2006/xaml"" />";

            xaml += "</GalleryItem>";
            return xaml;

        }*/

        public static string ConvertChifreToLettre(decimal chiffre)
        {
            bool dix = false;
            string lettre = "";
            //strcpy(lettre, "");

            int reste = (int)chiffre / 1;

            for (int i = 1000000000; i >= 1; i /= 1000)
            {
                int y = reste / i;
                if (y != 0)
                {
                    int centaine = y / 100;
                    int dizaine = (y - centaine * 100) / 10;
                    int unite = y - (centaine * 100) - (dizaine * 10);
                    switch (centaine)
                    {
                        case 0:
                            break;
                        case 1:
                            lettre += "cent ";
                            break;
                        case 2:
                            if ((dizaine == 0) && (unite == 0)) lettre += "deux cents ";
                            else lettre += "deux cent ";
                            break;
                        case 3:
                            if ((dizaine == 0) && (unite == 0)) lettre += "trois cents ";
                            else lettre += "trois cent ";
                            break;
                        case 4:
                            if ((dizaine == 0) && (unite == 0)) lettre += "quatre cents ";
                            else lettre += "quatre cent ";
                            break;
                        case 5:
                            if ((dizaine == 0) && (unite == 0)) lettre += "cinq cents ";
                            else lettre += "cinq cent ";
                            break;
                        case 6:
                            if ((dizaine == 0) && (unite == 0)) lettre += "six cents ";
                            else lettre += "six cent ";
                            break;
                        case 7:
                            if ((dizaine == 0) && (unite == 0)) lettre += "sept cents ";
                            else lettre += "sept cent ";
                            break;
                        case 8:
                            if ((dizaine == 0) && (unite == 0)) lettre += "huit cents ";
                            else lettre += "huit cent ";
                            break;
                        case 9:
                            if ((dizaine == 0) && (unite == 0)) lettre += "neuf cents ";
                            else lettre += "neuf cent "; break;
                    }// endSwitch(centaine)

                    switch (dizaine)
                    {
                        case 0:
                            break;
                        case 1:
                            dix = true;
                            break;
                        case 2:
                            lettre += "vingt ";
                            break;
                        case 3:
                            lettre += "trente ";
                            break;
                        case 4:
                            lettre += "quarante ";
                            break;
                        case 5:
                            lettre += "cinquante ";
                            break;
                        case 6:
                            lettre += "soixante ";
                            break;
                        case 7:
                            dix = true;
                            lettre += "soixante ";
                            break;
                        case 8:
                            lettre += "quatre-vingt ";
                            break;
                        case 9:
                            dix = true;
                            lettre += "quatre-vingt "; break;
                    } // endSwitch(dizaine)

                    switch (unite)
                    {
                        case 0:
                            if (dix) lettre += "dix ";
                            break;
                        case 1:
                            if (dix) lettre += "onze ";
                            else lettre += "un ";
                            break;
                        case 2:
                            if (dix) lettre += "douze ";
                            else lettre += "deux ";
                            break;
                        case 3:
                            if (dix) lettre += "treize ";
                            else lettre += "trois ";
                            break;
                        case 4:
                            if (dix) lettre += "quatorze ";
                            else lettre += "quatre ";
                            break;
                        case 5:
                            if (dix) lettre += "quinze ";
                            else lettre += "cinq ";
                            break;
                        case 6:
                            if (dix) lettre += "seize ";
                            else lettre += "six ";
                            break;
                        case 7:
                            if (dix) lettre += "dix-sept ";
                            else lettre += "sept ";
                            break;
                        case 8:
                            if (dix) lettre += "dix-huit ";
                            else lettre += "huit ";
                            break;
                        case 9:
                            if (dix) lettre += "dix-neuf ";
                            else lettre += "neuf "; break;
                    } // endSwitch(unite)

                    switch (i)
                    {
                        case 1000000000:
                            if (y > 1) lettre += "milliards ";
                            else lettre += "milliard ";
                            break;
                        case 1000000:
                            if (y > 1) lettre += "millions ";
                            else lettre += "million ";
                            break;
                        case 1000:
                            lettre += "mille "; break;
                    }
                } // end if(y!=0)
                reste -= y * i;
                dix = false;
            } // end for
            if (lettre.Length == 0) lettre += "zero";

            return lettre;
        }

        public static string GetFileTypeFromExt(string ext)
        {
            var imageExts = new[] { "jpg", "jpeg", "png", "bmp" };
            var videoExts = new[] { "wmv", "avi", "flv", "mpeg" };

            if (imageExts.Contains(ext)) return "P";
            if (videoExts.Contains(ext)) return "V";
            return string.Empty;
        }

    

        public static MessageBoxResult ShowWarningMessageBox(string title, string message)
        {
            return MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static MessageBoxResult ShowErrorMessageBox(string title, string message)
        {
            return MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static MessageBoxResult ShowQuestionMessageBox(string title, string message)
        {
            return MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
        }

        public static String[] GetFilesFrom(String searchFolder, String[] filters, bool isRecursive)
        {
            List<String> filesFound = new List<String>();
            var searchOption = isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            foreach (var filter in filters)
            {
                filesFound.AddRange(Directory.GetFiles(searchFolder, String.Format("*.{0}", filter), searchOption));
            }
           
            return filesFound.ToArray();
        }
        public static FileInfo[] GetFilesinfosFromFilesnames(String[] FilesNames)
        {
            List<FileInfo> files = new List<FileInfo>();
            foreach (var filename in FilesNames)
            {
                files.Add(new FileInfo(filename));
            }
            return files.ToArray();
        }



    }

    public static class StreamExtensions
    {
        public static byte[] ToByteArray(this Stream stream)
        {
            stream.Position = 0;
            byte[] buffer = new byte[stream.Length];
            for (int totalBytesCopied = 0; totalBytesCopied < stream.Length; )
                totalBytesCopied += stream.Read(buffer, totalBytesCopied, Convert.ToInt32(stream.Length) - totalBytesCopied);
            return buffer;
        }
    }



}