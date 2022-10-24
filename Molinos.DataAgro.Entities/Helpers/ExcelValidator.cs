using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Molinos.DataAgro.Entities.Helpers
{
    public class ExcelValidator
    {
        private List<ExcelValidatorItem> _items;

        public ExcelValidator(List<ExcelValidatorItem> items)
        {
            this._items = items;
        }

        public ExcelValidatorResult Validate(DataTable dt, bool hasHeader = false)
        {
            var ret = new ExcelValidatorResult();

            var rows = dt.AsEnumerable().Select(x => x.ItemArray).Skip(hasHeader ? 1 : 0);

            for (int i = 0; i < rows.Count(); i++)
            {
                var retRow = ValidateRow(rows.ElementAt(i));

                //if (!retRow.IsValid)
                //{
                retRow.Row = i;
                ret.RowsResult.Add(retRow);
                //}
            }

            return ret;
        }

        private ExcelValidatorRowResult ValidateRow(object[] row)
        {
            var ret = new ExcelValidatorRowResult();
            ret.ContratoCorredor = row[0].ToString();
            foreach (var item in this._items)
            {
                var retCell = ValidateCell(row[item.Position], item);

                if (!retCell.IsValid)
                    ret.ItemsResult.Add(retCell);
            }

            return ret;
        }

        private ExcelValidatorItemResult ValidateCell(object cell, ExcelValidatorItem item)
        {
            var ret = new ExcelValidatorItemResult();
            ret.Item = item;

            if (item.Required && item.Type != ExcelValidationColumnType.Bool && (cell == null || string.IsNullOrEmpty(cell.ToString().Trim())))
            {
                ret.Errors.Add(string.Format("El campo {0} es obligatorio", item.Name));
                return ret;
            }
            if (!item.Required && string.IsNullOrEmpty(cell.ToString().Trim()))
            {
                return ret;
            }
            switch (item.Type)
            {
                case ExcelValidationColumnType.Int:
                    int n;
                    if (!int.TryParse(cell.ToString().Trim(), out n))
                    {
                        ret.Errors.Add(string.Format("El campo {0} debe ser un número", item.Name));
                    }
                    break;
                case ExcelValidationColumnType.Long:
                    long l;
                    if (!long.TryParse(cell.ToString().Trim(), out l))
                    {
                        ret.Errors.Add(string.Format("El campo {0} debe ser un número", item.Name));
                    }
                    break;
                case ExcelValidationColumnType.Decimal:
                    decimal d;
                    if (!decimal.TryParse(cell.ToString().Trim(), out d))
                    {
                        ret.Errors.Add(string.Format("El campo {0} debe ser un número decimal", item.Name));
                    }
                    break;
                case ExcelValidationColumnType.List:
                    if (item.Options != null && !item.Options.Select(x=> x.ToLower()).Contains(cell.ToString().Trim().ToLower()))
                    {
                        ret.Errors.Add(string.Format("El campo {0} debe ser uno de los siguientes valores: {1}", item.Name, string.Join(", ", item.Options.ToArray())));
                    }
                    break;
                case ExcelValidationColumnType.Date:
                    DateTime dt;
                    if (!DateTime.TryParse(cell.ToString().Trim(), out dt))
                    {
                        ret.Errors.Add(string.Format("El campo {0} debe ser una fecha válida", item.Name));
                    }
                    break;
                case ExcelValidationColumnType.Bool:
                    var b = cell.ToString().Trim();

                    if (!string.IsNullOrEmpty(b) && b.ToUpper() != "X")
                    {
                        ret.Errors.Add(string.Format("El campo {0} debe ser estar vacío o ser una X", item.Name));
                    }
                    break;
                default:
                    break;
            }

            return ret;
        }
    }

    public class ExcelValidatorItem
    {
        public string Name { get; set; }
        public ExcelValidationColumnType Type { get; set; }
        public bool Required { get; set; }
        public int Position { get; set; }
        public ExcelValidationErrorType ErrorType { get; set; }

        public List<string> Options { get; set; }

        //public Func<object, bool> CustomItemValidationByValue { get; set; }
        //public Func<object[], bool> CustomItemValidationByRow { get; set; }
        //public Func<object[][], bool> CustomItemValidationByTable { get; set; }

    }

    public class ExcelValidatorResult
    {
        public List<ExcelValidatorRowResult> RowsResult { get; set; }
        public bool IsValid
        {
            get
            {
                return /*this.RowsResult.All(x => x.IsValid) &&*/ !this.RowsResult.Any(x => x.ItemsResult.Any(y => y.Item.ErrorType == ExcelValidationErrorType.Fatal));
            }
        }

        public List<ExcelValidatorResumeItem> Resume
        {
            get
            {
                var ret = new List<ExcelValidatorResumeItem>();

                var rowFatal = this.RowsResult.Where(x => x.ItemsResult.Any(y => y.Item.ErrorType == ExcelValidationErrorType.Fatal)).FirstOrDefault();
                if (rowFatal != null)
                {
                    var itemFatal = rowFatal.ItemsResult.FirstOrDefault(y => y.Item.ErrorType == ExcelValidationErrorType.Fatal);

                    var resumeItem = new ExcelValidatorResumeItem()
                    {
                        IsFatal = true,
                        Row = rowFatal.Row,
                        ContratoCorredor = rowFatal.ContratoCorredor
                    };

                    resumeItem.Errors.Add(string.Join("\n", itemFatal.Errors));
                    ret.Add(resumeItem);
                }
                else
                {
                    foreach (var row in this.RowsResult)
                    {
                        var item = new ExcelValidatorResumeItem();
                        item.Row = row.Row;
                        item.ContratoCorredor = row.ContratoCorredor;

                        foreach (var itemResult in row.ItemsResult)
                        {
                            item.Errors.Add(string.Join("\n", itemResult.Errors));
                        }

                        ret.Add(item);
                    }
                }

                return ret;
            }
        }

        public ExcelValidatorResult()
        {
            this.RowsResult = new List<ExcelValidatorRowResult>();
        }
    }

    public class ExcelValidatorRowResult
    {
        public int Row { get; set; }
        public string ContratoCorredor { get; set; }
        public List<ExcelValidatorItemResult> ItemsResult { get; set; }
        public bool HasError
        {
            get
            {
                return this.ItemsResult.All(x => !x.IsValid);
            }
        }

        public ExcelValidatorRowResult()
        {
            this.ItemsResult = new List<ExcelValidatorItemResult>();
        }
    }

    public class ExcelValidatorItemResult
    {
        public ExcelValidatorItem Item { get; set; }
        public bool IsValid
        {
            get
            {
                return !this.Errors.Any();
            }
        }
        public List<string> Errors { get; set; }

        public ExcelValidatorItemResult()
        {
            this.Errors = new List<string>();
        }
    }

    public class ExcelValidatorResumeItem
    {
        public int Row { get; set; }
        public string ContratoCorredor { get; set; }
        public List<string> Errors { get; set; } //field, errors
        public bool IsFatal { get; set; }
        public bool HasError
        {
            get { return Errors.Count != 0; }
        }
        public ExcelValidatorResumeItem()
        {
            this.Errors = new List<string>();
        }
    }

    public enum ExcelValidationColumnType
    {
        Int,
        String,
        Date,
        Bool,
        Decimal,
        List,
        Long
    }

    public enum ExcelValidationErrorType
    {
        Fatal,
        Error
    }
}
