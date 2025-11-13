using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TESTING
{
    public class VariableStoreTesting : MonoBehaviour
    {
        public int var_int = 0;

        // Start is called before the first frame update
        void Start()
        {
            VariableStore.CreateDatabase("DB_Numbers");
            VariableStore.CreateDatabase("DB2");
            VariableStore.CreateDatabase("DB3");


            //internal variable
            VariableStore.CreateVariable("DB_Numbers.num1", 1);
            VariableStore.CreateVariable("DB_Numbers.num5", 5);
            VariableStore.CreateVariable("lightIsOn", true);
            VariableStore.CreateVariable("float1", 7.5f);
            VariableStore.CreateVariable("str1", "hi");
            VariableStore.CreateVariable("str2", "hello");


            VariableStore.PrintAllVariables();

            VariableStore.PrintAllDatabases();


            VariableStore.CreateDatabase("DB_Links");
            VariableStore.CreateVariable("L_int", var_int, () => var_int, value=>var_int =  value);

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                string variable = "DB_Numbers.num1";
                VariableStore.TryGetValue(variable, out object v);
                VariableStore.TrySetValue(variable, (int)v + 5);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                VariableStore.TryGetValue("DB_Numbers.num1", out object num1);
                VariableStore.TryGetValue("DB_Numbers.num5", out object num2);

                Debug.Log($"num1 + num2 is {(int)num1 + (int)num2}");

            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                VariableStore.TryGetValue("L_int", out object linked_integer);
                VariableStore.TrySetValue("L_int", (int)linked_integer + 5);
            
                Debug.Log($"var_int is now {var_int}");

            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                VariableStore.RemoveVariable("DB_Links.L_int");
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                VariableStore.RemoveAllVariables();
            }
        }


    }
}