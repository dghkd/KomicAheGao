using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Windows;
using System.Collections.ObjectModel;

using KomicAheGao.ViewModel;

namespace KomicAheGao
{
    public class GaoData : IDisposable
    {
        public const String TABLE_NAME = "GaoDictionary";

        #region Private Member
        private static readonly Lazy<GaoData> _lazyInstance = new Lazy<GaoData>(() => new GaoData(), true);

        private DataTable _gaoTable;
        private List<GaoVM> _gaoList;

        #endregion


        #region Constructor
        public GaoData()
        {
            _gaoList = new List<GaoVM>();
            _gaoTable = new DataTable(TABLE_NAME);

            CreateTableColumn();
        }
        #endregion


        #region Public Member
        public static GaoData Data { get { return _lazyInstance.Value; } private set { } }

        public List<GaoVM> GaoList
        {
            get
            {
                return _gaoList;
            }
            set
            {
                _gaoList = value;
            }
        }

        public Func<String, GaoVM, bool> CommandAction;
        #endregion


        #region Public Method

        public void LoadData()
        {
            String dataPath = AppDomain.CurrentDomain.BaseDirectory + "\\" + TABLE_NAME;
            if (File.Exists(dataPath))
            {
                _gaoTable.ReadXml(dataPath);
                foreach (DataRow row in _gaoTable.Rows)
                {
                    GaoVM vm = new GaoVM();
                    vm.CommandAction = On_Command_Execute;
                    foreach (PropertyInfo prop in GetDataTablePropList(vm))
                    {
                        prop.SetValue(vm, row[prop.Name]);
                    }
                    _gaoList.Add(vm);
                    
                }
            }
        }

        public void SaveData()
        {
            if (_gaoTable != null
               && _gaoTable.TableName != ""
               && _gaoTable.TableName != null)
            {
                try
                {
                    _gaoTable.Clear();
                    foreach (GaoVM vm in _gaoList)
                    {
                        AddToTable(vm);
                    }
                    _gaoTable.WriteXml(TABLE_NAME, XmlWriteMode.WriteSchema);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        public GaoCollection GetCollection()
        {
            return new GaoCollection(_gaoList);
        }

        public GaoCollection GetCollection(String searchName)
        {
            List<GaoVM> ret = _gaoList.FindAll(x => x.Name.IndexOf(searchName, StringComparison.OrdinalIgnoreCase) >= 0);
            
            return new GaoCollection(ret);
        }

        public void AddData(GaoVM vm)
        {
            vm.CommandAction = On_Command_Execute;
            _gaoList.Add(vm);
            //AddToTable(vm);
            SaveData();
        }

        public void DeleteData(GaoVM vm)
        {
            int idx = _gaoList.IndexOf(vm);
            _gaoList.RemoveAt(idx);
            DelFromTable(vm);
            SaveData();
        }

        public void MoveTo(int oldIdx,int newIdx)
        {
            GaoVM vm = _gaoList.ElementAt(oldIdx);
            _gaoList.RemoveAt(oldIdx);
            _gaoList.Insert(newIdx, vm);
            SaveData();
        }

        public void Insert(int idx, GaoVM vm)
        {
            vm.CommandAction = On_Command_Execute;
            _gaoList.Insert(idx, vm);
            SaveData();
        }

        public void Dispose()
        {

        }

        #endregion


        #region Private Method

        private bool On_Command_Execute(String cmdKey, GaoVM vm)
        {
            if (this.CommandAction != null)
            {
                this.CommandAction(cmdKey, vm);
                return true;
            }
            return false;
        }


        /// <summary>
        /// Initial the Gao table and create the columns.
        /// </summary>
        private void CreateTableColumn()
        {
            _gaoTable.TableName = TABLE_NAME;

            GaoVM vm = new GaoVM();
            foreach (PropertyInfo info in GetDataTablePropList(vm))
            {
                _gaoTable.Columns.Add(info.Name, info.PropertyType);
            }
        }


        void IDisposable.Dispose()
        {
            this.Dispose();
        }

        /// <summary>
        /// Get the Gao view model properties whitch has "DataTableProp" description attribute.
        /// </summary>
        private List<PropertyInfo> GetDataTablePropList(GaoVM vm)
        {
            List<PropertyInfo> propList = new List<PropertyInfo>();

            foreach (PropertyInfo prop in vm.GetType().GetProperties())
            {
                foreach (CustomAttributeData attr in prop.CustomAttributes)
                {
                    CustomAttributeTypedArgument arg = attr.ConstructorArguments.FirstOrDefault(x => (String)x.Value == GaoVM.ATTR_DESC_DATATABLEPROP);
                    if (arg != null)
                    {
                        propList.Add(prop);
                        break;
                    }
                }
            }

            return propList;
        }


        private void AddToTable(GaoVM vm)
        {
            DataRow row = _gaoTable.NewRow();
            foreach (PropertyInfo prop in GetDataTablePropList(vm))
            {
                row[prop.Name] = prop.GetValue(vm);
            }
            _gaoTable.Rows.Add(row);
        }

        private void DelFromTable(GaoVM vm)
        {
            List<DataRow> rows = new List<DataRow>();
            foreach (DataRow row in _gaoTable.Rows)
            {
                if ((String)row["Text"] == vm.Text)
                {
                    rows.Add(row);
                }
            }
            foreach (DataRow row in rows)
            {
                _gaoTable.Rows.Remove(row);
            }
        }
        #endregion
    }
}
