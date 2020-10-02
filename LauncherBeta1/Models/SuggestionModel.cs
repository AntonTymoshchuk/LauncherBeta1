using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace LauncherBeta1.Models
{
    public class SuggestionModel : INotifyPropertyChanged
    {
        private int height;
        private int imageColumnWidth;
        private BitmapSource image;
        private string requestedText;
        private string remainedText;
        private string completeText;
        
        public event PropertyChangedEventHandler PropertyChanged;

        public int Height
        {
            get { return height; }
        }

        public int ImageColumnWidth
        {
            get { return imageColumnWidth; }
        }

        public BitmapSource Image
        {
            get { return image; }
        }

        public string CompleteText
        {
            get { return completeText; }
            set { completeText = value; }
        }

        public string RequestedText
        {
            get { return requestedText; }
            set { requestedText = value; }
        }

        public string RemainedText
        {
            get { return remainedText; }
            set { remainedText = value; }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SuggestionModel AsSuggestionType(string suggestionType)
        {
            height = 36;
            image = null;
            imageColumnWidth = 0;
            completeText = suggestionType;
            return this;
        }

        public SuggestionModel AsSuggestionItem(BitmapSource image, string requestedText, string remainedText)
        {
            height = 30;
            imageColumnWidth = 30;
            this.image = image;
            this.requestedText = requestedText;
            this.remainedText = remainedText;
            return this;
        }
    }
}
