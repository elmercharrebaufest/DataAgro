using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    [DataContract]
    public class ErrorMessage
    {
        private int mintItem;
        private int mintErrorCode;
        private int mintLogId;
        private string mstrMessage;
        private string mstrSource;
        private string mstrLargeDescription;
        private bool mblnTranslate;
        private string mstrFormat;
        private List<object> marrArgs;

        [DataMember]
        public int Item
        {
            get
            {
                return this.mintItem;
            }
            set
            {
                this.mintItem = value;
            }
        }

        [DataMember]
        public int ErrorCode
        {
            get
            {
                return this.mintErrorCode;
            }
            set
            {
                this.mintErrorCode = value;
            }
        }

        [DataMember]
        public int LogId
        {
            get
            {
                return this.mintLogId;
            }
            set
            {
                this.mintLogId = value;
            }
        }

        [DataMember]
        public string Message
        {
            get
            {
                return this.mstrMessage;
            }
            set
            {
                this.mstrMessage = value;
            }
        }

        [DataMember]
        public string Source
        {
            get
            {
                return this.mstrSource;
            }
            set
            {
                this.mstrSource = value;
            }
        }

        [DataMember]
        public string LargeDescription
        {
            get
            {
                return this.mstrLargeDescription;
            }
            set
            {
                this.mstrLargeDescription = value;
            }
        }

        [DataMember]
        public bool Translate
        {
            get
            {
                return this.mblnTranslate;
            }
            set
            {
                this.mblnTranslate = value;
            }
        }

        [DataMember]
        public string Format
        {
            get
            {
                return this.mstrFormat;
            }
            set
            {
                this.mstrFormat = value;
            }
        }

        public List<object> Args
        {
            get
            {
                return this.marrArgs;
            }
            set
            {
                this.marrArgs = value;
            }
        }

        public ErrorMessage()
        {
            this.mintItem = 0;
            this.mintErrorCode = 0;
            this.mintLogId = 0;
            this.mstrMessage = "";
            this.mstrSource = "";
            this.mstrLargeDescription = "";
            this.mblnTranslate = false;
            this.mstrFormat = "";
            this.marrArgs = new List<object>();
        }

        public ErrorMessage(string strMessage)
        {
            this.mintItem = 0;
            this.mintErrorCode = 0;
            this.mintLogId = 0;
            this.mstrMessage = strMessage;
            this.mstrSource = "";
            this.mstrLargeDescription = "";
            this.mblnTranslate = false;
            this.mstrFormat = "";
            this.marrArgs = new List<object>();
        }

        public ErrorMessage(string strMessage, string strSource)
        {
            this.mintItem = 0;
            this.mintErrorCode = 0;
            this.mintLogId = 0;
            this.mstrMessage = strMessage;
            this.mstrSource = strSource;
            this.mstrLargeDescription = "";
            this.mblnTranslate = false;
            this.mstrFormat = "";
            this.marrArgs = new List<object>();
        }

        public ErrorMessage(string strFormat, string strSource, params object[] args)
        {
            this.mintItem = 0;
            this.mintErrorCode = 0;
            this.mintLogId = 0;
            this.mstrMessage = "";
            this.mstrSource = strSource;
            this.mstrLargeDescription = "";
            this.mblnTranslate = false;
            this.mstrFormat = strFormat;
            this.marrArgs = ((IEnumerable<object>)args).ToList<object>();
        }

        public ErrorMessage(int intErrorCode, string strMessage)
        {
            this.mintItem = 0;
            this.mintLogId = 0;
            this.mintErrorCode = intErrorCode;
            this.mstrMessage = strMessage;
            this.mstrSource = "";
            this.mstrLargeDescription = "";
            this.mblnTranslate = false;
            this.mstrFormat = "";
            this.marrArgs = new List<object>();
        }

        public ErrorMessage(int intItem, int intErrorCode, string strMessage, string strLargeDescription)
        {
            this.mintItem = intItem;
            this.mintErrorCode = intErrorCode;
            this.mintLogId = 0;
            this.mstrMessage = strMessage;
            this.mstrSource = "";
            this.mstrLargeDescription = strLargeDescription;
            this.mblnTranslate = false;
            this.mstrFormat = "";
            this.marrArgs = new List<object>();
        }

        public ErrorMessage(int intItem, int intErrorCode, int intLogId, string strMessage, string strLargeDescription)
        {
            this.mintItem = intItem;
            this.mintErrorCode = intErrorCode;
            this.mintLogId = intLogId;
            this.mstrMessage = strMessage;
            this.mstrSource = "";
            this.mstrLargeDescription = strLargeDescription;
            this.mblnTranslate = false;
            this.mstrFormat = "";
            this.marrArgs = new List<object>();
        }

        public ErrorMessage(int intNroLinea, string strCampo, string strSource, string strMessage)
          : this(strMessage, strSource)
        {
            this.mstrSource = string.Format("#root|{0}|{1}|{2}", (object)strSource, (object)intNroLinea.ToString(), (object)strCampo);
        }

        public ErrorMessage(Exception oException)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("{0} : {1}{2}", (object)"Type    ", (object)oException.GetType().FullName, (object)Environment.NewLine);
            stringBuilder.AppendLine("");
            for (Exception exception = oException; exception != null; exception = exception.InnerException)
            {
                if (exception is SqlException)
                {
                    SqlException sqlException = (SqlException)exception;
                    int num1 = 0;
                    int num2 = checked(sqlException.Errors.Count - 1);
                    int index = num1;
                    while (index <= num2)
                    {
                        string str = "Number " + Conversions.ToString(sqlException.Errors[index].Number) + "  Message: " + sqlException.Errors[index].Message + "  LineNumber: " + Conversions.ToString(sqlException.Errors[index].LineNumber) + "  Source: " + sqlException.Errors[index].Source + "  Procedure: " + sqlException.Errors[index].Procedure;
                        stringBuilder.AppendFormat("{0} : {1}{2}", (object)"SQL Err.", (object)str, (object)Environment.NewLine);
                        stringBuilder.AppendLine("");
                        checked { ++index; }
                    }
                }
            }
            stringBuilder.AppendFormat("{0} : {1}{2}", (object)"Error   ", (object)oException.ToString(), (object)Environment.NewLine);
            this.mintItem = 0;
            this.mintErrorCode = 0;
            this.mintLogId = -1;
            this.mstrMessage = oException.Message;
            this.mstrLargeDescription = stringBuilder.ToString();
            this.mstrSource = "";
            this.mstrFormat = "";
            this.marrArgs = new List<object>();
        }

        public override string ToString()
        {
            string Expression = "";
            if (this.mintLogId > 0)
                Expression = Expression + "Suceso Id: " + Conversions.ToString(this.mintLogId) + "\r\n";
            if (Strings.Len(Strings.Trim(this.mstrMessage)) > 0)
            {
                if (Strings.Len(Expression) > 0)
                    Expression += "\r\n";
                Expression = Expression + Strings.Trim(this.mstrMessage) + "\r\n";
            }
            else if (Strings.Len(Strings.Trim(this.mstrFormat)) > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat(this.mstrFormat, this.marrArgs.ToArray());
                Expression = Expression + Strings.Trim(stringBuilder.ToString()) + "\r\n";
            }
            if (Strings.Len(Strings.Trim(this.mstrLargeDescription)) > 0)
            {
                if (Strings.Len(Expression) > 0)
                    Expression += "\r\n";
                Expression = Expression + this.mstrLargeDescription + "\r\n";
            }
            return Expression;
        }
    }
}
