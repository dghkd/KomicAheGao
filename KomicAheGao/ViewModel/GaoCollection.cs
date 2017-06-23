using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace KomicAheGao.ViewModel
{
    public class GaoCollection : ObservableCollection<GaoVM>
    {
        #region Private Member

        #endregion

        #region Constructor
        public GaoCollection()
        {

        }

        public GaoCollection(List<GaoVM> list)
            : base(list)
        {

        }
        #endregion

        #region Public Member

        #endregion

        #region Private Method

        #endregion
    }
}
