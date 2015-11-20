/*
-------------------------------------------------------------------------------------------------------------------------------------------------
---CompareSequence---
Given a ID from uniprot (and its sequence) and a ID from Ensembl (and its sequence) compare if both sequences are equal.
(It helps when trying to mapp an ID uniprot to ID Ensembl, when normally u get more than 1 ID_ensembl)
-------------------------------------------------------------------------------------------------------------------------------------------------

[Input Requirements]
Id_Uniprot
ID_ensembl
sequence_path

[Output Formats]
"True" if both sequence are equal
"False" if not

Author: Elena Alvarez de la Campa Crespo
Date: 2014-12-07
-------------------------------------------------------------------------------------------------------------------------------------------------
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace ExtractSequence
{
	public class CompareSequence
	{
//-------------------------------------------------------------------------------------------------------------------------------------------------
//Main
//-------------------------------------------------------------------------------------------------------------------------------------------------
		public static void Main(string[] args)
		{
			if (args.Length != 3)
			{
				//Check if user inputs are correct, if not exit.
				Console.WriteLine("Incorrect calling to program!");
				Console.WriteLine("CompareSequence.cs ID_uniprot ID_ensembl sequence_path");
				System.Environment.Exit(1);
			}
			else
			{
				//read variables as string
				string ID_uniprot = args[0];
				string ID_ensembl = args[1];
				string sequence_path = args[2];
				//execute function
				Comparation(ID_uniprot, ID_ensembl, sequence_path);			
			}
		}
//-------------------------------------------------------------------------------------------------------------------------------------------------
//Function
//-------------------------------------------------------------------------------------------------------------------------------------------------
		public static void Comparation(string ID_uniprot, string ID_ensembl, string sequence_path)
		{
			//open ID_uniprot fasta file and save sequence in a uniprot_seq string variable
			StreamReader readtext = new StreamReader(sequence_path + "/" + ID_uniprot + ".fa");
			string line;
			List<string> uniprot_seq_list_aux = new List<string>();
			while ((line = readtext.ReadLine()) != null)
			{
				string line_2 = line.Replace("\n", string.Empty);
				string check = line_2.Contains(">") ? "True" : "False";

				if (check == "False")
				{	
					uniprot_seq_list_aux.Add(line_2);
				}
			}
			readtext.Close();
			string uniprot_seq = string.Join("", uniprot_seq_list_aux.ToArray());
			Console.Write(uniprot_seq);
			Console.Write("\n\n");

			//open Ensembl fasta sequence and check if all seq lines are in uniprot_seq variable, check if both sequence are the same
			StreamReader read_ens = new StreamReader(sequence_path + "/" + ID_ensembl + ".fa");
			string line_ens;
			List<string> ens_seq_list_aux = new List<string>();
			while ((line_ens = read_ens.ReadLine()) != null)
			{
				string line_2_ens = line_ens.Replace("\n", string.Empty);
				string check = line_2_ens.Contains(">") ? "True" : "False";

				if (check == "False")
				{	
					ens_seq_list_aux.Add(line_2_ens);
				}
			}
			read_ens.Close();
			string ensembl_seq = string.Join("", ens_seq_list_aux.ToArray());
			Console.Write(ensembl_seq);

			Console.Write("\n\n");

			Console.WriteLine((ensembl_seq == uniprot_seq) ? "True" : "False");	
		}
	}
}
