namespace ActionCat.Experimental {
    using UnityEngine;

    public static class ACExtensionMethod {

    }

    public class Utility {
        public static bool IsNumeric(string str) {
            float output;
            return float.TryParse(str, out output);
        }
    }

    public static class ExtensionMethodExam {
        public static bool IsNumeric(this string str) {
            return float.TryParse(str, out float temp);
        }

        public static int DoublePlus(this int origin, int i, int j) {
            return origin + i + j;
        }
    }

    public class Example {

        public void OnExample() {
            string UtilityTest = "4";
            if(Utility.IsNumeric(UtilityTest) == true) {
                CatLog.Log("Is Numeric : True");
            }
            else {
                CatLog.Log("Is Numeric : False");
            }
        }

        public void OnExampleIsNumeric() {
            string tempString = "3";
            if(tempString.IsNumeric() == true){
                CatLog.Log("Is Numeric : True");
            }
            else {
                CatLog.Log("Is Numeric : False");
            }

            int tempInt = 10;
            CatLog.Log($"Result : {tempInt.DoublePlus(5, 3)}");
            //Result : 18
        }

        public void AddListerToAction(System.Action action) {
            action += OnExampleIsNumeric; //is doesn't work !
        }
    }

}
