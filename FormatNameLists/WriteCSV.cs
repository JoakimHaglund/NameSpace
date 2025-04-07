using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FormatNameLists
{
    class WriteCSV
    {
        private string? FilePath { get; set; } = null;
        private List<string> HeaderColumns { get; set; } = new();
        private bool SetHeadersFromType { get; set; } = false;
        private char Delimitor { get; set; } = ',';
        public WriteCSV SetFilePath(string filePath)
        {
            FilePath = filePath;
            return this;
        }
        public WriteCSV SetHeaderColumns(List<string> columns)
        {
            HeaderColumns = columns;
            return this;
        }
        public WriteCSV UsePropertynamesAsHeader()
        {
            SetHeadersFromType = true;
            return this;
        }
        public WriteCSV SetDelimitor(char input)
        {
            Delimitor = input;
            return this;
        }
        public void Write<T>(List<T> inputObjects)
        {
            if (FilePath == null) throw new ArgumentNullException("file path can not be null");
            if (inputObjects == null) throw new ArgumentNullException("input can not be null");

            var properties = typeof(T).GetProperties();
            var linesToWrite = new List<string>();

            if (SetHeadersFromType)
            {
                var headers = new List<string>();
                foreach (var property in properties)
                {
                    headers.Add(property.Name);
                }
                HeaderColumns = headers;
            }
            if (HeaderColumns.Count > 0) {
                linesToWrite.Add(SetLine(HeaderColumns));
            }
            foreach (var inputObject in inputObjects)
            {
                var values = new List<string>();
                foreach (var property in properties)
                {
                    var value = property.GetValue(inputObject) ?? string.Empty;
                    values.Add(value.ToString() ?? string.Empty);
                }
                linesToWrite.Add(SetLine(values));
            }
            try
            {
                File.WriteAllLines(FilePath, linesToWrite);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }
        public void Write(Dictionary<string, string> input)
        {

        }
        public void Write(Dictionary<string, int> input)
        {

        }
        private string SetLine(List<string> inputs)
        {
            string result = "";
            foreach (var input in inputs)
            {
                result += input.Trim() + Delimitor;
            }
            return result.Remove(result.Length - 1);
        }
    }
}
