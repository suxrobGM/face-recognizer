using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FaceRecognizer.App.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        #region Methods

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) 
                return false;

            storage = value;
            RaisePropertyChanged(propertyName);

            return true;
        }

        #endregion
        

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}