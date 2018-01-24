
using System.ComponentModel;

namespace Safire.Library.TableModels
{
    public class ViewModelBase
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
          
    }
}
