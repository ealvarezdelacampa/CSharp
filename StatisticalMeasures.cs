/*
-------------------------------------------------------------------------------------------------------------------------------------------------
---StatisticalMeasures---

From TP (True Positive), FP (False Positive), TN (True Negative), FN (False Negative) compute the values
of Sensitivity, Specificity, Accuracy and Mathew's Correlation Coeficient.
-------------------------------------------------------------------------------------------------------------------------------------------------

[Input Requirements]
TP, FP, TN, FN values

[Output Formats]
Accuracy = 0,667
Mathews = 0,316
Sensibility = 0,5
Specificity = 0,8

[Execution Format]
StatisticalMeasures.cs TP FP TN FN

Author: Elena Alvarez de la Campa Crespo
Date: 2014-07-28
-------------------------------------------------------------------------------------------------------------------------------------------------
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Performance_Calcules
{
	public class StatisticalMeasures
	{
//-------------------------------------------------------------------------------------------------------------------------------------------------
//Main
//-------------------------------------------------------------------------------------------------------------------------------------------------
		public static void Main(string[] args)
		{
			if (args.Length != 4)
			{
				//Check if user inputs are correct, if not exit.
				Console.WriteLine("Incorrect calling to program!");
				Console.WriteLine("statistical_measures.cs TP FP TN FN");
				System.Environment.Exit(1);
			}
			else
			{
				//read as string, need for int conversion
				int TP = Convert.ToInt16(args[0]);
				int FP = Convert.ToInt16(args[1]);
				int TN = Convert.ToInt16(args[2]);
				int FN = Convert.ToInt16(args[3]);
				
				//function calls
				float acc_aux = accuracy(TP, FP, TN, FN);
				float mcc_aux = mathews(TP,FP,TN,FN);
				float sens_aux = sensitivity(TP, FN);
				float spec_aux = specificity(TN, FP);

				//print
				Console.WriteLine("Accuracy = {0}", Math.Round(acc_aux, 3));
				Console.WriteLine("Mathews = {0}", Math.Round(mcc_aux, 3));
				Console.WriteLine("Sensibility = {0}", Math.Round(sens_aux, 3));
				Console.WriteLine("Specificity = {0}", Math.Round(spec_aux, 3));
			}
			
		}
//-------------------------------------------------------------------------------------------------------------------------------------------------
//Functions
//-------------------------------------------------------------------------------------------------------------------------------------------------

		public static float accuracy(int TP, int FP, int TN, int FN)
		{
			float acc = (float)(TP + TN)/(float)(TP + FP + TN + FN);
			return acc;
		}
		public static float mathews(int TP, int FP, int TN, int FN)
		{
			float num = (TP * TN) - (FP * FN);
			float den = (TP + FP) * (TP + FN) * (TN + FP) * (TN + FN);
			float mcc = num/(float)Math.Sqrt(den);
			return mcc;
		}
		public static float sensitivity(int TP, int FN)
		{
			float sens = (float)TP/(float)(TP+FN);
			return sens;
		}
		public static float specificity(int TN, int FN)
		{
			float spec = (float)TN/(float)(TN+FN);
			return spec;
		}
	}
	
}
