using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ExcelHelper
    {
        private string ExcelStr = @"
<html xmlns:x=""urn:schemas-microsoft-com:office:excel"" xmlns:v=""urn:schemas-microsoft-com:vml"">
<head>
	<meta charset=""utf-8"">
    <!--[if gte mso 9]>
        <xml>
            <x:ExcelWorkbook>
                <x:ExcelWorksheets>
                    <x:ExcelWorksheet>
                        <x:Name>Report Data</x:Name>
                        <x:WorksheetOptions>
                            <x:Print>
                                <x:ValidPrinterInfo/>
                            </x:Print>
                        </x:WorksheetOptions>
                    </x:ExcelWorksheet>
                </x:ExcelWorksheets>
            </x:ExcelWorkbook>
        </xml>
    <![endif]-->
    <style>
        {0}
    </style>
</head>
<body>
    {4}
	<table border=""1"" rules=""all"">
        {1}
        <thead>
            {2}
        </thead>
        <tbody>
            {3}
        </tbody>
    </table>
</body>";

        private string DefaultStyle = @"
.caption .content {
    text-decoration: underline;
}
table {
	border-collapse: collapse;
	border-spacing: 0;
	font-family: 微软雅黑, Arail;
	font-size: 12px;
	line-height: 1.5;
    border-color: #ccc;
}
table thead th,
table thead td {
	height: 30px;
	text-align: center;
	font-weight: bold;
	background-color: #5B9BD5;
	color: #fff;
    border-color: #ccc;
    white-space: pre-wrap;
}
table tbody td {
	height: 25px;
    border-color: #ccc;
}
";
        /// <summary>
        /// 样式
        /// </summary>
        public string Style { get; set; }
        /// <summary>
        /// Colgroup模板
        /// </summary>
        public string ColgroupTemplate { get; set; }
        /// <summary>
        /// 表头模板
        /// </summary>
        public string HeaderTemplate { get; set; }
        /// <summary>
        /// 表体模板
        /// </summary>
        public string BodyTemplate { get; set; }
        /// <summary>
        /// 表格描述模板
        /// 一般用于显示表格的额外信息
        /// </summary>
        public string DescriptiontTemplate { get; set; }
        /// <summary>
        /// 列集合
        /// </summary>
        public List<TableColumn> Columns { get; set; }
        /// <summary>
        /// 标题集合
        /// </summary>
        public List<TableCaptionItem> CaptionItems { get; set; }
        /// <summary>
        /// 定义回调
        /// </summary>
        /// <param name="column">表头信息</param>
        /// <param name="data">行数据</param>
        /// <returns></returns>
        public delegate object Callback(TableColumn column, object data, object value);

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExcelHelper()
        {
            Style = DefaultStyle;
        }

        /// <summary>
        /// 带列设置的构造函数
        /// </summary>
        /// <param name="columns"></param>
        public ExcelHelper(List<TableColumn> columns, List<TableCaptionItem> captionItems = null) : this()
        {
            if (columns == null)
            {
                throw new ArgumentNullException("columns");
            }
            Columns = columns;
            CaptionItems = captionItems;
            var strBuilder = new StringBuilder("<tr>\n");
            foreach (var header in Columns)
            {
                strBuilder.AppendFormat("<th style=\"vnd.ms-excel.numberformat: @;\">{0}</th>\n", header.ColumnName);
            }
            strBuilder.Append("</tr>\n");
            HeaderTemplate = strBuilder.ToString();
            if (CaptionItems != null && CaptionItems.Count > 0)
            {
                var items = new List<string>();
                foreach (var captionItem in CaptionItems)
                {
                    items.Add(string.Format(@"
                                <span>
                                    <span class=""label"">{0}：</span>
						            <span class=""content"">{1}</span>
					            </span>", captionItem.Label, captionItem.Content));
                }
                DescriptiontTemplate = string.Format(@"<table class=""caption""><tbody><tr><td colspan=""100"">{0}</td></tr></tbody></table><div>&nbsp;</div>", string.Join("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;", items));
            }
        }

        /// <summary>
        /// 导出excel到文件流
        /// 可以在外部自定义样式、列设置、表头、表体
        /// </summary>
        /// <param name="fs"></param>
        public void ExportExcel(FileStream fs)
        {
            var resultStr = string.Format(ExcelStr, Style, ColgroupTemplate, HeaderTemplate, BodyTemplate, DescriptiontTemplate);
            var resultBytes = System.Text.Encoding.UTF8.GetBytes(resultStr);
            fs.Write(resultBytes, 0, resultBytes.Length);
        }

        /// <summary>
        /// 导出excel到文件流
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fs"></param>
        /// <param name="dataList"></param>
        /// <param name="callback"></param>
        public void ExportExcel<T>(FileStream fs, List<T> dataList, Callback callback = null)
        {
            var strBuilder = new StringBuilder();

            // 根据列信息反射属性
            Type dataType = typeof(T);
            var props = new List<PropertyInfo>();
            foreach (var header in Columns)
            {
                props.Add(!string.IsNullOrEmpty(header.ColumnField) ? dataType.GetProperty(header.ColumnField) : null);
            }

            foreach (var data in dataList)
            {
                strBuilder.Append("<tr>\n");
                for (var i = 0; i < Columns.Count; i++)
                {
                    var column = Columns[i];
                    var prop = props[i];
                    object value = column.IsIgnoreColumnField ? null : column.ColumnValue != null ? column.ColumnValue : prop != null ? prop.GetValue(data) : null;
                    if (callback != null)
                    {
                        value = callback(column, data, value);
                    }
                    var type = column.ColumnFieldType == FieldType.字符串 ? "vnd.ms-excel.numberformat: @;" : "";
                    strBuilder.AppendFormat("<td style=\"{0}\">{1}</td>\n", type, value);
                }
                strBuilder.Append("</tr>\n");
            }
            BodyTemplate = strBuilder.ToString();
            ExportExcel(fs);
        }

        /// <summary>
        /// 导出excel到文件路径
        /// 可以在外部自定义样式、列设置、表头、表体
        /// </summary>
        /// <param name="fs"></param>
        public void ExportExcel(string fileName)
        {
            var fs = new FileStream(fileName, FileMode.OpenOrCreate);
            ExportExcel(fs);
            fs.Close();
        }

        /// <summary>
        /// 导出excel到文件路径
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="dataList"></param>
        /// <param name="callback"></param>
        public void ExportExcel<T>(string fileName, List<T> dataList, Callback callback = null)
        {
            if (callback == null)
            {
                callback = DefaultCallback;
            }
            var fs = new FileStream(fileName, FileMode.OpenOrCreate);
            ExportExcel<T>(fs, dataList, callback);
            fs.Close();
        }

        /// <summary>
        /// 默认导出回调
        /// </summary>
        /// <param name="column"></param>
        /// <param name="data"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public object DefaultCallback(TableColumn column, object data, object value)
        {
            if (value == null) return "";
            if (value is bool)
            {
                return (bool)value ? "是" : "否";
            }
            else if (value is DateTime)
            {
                return ((DateTime)value).ToString("yyyy-MM-dd hh:mm:ss");
            }
            return value;
        }
    }

    /// <summary>
    /// 列
    /// </summary>
    public class TableColumn
    {
        /// <summary>
        /// 列字段
        /// </summary>
        public string ColumnField { get; set; }
        /// <summary>
        /// 列显示名称
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// 列值，如果有该字段则忽略ColumnField
        /// </summary>
        public object ColumnValue { get; set; }
        /// <summary>
        /// 列字段类型
        /// </summary>
        public FieldType ColumnFieldType { get; set; }
        /// <summary>
        /// 是否忽略列字段映射值
        /// </summary>
        public bool IsIgnoreColumnField { get; set; }

        public TableColumn()
        {
            ColumnFieldType = FieldType.字符串;
        }
    }

    /// <summary>
    /// 表头标题项
    /// 类似以下形式
    /// 名称：内容
    /// </summary>
    public class TableCaptionItem
    {
        /// <summary>
        /// 标题名
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// 标题内容
        /// </summary>
        public object Content { get; set; }
    }

    /// <summary>
    /// 列字段
    /// </summary>
    public enum FieldType
    {
        默认,
        数字,
        字符串,
        日期
    }
}
