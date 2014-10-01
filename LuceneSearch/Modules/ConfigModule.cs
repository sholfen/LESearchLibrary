using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

using System.Xml.Linq;

namespace LuceneSearch.Modules
{
    public class ConfigModule
    {
        public static string GetIndexPath()
        {
            string value = string.Empty;
            try
            {
                value = ConfigurationManager.AppSettings["IndexPath"];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return value;
        }

        public static Dictionary<string, FieldItem> GetJsonSetting(string filePath, string typeName)
        {
            Dictionary<string, FieldItem> typeSetting = new Dictionary<string, FieldItem>();
            string result = string.Empty;
            XElement xelement = XElement.Load(filePath);
            XElement item = xelement.Elements("Type").Where(x => x.Attribute("name").Value == typeName).First();
            foreach (XElement field in item.Elements("Field"))
            {
                FieldItem fieldItem = new FieldItem();
                fieldItem.DataType = Type.GetType(field.Attribute("ValueType").Value);
                fieldItem.FieldName = field.Attribute("Name").Value;
                fieldItem.IsStore = bool.Parse(field.Attribute("IsStore").Value);
                typeSetting.Add(field.Attribute("Name").Value, fieldItem);
            }

            return typeSetting;
        }
    }

    public class FieldItem
    {
        public string FieldName { get; set; }
        public Type DataType { get; set; }
        public bool IsStore { get; set; }
    }
}
