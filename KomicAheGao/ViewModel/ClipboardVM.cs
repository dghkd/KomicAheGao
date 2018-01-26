using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace KomicAheGao.ViewModel
{
    public class ClipboardVM : ViewModelBase
    {
        public const String TYPE_RICHTEXT = "Rich Text Format";
        public const String TYPE_OEMTEXT = "OEMText";
        public const String TYPE_TEXT = "Text";
        public const String TYPE_UNICODETEXT = "UnicodeText";
        public const String TYPE_SYSTEM_STRING = "System.String";
        public const String TYPE_FILE_DROP = "FileDrop";
        public const String TYPE_FILE_NAME = "FileName";
        public const String TYPE_FILE_NAME_W = "FileNameW";
        public const String TYPE_CSV = "Csv";
        public const String TYPE_HTML = "HTML Format";
        public const String TYPE_LOCALE = "Locale";
        public const String TYPE_DRAWING_BITMAP = "System.Drawing.Bitmap";
        public const String TYPE_BITMAP = "Bitmap";
        public const String TYPE_DEV_INDP_BITMAP = "DeviceIndependentBitmap";
        public const String TYPE_FORMAT17 = "Format17";

        //TODO:Use System.Windows.Forms.DataFormats.
        public enum ClipboardDataType
        {
            RichTextFormat,
            //OEMText,
            Text,
            //UnicodeText,
            //SystemString,
            File,
            //FileDrop,
            //FileName,
            //FileNameW,
            //Csv,
            HTMLFormat,
            Locale,
            //SystemDrawingBitmap,
            Bitmap,
            //DeviceIndependentBitmap,
            //Format17,

            Unknown,
        }

        #region Private Member
        private String _name;
        private String _toolTip;
        private ClipboardDataType _type;
        private String _txtContent;
        private int _dataObjectCount;
        private ImageSource _imgSource;

        private Dictionary<String, Object> _dict;
        private List<String> _fileDropList;
        #endregion


        #region Constructor

        public ClipboardVM(IDataObject dataObject)
        {
            _dict = new Dictionary<String, Object>();
            _fileDropList = new List<String>();

            foreach (string format in dataObject.GetFormats())
            {
                if (format == DataFormats.EnhancedMetafile)
                {
                    continue;
                }

                _dict.Add(format, dataObject.GetData(format));
                Debug.WriteLine(String.Format("{0}, {1}", format, dataObject.GetData(format)));
            }

            _dataObjectCount = _dict.Count;
            this.Type = ClipboardDataType.Unknown;

            if (_dict.ContainsKey(ClipboardVM.TYPE_RICHTEXT))
            {
                this.Type = ClipboardVM.GetDataType(ClipboardVM.TYPE_RICHTEXT);
                this.TxtContent = _dict[ClipboardVM.TYPE_RICHTEXT].ToString();
            }

            if (_dict.ContainsKey(ClipboardVM.TYPE_OEMTEXT)
                || _dict.ContainsKey(ClipboardVM.TYPE_TEXT)
                || _dict.ContainsKey(ClipboardVM.TYPE_UNICODETEXT)
                || _dict.ContainsKey(ClipboardVM.TYPE_SYSTEM_STRING)
                || _dict.ContainsKey(ClipboardVM.TYPE_CSV))
            {
                this.Type = ClipboardVM.GetDataType(ClipboardVM.TYPE_SYSTEM_STRING);
                this.TxtContent = _dict[ClipboardVM.TYPE_SYSTEM_STRING].ToString();
            }

            if (_dict.ContainsKey(ClipboardVM.TYPE_FILE_DROP)
                || _dict.ContainsKey(ClipboardVM.TYPE_FILE_NAME)
                || _dict.ContainsKey(ClipboardVM.TYPE_FILE_NAME_W))
            {
                String[] paths = _dict[ClipboardVM.TYPE_FILE_NAME_W] as String[];
                if (paths != null)
                {
                    this.Name = ClipboardVM.TYPE_FILE_NAME;
                    this.Type = ClipboardVM.GetDataType(ClipboardVM.TYPE_FILE_NAME_W);
                    this.TxtContent = paths.ElementAtOrDefault(0);
                    _fileDropList.Clear();
                    foreach (String f in paths)
                    {
                        _fileDropList.Add(f);
                    }
                }
            }
            
            if (_dict.ContainsKey(ClipboardVM.TYPE_DRAWING_BITMAP)
                || _dict.ContainsKey(ClipboardVM.TYPE_BITMAP)
                || _dict.ContainsKey(ClipboardVM.TYPE_DEV_INDP_BITMAP)
                || _dict.ContainsKey(ClipboardVM.TYPE_FORMAT17))
            {
                this.Name = ClipboardVM.TYPE_BITMAP;
                this.Type = ClipboardVM.GetDataType(ClipboardVM.TYPE_BITMAP);
                
                if (dataObject.GetDataPresent(DataFormats.Bitmap))
                {
                    Bitmap bitmap = (Bitmap)dataObject.GetData(DataFormats.Bitmap);
                    IntPtr hBitmap = bitmap.GetHbitmap();
                    try
                    {
                        this.ImgSource= System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero,System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    }
                    finally
                    {
                        Win32Helper.DeleteObject(hBitmap);
                    }
                }
            }

        }

        #endregion


        #region Public Member
        public String Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("Name"); }
        }

        public String ToolTipString
        {
            get { return _toolTip; }
            set { _toolTip = value; OnPropertyChanged("ToolTipString"); }
        }
        
        public ClipboardDataType Type
        {
            get { return _type; }
            set { _type = value; OnPropertyChanged("Type"); }
        }


        public String TxtContent
        {
            get { return _txtContent; }
            set { _txtContent = value; OnPropertyChanged("TxtContent"); }
        }

        public ImageSource ImgSource
        {
            get { return _imgSource; }
            set { _imgSource = value; OnPropertyChanged("ImgSource"); }
        }

        public DataObject GetDataObject()
        {
            DataObject ret = new System.Windows.Forms.DataObject();
            foreach (String format in _dict.Keys)
            {
                ret.SetData(format, _dict[format]);
            }

            return ret;
        }


        public int DataObjectCount
        {
            get { return _dataObjectCount; }
            set { _dataObjectCount = value; OnPropertyChanged("DataObjectCount"); }
        }

        #endregion


        #region Command

        public const string CmdKey_SendData = "CmdKey_SendData";
        private CommandBase _cmdSendData;
        public CommandBase CmdSendData
        {
            get
            {
                return _cmdSendData ?? (_cmdSendData = new CommandBase(x => ExecuteCommand(CmdKey_SendData)));
            }
        }
        

        public Func<String, ClipboardVM, bool> CommandAction;

        private void ExecuteCommand(String cmd)
        {
            if (this.CommandAction != null)
            {
                this.CommandAction(cmd, this);
            }
            System.Threading.Thread receive = new System.Threading.Thread(on_thread);
            receive.Abort();
            
            
        }

        private void on_thread()
        {
            
        }
        #endregion


        #region Public Method

        public static ClipboardDataType GetDataType(string dataFormat)
        {
            switch (dataFormat)
            {
                case ClipboardVM.TYPE_RICHTEXT:
                    return ClipboardDataType.RichTextFormat;

                case ClipboardVM.TYPE_OEMTEXT:
                case ClipboardVM.TYPE_TEXT:
                case ClipboardVM.TYPE_UNICODETEXT:
                case ClipboardVM.TYPE_SYSTEM_STRING:
                case ClipboardVM.TYPE_CSV:
                    return ClipboardDataType.Text;

                case ClipboardVM.TYPE_FILE_DROP:
                case ClipboardVM.TYPE_FILE_NAME:
                case ClipboardVM.TYPE_FILE_NAME_W:
                    return ClipboardDataType.File;

                case ClipboardVM.TYPE_HTML:
                    return ClipboardDataType.HTMLFormat;

                case ClipboardVM.TYPE_LOCALE:
                    return ClipboardDataType.Locale;

                case ClipboardVM.TYPE_DRAWING_BITMAP:
                case ClipboardVM.TYPE_BITMAP:
                case ClipboardVM.TYPE_DEV_INDP_BITMAP:
                case ClipboardVM.TYPE_FORMAT17:
                    return ClipboardDataType.Bitmap;
            }

            return ClipboardDataType.Unknown;
        }

        public static byte[] ToByteArray(object source)
        {
            var Formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            using (var stream = new System.IO.MemoryStream())
            {
                Formatter.Serialize(stream, source);
                return stream.ToArray();
            }
        }

        public bool Compare(ClipboardVM vm)
        {
            if (vm == null)
            {
                return false;
            }

            if (this.Type != vm.Type)
            {
                return false;
            }

            if (this.TxtContent != vm.TxtContent)
            {
                return false;
            }

            if (vm.Type == ClipboardDataType.File)
            {
                return _fileDropList.SequenceEqual(vm._fileDropList);
            }

            if (vm.Type == ClipboardDataType.Bitmap)
            {
                byte[] x = ClipboardVM.ToByteArray(vm.ImgSource);
                byte[] y = ClipboardVM.ToByteArray(this.ImgSource);
                return x.SequenceEqual(y);
            }

            return true;
        }
        #endregion
    }
}
