using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ISBLScan.ViewCode
{
    static class RouteParser
    {
        private static string GetNodeString(XmlNode node)
        {
            string parsedString = "";
            parsedString = node.InnerText;
            parsedString = parsedString.Replace("{5314B05F-CF9F-4F66-99EC-24992A5FB114}", "");
            try
            {
                byte[] data = Convert.FromBase64String(parsedString);
                parsedString = System.Text.Encoding.GetEncoding(1251).GetString(data);
            }
            catch { }
            return parsedString;
        }

        public static void ParseBlockProperties(string schemaString, IsbNode routeBlockNode)
        {
            var schema = new XmlDocument();
            schema.LoadXml(schemaString);

            var events = schema.SelectNodes("/Settings/Event/node()");
            foreach (XmlNode eventXmlNode in events)
            {
                var eventString = GetNodeString(eventXmlNode);
                if (!System.String.IsNullOrEmpty(eventString))
                {
                    var eventNode = new IsbNode();
                    eventNode.Name = eventXmlNode.Name;
                    eventNode.Text = eventString;
                    routeBlockNode.Nodes.Add(eventNode);
                }
            }

            var properties = schema.SelectNodes("//Properties/Property[(@Type = '2' or @Type = '5') and @Name != 'Name']");
            foreach (XmlNode property in properties)
            {
                var propertyStringNode = property.SelectSingleNode("Value/Value");
                if (propertyStringNode != null)
                {
                    var propertyString = GetNodeString(propertyStringNode);
                    if (!System.String.IsNullOrEmpty(propertyString))
                    {
                        var blockStringNode = new IsbNode();
                        blockStringNode.Name = property.Attributes["Description"].Value;
                        blockStringNode.Text = propertyString;
                        routeBlockNode.Nodes.Add(blockStringNode);
                    }
                }
            }
        }

        public static void ParseRoute(string schemaString, IsbNode routeNode)
        {
            var schema = new XmlDocument();
            schema.LoadXml(schemaString);

            var events = schema.SelectNodes("/Settings/Event/node()");
            var eventNameToTitle = new Dictionary<String, String>()
                            {
                                {"InitScript", "Начало выбора"},
                                {"Script", "Завершение выбора"},
                                {"TaskStart", "Возможность старта"},
                                {"TaskAbortPossibility", "Возможность прекращения"},
                                {"TaskAbort", "Прекращение"}
                            };
            foreach (XmlNode eventXmlNode in events)
            {
                var eventString = GetNodeString(eventXmlNode);
                if (!System.String.IsNullOrEmpty(eventString))
                {
                    var eventNode = new IsbNode();
                    eventNode.Name = eventNameToTitle.ContainsKey(eventXmlNode.Name) ? eventNameToTitle[eventXmlNode.Name] : eventXmlNode.Name;
                    eventNode.Text = eventString;
                    routeNode.Nodes.Add(eventNode);
                }
            }
            var routeProperties = schema.SelectNodes("/Settings/Properties/Property[(@Type = '2' or @Type = '5') and @Name != 'Name']");
            foreach (XmlNode property in routeProperties)
            {
                var propertyStringNode = property.SelectSingleNode("Value/Value");
                if (propertyStringNode != null)
                {
                    var propertyString = GetNodeString(propertyStringNode);
                    if (!System.String.IsNullOrEmpty(propertyString))
                    {
                        var routeStringNode = new IsbNode(property.Attributes["Description"].Value);
                        routeStringNode.Text = propertyString;
                        routeNode.Nodes.Add(routeStringNode);
                    }
                }
            }

            var blocks = schema.SelectNodes("//Block");
            foreach (XmlNode block in blocks)
            {
                var blockNode = new IsbNode();
                var nameProperty = block.SelectSingleNode("Properties/Property[(@Type = '2' or @Type = '5') and @Name = 'Name']/Value/Value");
                if (nameProperty != null) blockNode.Name = block.Attributes["ID"].Value + ". " + GetNodeString(nameProperty);
                else blockNode.Name = block.Attributes["ID"].Value;

                var properties = block.SelectNodes("Properties/Property[(@Type = '2' or @Type = '5') and @Name != 'Name']");
                foreach (XmlNode property in properties)
                {
                    var propertyStringNode = property.SelectSingleNode("Value/Value");
                    if (propertyStringNode != null)
                    {
                        var propertyString = GetNodeString(propertyStringNode);
                        if (!System.String.IsNullOrEmpty(propertyString))
                        {
                            var blockStringNode = new IsbNode(property.Attributes["Description"].Value);
                            blockStringNode.Text = propertyString;
                            blockNode.Nodes.Add(blockStringNode);
                        }
                    }
                }

                if (blockNode.Nodes.Count > 0)
                {
                    routeNode.Nodes.Add(blockNode);
                }
                else blockNode = null;

            }
        }
    }
}
