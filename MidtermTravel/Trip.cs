using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MidtermTravel
{
	public class Trip
	{
		//Default Cons- need it for sure for EF
		public Trip() { }

		//Constructor with param
		public Trip(string destination, string name, string passportNumber, DateTime departure, DateTime returnDate)
		{
			Destination = destination;
			Name = name;
			PassportNumber = passportNumber;
			Departure = departure;
			Return = returnDate;

		}

		[Key]
		public int Id { get; set; } // key must be public

		private string _destination;


		[Required]
		[StringLength(30)]
		public string Destination
		{
			get
			{
				return _destination;
			}
			set
			{
				//validation 
				if (value.Length < 2 || value.Length > 30)
				{
					throw new ArgumentException("Destination should be between 2-30 characters");
				}
				_destination = value;

			}

		}
		private string _name;

		[Required]
		[StringLength(30)]
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				
				if (value.Length < 2 || value.Length > 30)
				{
					throw new ArgumentException("Name should be between 2-30 characters");
				}
				_name = value;

			}
		}
		private string _passportNumber;

		[Required]
		public string PassportNumber
		{
			get
			{
				return _passportNumber;
			}
			set
			{
				//TODO: check the regex again
				//Regex regex = new Regex("^[A-Z]{2}[0-9]{6}$");
				if (!Regex.IsMatch(value, @"^[A-Z]{2}[0-9]{6}$"))
				{
					throw new ArgumentException("Passport Number should be in the right format: ex: AA123456");
				}
				_passportNumber = value;

			}
		}

		private DateTime _departure;

		[Required]
		[DataType(DataType.Date)]
		public DateTime Departure
		{
			get
			{
				return _departure;
			}
			set
			{
				_departure = value;

			}
		}

		private DateTime _return;

		[Required]
		[DataType(DataType.Date)]
		//[Compare("Departure", ErrorMessage = "Return date must be after the departure date!")]
		public DateTime Return
		{
			get
			{
				return _return;
			}
			set
			{
				_return = value;

			}
		}

		public override string ToString()
		{
			return $"{Destination};{Name};{PassportNumber};{Departure};{Return}";
		}

	}
}
