using System.ComponentModel;
using System.Xml.Serialization;

namespace SpreadsheetEngine;
    
public class Cell: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected int _rowIndex;
        protected int _columnIndex;
        public int RowIndex
        {
            get { return _rowIndex; }
            set
            {
                _rowIndex = value;
                
            }
        }

        public int ColumnIndex
        {
            get { return _columnIndex; }
            set
            {
                _columnIndex = value;
            }
        }

        protected string _text = "";
        public string Text {
            get
            {
                return _text;
            }
            set
            {
                if (_text != value)
                {
                    if (_columnIndex == 0)
                    {
                        _text = (_rowIndex + 1).ToString();
                        _value = _text;
                        return;
                    }

                    _text = value;
                   
                }
            }
        }

        protected string _value;

        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public uint BackgroundColor = 0xAAAAAAAA;
        
        public Cell(int ri, int ci)
        {
            _rowIndex = ri;
            _columnIndex = ci;
        }
        public Cell()
        {
        }

        public string GetCellId()
        {
            return _rowIndex.ToString() + ":" + _columnIndex.ToString();
        }

        public void FireTextPropertyChanged()
        {
            //This method is called by Avalonia DataGrid logic side...
            OnPropertyChanged(Text);
        }

        public bool IsDefault()
        {
            if ((Text == null || Text.Length == 0) && BackgroundColor == 0xAAAAAAAA)
                return true;
            return false;
        }

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


