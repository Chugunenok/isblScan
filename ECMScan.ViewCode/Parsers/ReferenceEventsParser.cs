using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ISBLScan.ViewCode
{
    static class ReferenceEventsParser
    {
        public static Dictionary<Char, String> SectionCodeToName = new Dictionary<char, string>()
        {
            {'Ш', "Карточка"},
            {'Т', "Таблица"},
            {'С', "Таблица 2"},
            {'Р', "Таблица 3"},
            {'О', "Таблица 4"},
            {'Н', "Таблица 5"},
            {'М', "Таблица 6"},
            {'Q', "Таблица 7"},
            {'W', "Таблица 8"},
            {'U', "Таблица 9"},
            {'R', "Таблица 10"},
            {'I', "Таблица 11"},
            {'Y', "Таблица 12"},
            {'B', "Таблица 13"},
            {'H', "Таблица 14"},
            {'L', "Таблица 15"},
            {'M', "Таблица 16"},
            {'N', "Таблица 17"},
            {'P', "Таблица 18"},
            {'O', "Таблица 19"},
            {'S', "Таблица 20"},
            {'T', "Таблица 21"},
            {'V', "Таблица 22"},
            {'X', "Таблица 23"},
            {'Z', "Таблица 24"},
            {'К', "Действие"},
            {'A', "Действие"},
            {'C', "Карточка"},
            {'D', "Таблица"},
            {'E', "Таблица 2"},
            {'F', "Таблица 3"},
            {'G', "Таблица 4"},
            {'J', "Таблица 5"},
            {'K', "Таблица 6"}
        };
        public static Dictionary<String, String> SectionLocalizedIDToName = new Dictionary<String, string>()
        {
            {"SYSRES_SYSCOMP.REQUISITE_SECTION_CARD", "Карточка"},
            {"SYSRES_SYSCOMP.REQUISITE_SECTION_TABLE", "Таблица"},
            {"SYSRES_SYSCOMP.REQUISITE_SECTION_TABLE2", "Таблица 2"},
            {"SYSRES_SYSCOMP.REQUISITE_SECTION_TABLE3", "Таблица 3"},
            {"SYSRES_SYSCOMP.REQUISITE_SECTION_TABLE4", "Таблица 4"},
            {"SYSRES_SYSCOMP.REQUISITE_SECTION_TABLE5", "Таблица 5"},
            {"SYSRES_SYSCOMP.REQUISITE_SECTION_TABLE6", "Таблица 6"},
            {"SYSRES_SYSCOMP.REQUISITE_SECTION_TABLE7", "Таблица 7"},
            {"SYSRES_SYSCOMP.REQUISITE_SECTION_TABLE8", "Таблица 8"},
            {"SYSRES_SYSCOMP.REQUISITE_SECTION_TABLE9", "Таблица 9"},
            {"SYSRES_SYSCOMP.REQUISITE_SECTION_TABLE10", "Таблица 10"},
            {"SYSRES_SYSCOMP.REQUISITE_SECTION_TABLE11", "Таблица 11"},
            {"SYSRES_SYSCOMP.REQUISITE_SECTION_TABLE12", "Таблица 12"},
            {"SYSRES_SYSCOMP.REQUISITE_SECTION_TABLE13", "Таблица 13"},
            {"SYSRES_SYSCOMP.REQUISITE_SECTION_TABLE14", "Таблица 14"},
            {"SYSRES_SYSCOMP.REQUISITE_SECTION_TABLE15", "Таблица 15"},
            {"SYSRES_SYSCOMP.REQUISITE_SECTION_TABLE16", "Таблица 16"},
            {"SYSRES_SYSCOMP.REQUISITE_SECTION_TABLE17", "Таблица 17"},
            {"SYSRES_SYSCOMP.REQUISITE_SECTION_TABLE18", "Таблица 18"},
            {"SYSRES_SYSCOMP.REQUISITE_SECTION_TABLE19", "Таблица 19"},
            {"SYSRES_SYSCOMP.REQUISITE_SECTION_TABLE20", "Таблица 20"},
            {"SYSRES_SYSCOMP.REQUISITE_SECTION_TABLE21", "Таблица 21"},
            {"SYSRES_SYSCOMP.REQUISITE_SECTION_TABLE22", "Таблица 22"},
            {"SYSRES_SYSCOMP.REQUISITE_SECTION_TABLE23", "Таблица 23"},
            {"SYSRES_SYSCOMP.REQUISITE_SECTION_TABLE24", "Таблица 24"},
            {"SYSRES_SYSCOMP.CARD_ACTION_SECTION_ACTIONS", "Действие"}
        };

        static Dictionary<string, String> EventCodePartToTitle = new Dictionary<string, string>()
        {
            {"DIALOG.CREATE", "Диалог. Создание"},
            {"DIALOG.VALID_CLOSE_WITH_RESULT", "Диалог. Закрытие Возможность"},
            {"DIALOG.CLOSE", "Диалог. Закрытие"},
            {"FORM.DIALOG_SHOW", "Форма. Показ"},
            {"FORM.DIALOG_HIDE", "Форма. Скрытие"},

            {"OPEN", "Открытие"},
            {"CLOSE", "Закрытие"},
            {"DATASET", "Набор данных"},
            {"CARD", "Запись"},
            {"VALID_DELETE", "Удаление возможность"},
            {"BEFORE_DELETE", "Удаление до"},
            {"AFTER_DELETE", "Удаление после"},
            {"BEFORE_INSERT", "Добавление до"},
            {"AFTER_INSERT", "Добавление после"},
            {"VALID_UPDATE", "Сохранение возможность"},
            {"BEFORE_UPDATE", "Сохранение до"},
            {"AFTER_UPDATE", "Сохранение после"},
            {"BEFORE_CANCEL", "Отмена до"},
            {"AFTER_CANCEL", "Отмена после"},
            {"LISTFORM", "Форма-список"},
            {"FORM", "Форма-карточка"},
            {"SHOW", "Показ"},
            {"HIDE", "Скрытие"},
            {"TABLE", "Таблица"},
            {"OPERATION", "Операция"},
            {"EXECUTION", "Выполнение"}
        };

        static string[] EventIdsRegExPatterns =
        {
            "DATASET{9AFC8FC7-30C4-4076-9076-6E09A49B791C}.[A-Z А-Я_]+",
            "CARD{2147B5A6-496E-4EFF-88D9-78970D889F1F}.[A-Z А-Я_]+",
            "FORM{B28D55C1-651A-46C9-AD4E-50E73EF213A8}.[A-Z А-Я_]+",
            "LISTFORM{EF850CF0-3135-4D1F-8726-74F95C4D08C7}.[A-Z А-Я_]+",
            "OPERATION{C6CE6EDC-3645-4BBC-B00F-587BD2A54B4C}.[A-Z А-Я_]+",
            "TABLE{D402E843-74B2-4DC1-BFFD-DE677B48452C}[0-9]*.[A-Z А-Я_]+",
            "DIALOG{3AA220D8-D906-4914-8586-F534A4C3767E}.[A-Z А-Я_]+",
            "КАРТОЧКА.[A-Z А-Я_]+",
            "НАБОР ДАННЫХ.[A-Z А-Я_]+",
            "ТАБЛИЦА[0-9]*.[A-Z А-Я_]+",
            "DATASET.[A-Z А-Я_]+",
            "CARD.[A-Z А-Я_]+",
            "FORM.[A-Z А-Я_]+",
            "TABLE[0-9]*.[A-Z А-Я_]+"
        };
        public static void ParseEvents(string eventsText, IsbNode parent)
        {
            var pattern = $"^({String.Join("|", EventIdsRegExPatterns)})";
            var events = Regex.Split(eventsText, pattern, RegexOptions.Multiline);
            if (events.Length > 0)
            {
                var eventsNode = new IsbNode("События");
                IsbNode eventNode = null;
                foreach (var eventText in events)
                {
                    if (!String.IsNullOrWhiteSpace(eventText))
                    {
                        if (Regex.IsMatch(eventText, pattern))
                        {
                            var eventName = Regex.Replace(eventText, "\\{[-\\w]+\\}", "");
                            foreach (var part in EventCodePartToTitle.Keys)
                            {
                                eventName = eventName.Replace(part, EventCodePartToTitle[part]);
                            }
                            eventNode = new IsbNode(eventName);
                        }
                        else
                        {
                            if(eventNode == null) eventNode = new IsbNode("Неизвестное событие");
                            eventNode.Text = eventText.Substring(2);
                            eventsNode.Nodes.Add(eventNode);
                        }
                    }
                }
                parent.Nodes.Add(eventsNode);
            }
        }
    }
}
