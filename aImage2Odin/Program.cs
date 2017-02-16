/*
 * Created by SharpDevelop.
 * User: aZuZu
 * Date: 30.11.2015.
 * Time: 21:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using ICSharpCode.SharpZipLib.Tar;

namespace aImage2Odin
{
	class Program
	{

		 public static int Write_Padding(uint Item_Size)
	     {
	        int count;
	        if ((Item_Size & (2048 - 1)) == 0)
	        {
	          	return 0;
	        }
	        count = (int)(2048 - (Item_Size & (2048 - 1)));
	        return count;
		 }		
       public static int ByteIndexOf(byte[] Where, byte[] What, int Start)
       {
			bool matched = false;
            for (int index = Start; index <= Where.Length - What.Length; ++index)
            {
             	matched = true;
           		for (int subIndex = 0; subIndex < What.Length; ++subIndex)
                {
                	if (What[subIndex] != Where[index + subIndex])
                 	{
                  		matched = false;
                  		break;
               		}
                }
                if (matched)
                {
                   	return index;
                }
         	}
           	return -1;
        }		
       public static void Make_TAR(string What, string Where)
	   {
			string Out_TAR = Path.GetFileNameWithoutExtension(Where) + ".tar";
			string Out_MD5 = Path.GetFileNameWithoutExtension(Where) + ".tar.md5";
	       	Stream fs = new FileStream(Out_MD5, FileMode.Create);
       		StringBuilder sb = new StringBuilder();
			MD5 MD5_Hash_Engine = new MD5CryptoServiceProvider();
			Console.WriteLine("Creating: " + Out_MD5 + " .....");
		   	TarArchive TA = TarArchive.CreateOutputTarArchive(fs);
		   	TarEntry TE = TarEntry.CreateEntryFromFile(What);
		   	TE.SetIds(0,0);
		   	TE.SetNames(string.Empty, string.Empty);
		   	TE.TarHeader.Mode = 420;
			TA.WriteEntry(TE,true);
			TA.Close();
			fs.Close();
       		Stream hash = new FileStream(Out_MD5, FileMode.Open);
			byte[] TAR_MD5_Hash = MD5_Hash_Engine.ComputeHash(hash);       		
			for (int i = 0; i < TAR_MD5_Hash.Length; i++)
			{
				sb.Append(TAR_MD5_Hash[i].ToString("X2"));
			}			
			byte[] Final_Hash = Encoding.ASCII.GetBytes(sb.ToString().ToLower() + Convert.ToChar(0x20).ToString() + Convert.ToChar(0x20).ToString() + Out_TAR + Convert.ToChar("\n").ToString());
			Console.WriteLine("MD5 for " + Out_MD5 + " is " + sb.ToString().ToLower());
			hash.Write(Final_Hash, 0, Final_Hash.Length);
			hash.Close();
	   }
       public static string Filler(string Data)
       {
       		char[] Fill = new char[10240 - Data.Length];
       		for ( int a = 0; a < Fill.Length; a++ )
       		{
       			Fill[a] = (char)0x0;
       		}
       		string outx = new string(Fill);
       		return outx + "  " + Data;
       }
       
	   public static void Main(string[] args)
	   {
			if ( args.Length < 2 )
			{
				Console.WriteLine("aImage2Odin, v1.02.3. (c) aZuZu. 2015");
				Console.WriteLine(Environment.NewLine);
				Console.WriteLine("usage: [Recovery Image] [Project Name]");
			} else {
				if ( File.Exists(args[0]) )
				{
					try { 
							Console.WriteLine("aImage2Odin, v1.02.3. (c) aZuZu. 2015");
							Console.WriteLine(Environment.NewLine);
							Make_TAR(args[0], args[1]);
					} catch ( Exception e ) {
						Console.WriteLine(e.Message.ToString());
					}
				} else {
					Console.WriteLine("File " + args[0] + " not found.");
					Environment.Exit(-1);
				}
			}

		}
	}
}