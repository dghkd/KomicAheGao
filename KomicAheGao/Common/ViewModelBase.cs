﻿using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace KomicAheGao
{
    /// <summary>
    /// The view model base used for MVVM design pattern.
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        #region Private Properties

        private readonly Dictionary<String, PropertyChangedEventArgs> _cacheEventArgs;

        #endregion

        #region Constructor

        protected ViewModelBase()
        {
            _cacheEventArgs = new Dictionary<String, PropertyChangedEventArgs>();
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(String propertyName)
        {
            PropertyChangedEventArgs args;
            if (!_cacheEventArgs.TryGetValue(propertyName, out args))
            {
                args = new PropertyChangedEventArgs(propertyName);
                _cacheEventArgs.Add(propertyName, args);
            }

            OnPropertyChanged(args);
        }

        protected void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, args);
        }

        #endregion
    }
}
