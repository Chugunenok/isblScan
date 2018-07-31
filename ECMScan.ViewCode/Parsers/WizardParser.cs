using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISBLScan.ViewCode
{
    static class WizardParser
    {
        static Dictionary<String, String> _wizardEventCodeToName = new Dictionary<string, string>()
        {
            {"wetWizardBeforeSelection", "До выбора"},
            {"wetWizardStart", "Старт"},
            {"wetWizardFinish", "Завершение"},
            {"wetStepStart", "Старт"},
            {"wetStepFinish", "Завершение"},
        };
        public static void ParseWizardText(string originText, IsbNode wizardNode)
        {

            /**********************************************************************
			 * Структура свойств мастера находится в структуре, похожей на
			 * Delphi Form. Все теги расположены в отдельных строках.
			 * Значения тегов в формате Unicode. Свойствам мастера присуща
			 * Объектная структура. Иерархия соблюдается за счёт табуляции
			 * тегов. Если значение тега очень длинное то оно разбивается на
			 * несколько строк с указанием в конце строки символа конкатенации(+)
			 **********************************************************************/

            /**********************************************************************
			 * Сначала идёт три "События мастера":
			 * 	До выбора
			 * 	Начало
			 * 	Завершение
			 * 
			 * Потом идут этапы мастера, например "Этап 1: Запрос параметров совещания"
			 * 	Этап содержит два события:
			 * 		Начало
			 * 		Завершение
			 * 	А также события:
			 * 		Previous
			 * 		Next
			 * 		Finish
			 * 		Cancel
			 * 	Ещё есть событие:
			 * 		ОК
			 **********************************************************************/
            var parsedWizardDfm = DfmParser.Parse(originText);

            // Wizard Events
            var events = parsedWizardDfm.Nodes.Where(n => n.PropertyName == "Events").First().Nodes;
            foreach (var ev in events)
            {
                var textNode = ev.Nodes.Where(n => n.PropertyName == "ISBLText").FirstOrDefault();
                if (textNode != null)
                {
                    var wizardEventNode = new IsbNode();
                    wizardEventNode.Text = (string)textNode.PropertyValue;
                    var eventNameNode = ev.Nodes.Where(n => n.PropertyName == "EventType").FirstOrDefault();
                    if (eventNameNode != null)
                    {
                        var eventCode = (string)eventNameNode.PropertyValue;
                        wizardEventNode.Name = _wizardEventCodeToName.ContainsKey(eventCode) ? _wizardEventCodeToName[eventCode] : eventCode;
                    }
                    else
                    {
                        wizardEventNode.Name = "Неизвестное событие";
                    }

                    wizardNode.Nodes.Add(wizardEventNode);
                }
            }

            var StepNumber = 1;
            foreach (var step in parsedWizardDfm.Nodes.Where(n => n.PropertyClass?.EndsWith("StepList") ?? false).First().Nodes.Where(n => n.PropertyClass?.EndsWith("WizardStep") ?? false))
            {
                var wizardStep = new IsbNode("");

                foreach (var eventNode in step.Nodes.Where(n => n.PropertyName == "Events").First().Nodes)
                {
                    var stepEventNode = new IsbNode();
                    foreach (var eventParam in eventNode.Nodes)
                    {
                        if (eventParam.PropertyName == "ISBLText")
                        {
                            stepEventNode.Text = (string)eventParam.PropertyValue;
                        }
                        if (eventParam.PropertyName == "EventType")
                        {
                            var eventCode = (string)eventParam.PropertyValue;
                            stepEventNode.Name = _wizardEventCodeToName.ContainsKey(eventCode) ? _wizardEventCodeToName[eventCode] : eventCode;
                        }
                    }
                    if (!String.IsNullOrWhiteSpace(stepEventNode.Text))
                    {
                        wizardStep.Nodes.Add(stepEventNode);
                    }
                }

                foreach (var actionNode in step.Nodes.Where(n => n.PropertyClass?.EndsWith("WizardActionList") ?? false).First().Nodes.Where(n => n.PropertyClass?.EndsWith("WizardAction") ?? false))
                {
                    var actionEventNode = new IsbNode();
                    foreach (var eventNode in actionNode.Nodes.Where(n => n.PropertyName == "Events").First().Nodes)
                    {
                        var textNode = eventNode.Nodes.Where(n => n.PropertyName == "ISBLText").FirstOrDefault();
                        if (textNode != null)
                        {
                            actionEventNode.Text = (string)textNode.PropertyValue;
                            actionEventNode.Name = (string)actionNode.Nodes.Where(n => n.PropertyName == "Title").First().PropertyValue;
                            wizardStep.Nodes.Add(actionEventNode);
                        }
                    }
                }

                if (wizardStep.Nodes.Count > 0)
                {
                    wizardStep.Name = StepNumber + ". " + (string)step.Nodes.Where(n => n.PropertyName == "Title").First().PropertyValue;
                    wizardNode.Nodes.Add(wizardStep);
                }
                StepNumber = StepNumber + 1;
            }
        }
    }
}
