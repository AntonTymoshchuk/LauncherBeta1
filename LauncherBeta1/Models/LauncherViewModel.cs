using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LauncherBeta1.Models
{
    public class LauncherViewModel : INotifyPropertyChanged
    {
        private SolidColorBrush requestAreaColor;
        private SolidColorBrush separationLineColor;

        private ObservableCollection<SuggestionModel> suggestionModels;

        public event PropertyChangedEventHandler PropertyChanged;

        public SolidColorBrush RequestAreaColor
        {
            get { return requestAreaColor; }
        }

        public SolidColorBrush SeparationLineColor
        {
            get { return separationLineColor; }
        }

        public ObservableCollection<SuggestionModel> SuggestionModels
        {
            get { return suggestionModels; }
            set
            {
                suggestionModels = value;
                OnPropertyChanged("SuggestionModels");
            }
        }

        public LauncherViewModel()
        {
            requestAreaColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(242, 244, 248));
            separationLineColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(229, 229, 229));

            suggestionModels = new ObservableCollection<SuggestionModel>();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void FindFiles(string request)
        {
            List<FileInfo> suggestedFileInfos = new List<FileInfo>();

            DirectoryInfo[] involvedDirectoryInfos = new DirectoryInfo[8];
            involvedDirectoryInfos[0] = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\3D Objects");
            involvedDirectoryInfos[1] = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));
            involvedDirectoryInfos[2] = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            involvedDirectoryInfos[3] = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads");
            involvedDirectoryInfos[4] = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
            involvedDirectoryInfos[5] = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));
            involvedDirectoryInfos[6] = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Desktop");
            involvedDirectoryInfos[7] = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\OneDrive");


            for (int i = 0; i < involvedDirectoryInfos.Length; i++)
            {
                FileInfo[] contentFileInfos = involvedDirectoryInfos[i].GetFiles();
                for (int j = 0; j < contentFileInfos.Length; j++)
                {
                    if (contentFileInfos[j].Name.ToLower().StartsWith(request))
                        suggestedFileInfos.Add(contentFileInfos[j]);
                }

                DirectoryInfo[] contentDirectoryInfos = involvedDirectoryInfos[i].GetDirectories();
                for (int j = 0; j < contentDirectoryInfos.Length; j++)
                    FindFilesInContentDirectories(request, contentDirectoryInfos[j], suggestedFileInfos);
            }

            if (suggestedFileInfos.Count > 0)
            {
                suggestionModels.Add(new SuggestionModel().AsSuggestionType("Files"));

                for (int i = 0; i < suggestedFileInfos.Count; i++)
                {
                    BitmapSource image = GetFileIcon(suggestedFileInfos[i]);

                    string fileName = suggestedFileInfos[i].Name.TrimEnd(suggestedFileInfos[i].Extension.ToCharArray());
                    string requestedText = fileName.Remove(request.Length, fileName.Length - request.Length);
                    string remainedText = fileName.Remove(0, request.Length);

                    SuggestionModel suggestionModel = new SuggestionModel().AsSuggestionItem(image, requestedText, remainedText);
                    suggestionModels.Add(suggestionModel);
                }
            }
        }

        private void FindFilesInContentDirectories(string request, DirectoryInfo parentDirectoryInfo, List<FileInfo> suggestedFileInfos)
        {
            try
            {
                FileInfo[] contentFileInfos = parentDirectoryInfo.GetFiles();
                for (int i = 0; i < contentFileInfos.Length; i++)
                {
                    if (contentFileInfos[i].Name.ToLower().StartsWith(request))
                        suggestedFileInfos.Add(contentFileInfos[i]);
                }

                DirectoryInfo[] contentDirectoryInfos = parentDirectoryInfo.GetDirectories();
                for (int i = 0; i < contentDirectoryInfos.Length; i++)
                    FindFilesInContentDirectories(request, contentDirectoryInfos[i], suggestedFileInfos);
            }
            catch { }
        }

        private BitmapSource GetFileIcon(FileInfo fileInfo)
        {
            BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap
            (
                Icon.ExtractAssociatedIcon(fileInfo.FullName).ToBitmap().GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()
            );
            return bitmapSource;
        }

        private void FindDirectories(string request)
        {
            List<DirectoryInfo> suggestedDirectoryInfos = new List<DirectoryInfo>();

            DirectoryInfo[] involvedDirectoryInfos = new DirectoryInfo[7];
            involvedDirectoryInfos[0] = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            involvedDirectoryInfos[1] = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));
            involvedDirectoryInfos[2] = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            involvedDirectoryInfos[3] = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads");
            involvedDirectoryInfos[4] = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
            involvedDirectoryInfos[5] = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));
            involvedDirectoryInfos[6] = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Desktop");

            for (int i = 0; i < involvedDirectoryInfos.Length; i++)
            {
                DirectoryInfo[] contentDirectoryInfos = involvedDirectoryInfos[i].GetDirectories();
                for (int j = 0; j < contentDirectoryInfos.Length; j++)
                {
                    if (contentDirectoryInfos[j].Name.ToLower().StartsWith(request))
                        suggestedDirectoryInfos.Add(contentDirectoryInfos[j]);
                    FindDirectoriesInContentDirectories(request, contentDirectoryInfos[j], suggestedDirectoryInfos);
                }
            }

            if (suggestedDirectoryInfos.Count > 0)
            {
                suggestionModels.Add(new SuggestionModel().AsSuggestionType("Directories"));

                for (int i = 0; i < suggestedDirectoryInfos.Count; i++)
                {
                    BitmapSource image = GetDirectoryIcon(suggestedDirectoryInfos[i]);

                    string directoryName = suggestedDirectoryInfos[i].Name;
                    string requestedText = directoryName.Remove(request.Length, directoryName.Length - request.Length);
                    string remainedText = directoryName.Remove(0, request.Length);

                    SuggestionModel suggestionModel = new SuggestionModel().AsSuggestionItem(image, requestedText, remainedText);
                    suggestionModels.Add(suggestionModel);
                }
            }
        }

        private void FindDirectoriesInContentDirectories(string request, DirectoryInfo parentDirectoryInfo, List<DirectoryInfo> suggestedDirectoryInfos)
        {
            try
            {
                DirectoryInfo[] contentDirectoryInfos = parentDirectoryInfo.GetDirectories();
                for (int i = 0; i < contentDirectoryInfos.Length; i++)
                {
                    if (contentDirectoryInfos[i].Name.ToLower().StartsWith(request))
                        suggestedDirectoryInfos.Add(contentDirectoryInfos[i]);
                    FindDirectoriesInContentDirectories(request, contentDirectoryInfos[i], suggestedDirectoryInfos);
                }
            }
            catch { }
        }

        private BitmapSource GetDirectoryIcon(DirectoryInfo directoryInfo)
        {
            try
            {
                ShellFolder shellFolder = ShellObject.FromParsingName(directoryInfo.FullName) as ShellFolder;
                return shellFolder.Thumbnail.BitmapSource;
            }
            catch { return null; }
        }

        public void ProcessRequest(string request)
        {
            suggestionModels.Clear();
            request = request.ToLower();

            FindDirectories(request);
            FindFiles(request);
        }
    }
}
