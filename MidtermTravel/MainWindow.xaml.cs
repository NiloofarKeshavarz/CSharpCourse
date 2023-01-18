using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MidtermTravel
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		List<Trip> trips= new List<Trip>(); //we collect created trip in it
		public MainWindow()
		{
			InitializeComponent();

			//connect to database
			
			try
			{
				Globals.dbContext = new TripDbContext(); //throws an Ex, Not an ok one
				LvTrips.ItemsSource = Globals.dbContext.Trips.ToList();

			}
			catch (SystemException ex)
			{
				MessageBox.Show(this, "Connection failed" + ex.Message, "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
				Environment.Exit(1);
			}
		}

		private void BtnAdd_Click(object sender, RoutedEventArgs e)
		{
			//check if dates are null
			try 
			{
				if (DpDeparture.SelectedDate == null || DpReturn.SelectedDate == null)
				{
					throw new ArgumentException("Please select a date.");
				}
				string destination = TxbDestination.Text;
				string name = TbxName.Text;
				string passportNumber = TbxPassort.Text;
				DateTime departure = (DateTime)DpDeparture.SelectedDate;
				DateTime returndate = (DateTime)DpReturn.SelectedDate;

				Trip newTrip = new Trip(destination, name, passportNumber, departure, returndate);
				Globals.dbContext.Trips.Add(newTrip);
				Globals.dbContext.SaveChanges();
				LvTrips.ItemsSource = Globals.dbContext.Trips.ToList();
				ResetFields();
			}
			catch (ArgumentException ex)
			{
				MessageBox.Show(this, ex.Message, "InputError", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			catch(SystemException ex)
			{
				MessageBox.Show(this, "Error reading from database\n" + ex.Message, "Databae Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}

		}
		private void ResetFields()
		{
			TxbDestination.Text = "";
			TbxName.Text = "";
			TbxPassort.Text = "";
			DpDeparture.SelectedDate = DateTime.Today;
			DpReturn.SelectedDate = DateTime.Today;
			BtnDelete.IsEnabled = true;
			BtnUpdate.IsEnabled = true;
		}

		private void LvTrips_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Trip selectedTrip = LvTrips.SelectedItem as Trip; //Cast the obj to a Trip Type
			if(selectedTrip == null ) 
			{
				ResetFields();
			}
			else
			{
				TxbDestination.Text = selectedTrip.Destination;
				TbxName.Text = selectedTrip.Name;
				TbxPassort.Text = selectedTrip.PassportNumber;
				DpDeparture.SelectedDate = selectedTrip.Departure;
				DpReturn.SelectedDate= selectedTrip.Return;

			}
		}

		private void BtnUpdate_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Trip selectedTrip = LvTrips.SelectedItem as Trip;
				if (selectedTrip != null)
				{
					string UpdatingDestination = TxbDestination.Text;
					string updatingName = TbxName.Text;
					string updatingPassportNumber = TbxPassort.Text;
					DateTime updatingDeparture = (DateTime)DpDeparture.SelectedDate;
					DateTime updatingReturnDate = (DateTime)DpReturn.SelectedDate;

					//FIXME:no need to create another db? Check this again
					using (var db = new TripDbContext()) 
						
					{
						var trip = db.Trips.Find(selectedTrip.Id);
						trip.Destination = UpdatingDestination;
						trip.Name = updatingName;
						trip.PassportNumber = updatingPassportNumber;
						trip.Departure = updatingDeparture;
						trip.Return = updatingReturnDate;
						db.SaveChanges();
					}
					selectedTrip.Destination = UpdatingDestination;
					selectedTrip.Name = updatingName;
					selectedTrip.PassportNumber = updatingPassportNumber;
					selectedTrip.Departure = updatingDeparture;
					selectedTrip.Return = updatingReturnDate;
					LvTrips.ItemsSource = Globals.dbContext.Trips.ToList();
					LvTrips.Items.Refresh();
				}
			}
			catch(SystemException ex)
			{
				MessageBox.Show(this, "Error updating from database\n" + ex.Message, "Databae Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void BtnDelete_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Trip selectedTrip = LvTrips.SelectedItem as Trip;
				if (selectedTrip != null)
				{
					Globals.dbContext.Trips.Remove(Globals.dbContext.Trips.Find(selectedTrip.Id));
					Globals.dbContext.SaveChanges();
				}
				trips.Remove(selectedTrip);
				LvTrips.ItemsSource = Globals.dbContext.Trips.ToList();
				LvTrips.Items.Refresh();
			}
			catch(SystemException ex)
			{
				MessageBox.Show(this, "Error deleting from database\n" + ex.Message, "Databae Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}

		}

		private void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				// prompts the user for a location to save the file
				SaveFileDialog saveFile = new SaveFileDialog();

				//file format
				saveFile.Filter = "Text Files (*.txt) | *.txt";
				
				saveFile.FileName = "trips.txt";

				if (saveFile.ShowDialog() == true)
				{
					using (StreamWriter sr = new StreamWriter(saveFile.OpenFile()))
					{
						sr.WriteLine("Destination","Name", "Passport Number", "Departure", "Return");
						//FIXME
						//foreach(Trip trip in LvTrips.SelectedItems)
						foreach (Trip trip in Globals.dbContext.Trips.ToList())
						{
							sr.WriteLine($"{trip.Destination}; {trip.Name};{trip.PassportNumber}; {trip.Departure};{trip.Return}");
						}
					}
				}
			}
			catch (SystemException ex)
			{
				MessageBox.Show(this, "Error Exporting file\n" + ex.Message, "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}
