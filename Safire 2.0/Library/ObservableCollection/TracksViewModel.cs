using System.Collections.ObjectModel;
using Safire.Library.TableModels;
using Safire.Library.ViewModels;

namespace Safire.Library.ObservableCollection
{
    public class ObservableTracksViewModel : ViewModelBase
    {
        private ObservableCollection<TrackViewModel> tracks;

        public ObservableCollection<TrackViewModel> Tracks
        {
            get { return tracks; }

            set
            {
                tracks = value;
                RaisePropertyChanged("Tracks");
            }
        }

        public void NotifyChanges()
        {
            RaisePropertyChanged("Tracks");
        }

        
    }
}