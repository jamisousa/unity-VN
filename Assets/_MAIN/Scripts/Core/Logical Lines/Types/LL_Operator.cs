using System.Collections;
using System.Collections.Generic;
using DIALOGUE;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;

using static DIALOGUE.LogicalLines.LogicalLineUtils.Expressions;
using System;
using Unity.VisualScripting;

namespace DIALOGUE.LogicalLines
{

    //logical line to delcare and modify variables within a dialogue file
    public class LL_Operator : ILogicalLine
    {
        public string keyword => throw new System.NotImplementedException();

        public IEnumerator Execute(DIALOGUE_LINES line)
        {
            string trimmedLine = line.rawData.Trim();
            string[] parts = Regex.Split(trimmedLine, REGEX_ARITHMATIC);

            if (parts.Length < 3)
            {
                Debug.LogError($"Invalid command: {trimmedLine}");
                yield break;
            }

            string variable = parts[0].Trim().TrimStart(VariableStore.VARIABLE_ID);
            string op = parts[1].Trim();
            string[] remainingParts = new string[parts.Length - 2];
            Array.Copy(parts, 2, remainingParts, 0, parts.Length - 2);

            Debug.Log($"variable is {variable}, op is {op}, remaining parts is {remainingParts}");

            object value = CalculateValue(remainingParts);

            if (value == null)
                yield break;

            ProcessOperator(variable, op, value);
        }



        private void ProcessOperator(string variable, string op, object value)
        {

            Debug.Log($"Processing operator with values: variable is {variable}, op is {op} and value is {value}");
            

            if (VariableStore.TryGetValue(variable, out object currentValue))
            {

                ProcessOperatorOnVariable(variable, op, value, currentValue);
            }
            else if (op == "=")
            {
                VariableStore.CreateVariable(variable, value);
            }
        }

        private void ProcessOperatorOnVariable(string variable, string op, object value, object currentValue)
        {
            switch (op)
            {
                case "=":
                    VariableStore.TrySetValue(variable, value);
                    break;
                case "+=":
                    VariableStore.TrySetValue(variable, ConcatenateOrAdd(value, currentValue));
                    break;
                case "-=":
                    VariableStore.TrySetValue(variable, Convert.ToDouble(currentValue) - Convert.ToDouble(value));
                    break;
                case "*=":
                    VariableStore.TrySetValue(variable, Convert.ToDouble(currentValue) * Convert.ToDouble(value));
                    break;
                case "/=":
                    VariableStore.TrySetValue(variable, Convert.ToDouble(currentValue) / Convert.ToDouble(value));
                    break;
                default:
                    Debug.LogError($"Invalid operator: {op}");
                    break;
            }
        }

        private object ConcatenateOrAdd(object value, object currentValue)
        {
            if(value is string)
            {
                return currentValue.ToString() + value;
            }

            return Convert.ToDouble(currentValue) + Convert.ToDouble(value);
        }

        public bool Matches(DIALOGUE_LINES line)
        {
            //make sure if has both a variable and an operator assignment
            Match match = Regex.Match(line.rawData.Trim(), REGEX_OPERATOR_LINE);

            return match.Success;
        }
    }
}