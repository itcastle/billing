using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;

namespace GestionCommerciale.DomainModel.Validator
{
	public class Validator
	{
	    private static readonly IDictionary<string, Regex> RegexDictionary = new Dictionary<string, Regex>() {
		{"FixPhone", new Regex("^(0)([0-9]{8})$")},
		{"MobPhone", new Regex("^(0)([5-7])([0-9]{8})$")},
		{"Email", new Regex(@"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
	 + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
	 + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
	 + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$")},
		{"IP", new Regex(@"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$")},  
							   
		{"IsAlphanumeric", new Regex("^[0-9a-zA-Z]+$")},
		{"IsAlphabetic", new Regex("^[a-zA-Z]+$")},
		{"Number", new Regex("^[+-]?([0-9]*[0-9][0-9]*(\\.[0-9]+)?|[0]+\\.[0-9]*[1-9][0-9]*)$")},   
		{"PositiveNumber", new Regex("^[+]?([0-9]*[0-9][0-9]*(\\.[0-9]+)?|[0]+\\.[0-9]*[1-9][0-9]*)$")},
		{"NegativeNumber", new Regex("^(-)([0-9]*[0-9][0-9]*(\\.[0-9]+)?|[0]+\\.[0-9]*[1-9][0-9]*)$")},
		{"URL", new Regex(@"^((www\.|(http|https|ftp)+\:\/\/)[&#95;.a-z0-9-]+\.[a-z0-9\/&#95;:@=.+?,##%&~-]*[^.|\'|\# |!|\(|?|,| |>|<|;|\)])")},
		{"PassWord", new Regex(@"^.*(?=.{7,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[\W_]).*$")},
		//Password must be  :
						//At least 7 chars
						//At least 1 uppercase char (A-Z)
						//At least 1 number (0-9)
						//At least one special char
		{"ZipCode", new Regex(@"^((([0-4])([0-8]))|(([0-3])([0-9])))[0-9]{3}$")},
		{"Login", new Regex(@"^([a-zA-Z_])([0-9a-zA-Z_]*)$")},
		{"Login2", new Regex(@"^[a-zA-Z][-\w.]{0,22}([a-zA-Z\d]|(?<![-.])_)$")}

		/// Login 2 determines whether the username meets conditions.
		/// Username conditions:
		/// Must be 1 to 24 character in length
		/// Must start with letter a-zA-Z
		/// May contain letters, numbers or '.','-' or '_'
		/// Must not end in '.','-','._' or '-_' 
		/// <returns>True if the username is valid</returns>


		};

		public static bool IsFixPhoneValid(string fixPhone)
		{
			return RegexDictionary["FixPhone"].IsMatch(fixPhone);
		}

		public static bool IsMobPhoneValid(string mobPhone)
		{
			return RegexDictionary["MobPhone"].IsMatch(mobPhone);
		}

		public static bool IsEmailValid(string email)
		{
			return RegexDictionary["Email"].IsMatch(email);
		}

	    private static bool IsIpValid(string ip)
		{
			return RegexDictionary["IP"].IsMatch(ip);
		}

		public static bool IsAlphanumericValid(string alphaNumeric)
		{
			return RegexDictionary["IsAlphanumeric"].IsMatch(alphaNumeric);
		}

		public static bool IsAlphabeticValid(string alphabetic)
		{
			return RegexDictionary["IsAlphabetic"].IsMatch(alphabetic);
		}

		public static bool IsNumberValid(String number)
		{
			return RegexDictionary["Number"].IsMatch(number);
		}

	    private static bool IsPositiveNumberValid(String posNumber)
		{
			return RegexDictionary["PositiveNumber"].IsMatch(posNumber);
		}

	    private static bool IsNegativeNumberValid(String negNumber)
		{

			return RegexDictionary["NegativeNumber"].IsMatch(negNumber);
		}

		public static bool IsUrlValid(string url)
		{
			return RegexDictionary["URL"].IsMatch(url);
		}

	    private static bool IsPassWordValid(string passWord)
		{
			return RegexDictionary["PassWord"].IsMatch(passWord);
		}

		public static bool IsPostalCodeValid(String pc)
		{
			return IsZipCodeValid(pc);
		}

	    private static bool IsLoginValid(String login)
		{
			return RegexDictionary["Login"].IsMatch(login);
		}

		public static bool IsLoginValid2(String login)
		{

			return RegexDictionary["Login2"].IsMatch(login);
		}

	    private static bool IsZipCodeValid(string zipCode)
		{
			return RegexDictionary["ZipCode"].IsMatch(zipCode);
		}


	    private static void SaySomthing(string text)
		{
			Console.WriteLine(text);
		}



		static void main(string[] args)
		{

			String ip = "0821555555";
			Console.WriteLine("IsFixPhoneValid {0}", IsFixPhoneValid(ip));
			Console.WriteLine("IsMobPhoneValid {0}", IsMobPhoneValid(ip));
			Console.WriteLine("IsEmailValid {0}", IsEmailValid(ip));
			Console.WriteLine("IsIPValid {0}", IsIpValid(ip));
			Console.WriteLine("IsAlphanumeriqueValid {0}", IsAlphanumericValid(ip));
			Console.WriteLine("IsAlphabetiqueValid {0}", IsAlphabeticValid(ip));
			Console.WriteLine("IsNumberValid {0}", IsNumberValid(ip));
			Console.WriteLine("IsPositiveNumberValid {0}", IsPositiveNumberValid(ip));
			Console.WriteLine("IsNegativeNumberValid {0}", IsNegativeNumberValid(ip));
			Console.WriteLine("IsURLValid {0}", IsUrlValid(ip));
			Console.WriteLine("IsPassWordValid {0}", IsPassWordValid(ip));
			Console.WriteLine("IsPostalCodeValid {0}", IsZipCodeValid(ip));
			Console.WriteLine("IsLoginValid {0}", IsLoginValid(ip));



			SaySomthing("Fin de Control Bonne Chance");
			Console.ReadLine();

		}

		public static byte[] ConvertImageToByteArray(string fileName)
		{
			Bitmap bitMap = new Bitmap(fileName);
			ImageFormat bmpFormat = bitMap.RawFormat;
			var imageToConvert = Image.FromFile(fileName);
			using (MemoryStream ms = new MemoryStream())
			{
				imageToConvert.Save(ms, bmpFormat);
				return ms.ToArray();
			}
		}

		public static Image ConvertByteArrayToImage(byte[] byteArray)
		{
			MemoryStream ms = new MemoryStream(byteArray);
			Image returnImage = Image.FromStream(ms);
			return returnImage;
		}
	}

}

