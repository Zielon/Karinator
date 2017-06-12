using System.ComponentModel;

namespace Karinator.API
{
    public class Node : INotifyPropertyChanged
    {
        private int _progress;

        public string FileName { get; set; }
        public string Path { get; set; }

        public int Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Progress"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
