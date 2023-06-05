using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Impactly_0._1.@class
{
    public class Todo : INotifyPropertyChanged
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
