/*
-------------------------------------------------------------------------------------------------------------------------------------------------
---IDEnsemblandIDUniprotMapping---
-------------------------------------------------------------------------------------------------------------------------------------------------

[Input Requirements]
Id_Protein (Uniprot format or Ensembl format)

[Output Formats]
ID in Ensembl format or Uniprot format

Author: Elena Alvarez de la Campa Crespo
Date: 2014-10-07
-------------------------------------------------------------------------------------------------------------------------------------------------
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace Csharp
{
	public class IDEnsemblIDUniprotMapping
	{
//-------------------------------------------------------------------------------------------------------------------------------------------------
//Main
//-------------------------------------------------------------------------------------------------------------------------------------------------
		public static void Main(string[] args)
		{
			if (args.Length != 1)
			{   
				//Check if user inputs are correct, if not exit.
				Console.WriteLine("Incorrect calling to program!");
				Console.WriteLine("IDEnsemblIDUniprotMapping.cs ID_protein");
				System.Environment.Exit(1);
			}
			else
			{
				//read ID and check if it's an Ensembl ID (case 1) or a UniprotID(case 2)
				string ID_input = args[0];
				int caseSwitch = ID_input.Contains("ENSP") ? 1 : 2;

				string outpath_temp = "/home/ealvarez/elena/Projects/CRAG_predictions/Crohn/previous/score_calculation/temp";

				switch (caseSwitch)
				{
					case 1: //From Ensembl ID to UniprotID
						string result1 = ObtainUniprotIDFromEnsemblIDMapping(ID_input).Replace("\n", string.Empty);
						Console.WriteLine(result1);
						break;
					
					case 2: //From UniprotID to EnsemblID
						string result2 = ObtainEnsemblIDFromUniprotInfo(ID_input).Replace("\n", string.Empty); //solo un output
						//in case obtaining EnsemblID from Uniprot protein information is not working, obtain ID from mapping
						if (result2.Length == 0)
						{
							string result3 = ObtainEnsemblIDFromUniprotIDMapping(ID_input); //da mas de un output, usar en caso que la 1a opcion de NA
							string[] result3_list = result3.Split('\n');
						
							
							foreach (string ID in result3_list)
							{
								if (ID != "")
								{
									if (result3_list.Length == 1)
									{
										Console.WriteLine(ID);
									}
									else
									{
										ExtractSequence_uniprot(ID_input, outpath_temp);
										ExtractSequence_ensembl(ID, outpath_temp);
										string is_same = Comparation(ID_input, ID, outpath_temp);
										File.Delete(outpath_temp + "/" + ID + ".fa");
										File.Delete(outpath_temp + "/" + ID_input + ".fa");

										if (is_same == "True")
										{
											Console.WriteLine(ID);
											break;
										}
									}
										 //if there is more than 1 EnsmblID, download sequence and compare them!!!!
								}
							}
						}
						else
						{
							Console.WriteLine(result2);
						}
						break;
					
					default:
						Console.WriteLine("Default Case");
						break;
				}
			}
		}
//-------------------------------------------------------------------------------------------------------------------------------------------------
//Functions
//-------------------------------------------------------------------------------------------------------------------------------------------------
		public static string ObtainEnsemblIDFromUniprotInfo (string ID_uniprot)
		{
			string server = "http://www.uniprot.org";
			string ext = "/uniprot/" + ID_uniprot + ".txt";

			HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(string.Format("{0}{1}", server, ext));
			WebReq.Method = "GET";
			//Get the response
			HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
			//Some information about the response
			///Console.WriteLine(WebResp.StatusCode);
			///Console.WriteLine(WebResp.Server);
			//Read the response (the string), and output it.
			Stream Answer = WebResp.GetResponseStream();
			StreamReader _Answer = new StreamReader(Answer);
			string line;
			string find_line = "";
			while ((line = _Answer.ReadLine()) != null)
			{
				string line_aux = line.Replace("\n", string.Empty);
				string check = line_aux.Contains("DR   STRING") ? "True" : "False";

				if (check == "True")
				{
					find_line = line_aux.Replace(" ", string.Empty);
					break;
				}
			}
			_Answer.Close();
			string[] split_find_line = find_line.Split(';');
			string ID_Ensemble = split_find_line[1].Split('.')[1];
			//Console.WriteLine(ID_Ensemble);
			return ID_Ensemble;
		}


		public static string ObtainEnsemblIDFromUniprotIDMapping (string ID_uniprot)
		{
			string server = "http://www.uniprot.org/mapping/";
			WebClient webClient = new WebClient();
			webClient.QueryString.Add("from", "ACC+ID");
			webClient.QueryString.Add("to", "ENSEMBL_PRO_ID");
			webClient.QueryString.Add("format", "list");
			webClient.QueryString.Add("query", ID_uniprot);
			string result = webClient.DownloadString(server);
			//Console.WriteLine(result);
			return result;
		}

		
		public static string ObtainUniprotIDFromEnsemblIDMapping (string ID_Ensembl)
		{
			string server = "http://www.uniprot.org/mapping/";
			WebClient webClient_1 = new WebClient();
			webClient_1.QueryString.Add("from", "ENSEMBL_PRO_ID");
			webClient_1.QueryString.Add("to", "ACC");
			webClient_1.QueryString.Add("format", "list");
			webClient_1.QueryString.Add("query", ID_Ensembl);
			string result = webClient_1.DownloadString(server);
			//Console.WriteLine(result);
			return result;
		}

		public static void ExtractSequence_uniprot(string ID_uniprot, string outpath)
		{
			string server = "http://www.uniprot.org";
			string ext = "/uniprot/" + ID_uniprot + ".fasta";

			HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(string.Format("{0}{1}", server, ext));
			WebReq.Method = "GET";

			//Get the response
			HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
			//Some information about the response
			///Console.WriteLine(WebResp.StatusCode);
			///Console.WriteLine(WebResp.Server);

			//Read the response (the string), and output it.
			Stream Answer = WebResp.GetResponseStream();
			StreamReader _Answer = new StreamReader(Answer);
			//Console.WriteLine(_Answer.ReadToEnd());

			//write it in a fasta file
			StreamWriter writetext = new StreamWriter(outpath+"/"+ID_uniprot+".fa");
			writetext.Write(_Answer.ReadToEnd());
			writetext.Close();
		}

		public static void ExtractSequence_ensembl(string ID_ensembl, string outpath)
		{
			string server = "http://rest.ensembl.org";
			string ext = "/sequence/id/" + ID_ensembl;

			HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(string.Format("{0}{1}", server, ext));
			WebReq.Method = "GET";
			WebReq.ContentType ="text/x-fasta";

			//Get the response
			HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
			//Some information about the response
			///Console.WriteLine(WebResp.StatusCode);
			///Console.WriteLine(WebResp.Server);

			//Read the response (the string), and output it.
			Stream Answer = WebResp.GetResponseStream();
			StreamReader _Answer = new StreamReader(Answer);
			//Console.WriteLine(_Answer.ReadToEnd());

			//write it in a fasta file
			StreamWriter writetext = new StreamWriter(outpath+"/"+ID_ensembl+".fa");
			writetext.Write(_Answer.ReadToEnd());
			writetext.Close();
		}

		public static string Comparation(string ID_uniprot, string ID_ensembl, string sequence_path)
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
			//Console.Write(uniprot_seq);
			//Console.Write("\n\n");

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
			//Console.Write(ensembl_seq);

			//Console.Write("\n\n");

			return (ensembl_seq == uniprot_seq) ? "True" : "False";
		}
	}
}
