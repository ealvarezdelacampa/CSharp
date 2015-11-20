/*
-------------------------------------------------------------------------------------------------------------------------------------------------
---ExtractSequenceFromUniprot---
-------------------------------------------------------------------------------------------------------------------------------------------------

[Input Requirements]
Id_Uniprot
outpath

[Output Formats]
sequence in fasta file

Author: Elena Alvarez de la Campa Crespo
Date: 2014-12-01
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
	public class ExtractSequenceFromUniProt
	{
//-------------------------------------------------------------------------------------------------------------------------------------------------
//Main
//-------------------------------------------------------------------------------------------------------------------------------------------------
		public static void Main(string[] args)
		{
			if (args.Length != 2)
			{
				//Check if user inputs are correct, if not exit.
				Console.WriteLine("Incorrect calling to program!");
				Console.WriteLine("ExtractSequenceFromEnsembl.cs ID_uniprot outpath");
				System.Environment.Exit(1);
			}
			else
			{
				//read variables as string
				string ID_uniprot = args[0];
				string outpath = args[1];
				//execute function
				ExtractSequence(ID_uniprot, outpath);			
			}
		}
//-------------------------------------------------------------------------------------------------------------------------------------------------
//Function
//-------------------------------------------------------------------------------------------------------------------------------------------------
		public static void ExtractSequence(string ID_uniprot, string outpath)
		{
			string server = "http://www.uniprot.org";
			string ext = "/uniprot/" + ID_uniprot + ".fasta";

			HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(string.Format("{0}{1}", server, ext));
			WebReq.Method = "GET";

			//Get the response
			HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
			//Some information about the response
			Console.WriteLine(WebResp.StatusCode);
			Console.WriteLine(WebResp.Server);

			//Read the response (the string), and output it.
			Stream Answer = WebResp.GetResponseStream();
			StreamReader _Answer = new StreamReader(Answer);
			//Console.WriteLine(_Answer.ReadToEnd());

			//write it in a fasta file
			StreamWriter writetext = new StreamWriter(outpath+"/"+ID_uniprot+".fa");
			writetext.Write(_Answer.ReadToEnd());
			writetext.Close();
		}
	}
}
