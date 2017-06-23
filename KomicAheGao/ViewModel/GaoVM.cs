using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace KomicAheGao.ViewModel
{
    public class GaoVM : ViewModelBase
    {
        #region Static Fields and Constants
        public const String ATTR_DESC_DATATABLEPROP = "DataTableProp";

        #endregion

        #region Private Member
        private String _name;
        private String _text;
        private String _toolTip;
        #endregion

        #region Constructor
        public GaoVM()
        {

        }
        #endregion

        #region Public Member
        [Description(ATTR_DESC_DATATABLEPROP)]
        public String Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("Name"); }
        }

        [Description(ATTR_DESC_DATATABLEPROP)]
        public String Text
        {
            get { return _text; }
            set
            {
                _text = value;
                _toolTip = _text;
                OnPropertyChanged("Text");
                OnPropertyChanged("ToolTipString");
            }
        }

        public String ToolTipString
        {
            get { return _toolTip; }
            set { _toolTip = value; OnPropertyChanged("ToolTipString"); }
        }

        #endregion

        #region Command
        public const String CmdKey_SendText = "CmdKey_SendText";
        private CommandBase _cmdSendText;
        public CommandBase CmdSendText
        {
            get
            {
                return _cmdSendText ?? (_cmdSendText = new CommandBase(x => ExecuteCommand(CmdKey_SendText)));
            }
        }


        public Func<String, GaoVM, bool> CommandAction;

        private void ExecuteCommand(String cmd)
        {
            if (this.CommandAction != null)
            {
                this.CommandAction(cmd, this);
            }
        }
        #endregion

        #region Public Method

        #endregion

        #region Private Method

        #endregion

    }
}
