
using HotelManagementSystem;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Excel = Microsoft.Office.Interop.Excel;

namespace HotelManagementSystem.Pages
{
    public partial class Receptionist : Page
    {
        private readonly Helper _helper = new Helper();
        private List<ReservationModel> _allReservations;
        private List<ReservationModel> _filteredReservations;

        public Receptionist()
        {
            InitializeComponent();
            LoadReservations();
        }

        private void LoadReservations()
        {
            //Загрузка бронированных номеров
            try
            {
                var context = _helper.GetContext();

                _allReservations = (from r in context.Reservations
                                    join g in context.Guests on r.GuestID equals g.ID
                                    join rm in context.Rooms on r.RoomID equals rm.ID
                                    join rt in context.RoomTypes on rm.TypeID equals rt.ID
                                    join s in context.Statuses on r.StatusID equals s.ID
                                    select new ReservationModel
                                    {
                                        ID = r.ID,
                                        GuestID = (int)r.GuestID,
                                        GuestName = g.FirstName + " " + g.LastName,
                                        RoomID = (int)r.RoomID,
                                        RoomNumber = (int)rm.RoomNumber,
                                        RoomType = rt.Name,
                                        CheckInDate = (DateTime)r.CheckInDate,
                                        CheckOutDate = (DateTime)r.CheckOutDate,
                                        Status = s.Name,
                                        Price = (decimal)rm.Price
                                    }).ToList();

                _filteredReservations = new List<ReservationModel>(_allReservations);
                ReservationsDataGrid.ItemsSource = _filteredReservations;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке бронирований: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            //Метод для реализации поиска и фильтров по статусам 
            try
            {
                string searchText = SearchTextBox.Text.ToLower();
                string statusFilter = (FilterComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

                _filteredReservations = _allReservations.Where(r =>
                    (string.IsNullOrEmpty(searchText) ||
                     r.GuestName.ToLower().Contains(searchText) ||
                     r.RoomNumber.ToString().Contains(searchText) ||
                     r.Status.ToLower().Contains(searchText)) &&
                    (statusFilter == "Все статусы" || r.Status == statusFilter)
                ).ToList();

                if (_filteredReservations.Count == 0 && !string.IsNullOrEmpty(searchText))
                {
                    MessageBox.Show("Бронирований по вашему запросу не найдено.", "Информация",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }

                ReservationsDataGrid.ItemsSource = _filteredReservations;
            }
            catch (Exception e)
            {
                MessageBox.Show("Ошибка: " + e.Message);
            }

        }

        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            if (_filteredReservations == null) return;

            _filteredReservations = _filteredReservations
                .OrderBy(r => r.CheckInDate)
                .ToList();

            ReservationsDataGrid.ItemsSource = _filteredReservations;
        }

        private void NewReservationButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new NewReservation(_helper));
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
        }



        private void BackButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ExportButton_Click_1(object sender, RoutedEventArgs e)
        {

            //Экспорт данных о бронированных номерах в формате Excel при помощи библеотеки Interop.Excel
            var helper = new Helper();

            var context = helper.GetContext();

            var rooms = context.Rooms.ToList();

            var excelApp = new Excel.Application();

            excelApp.Visible = true;

            excelApp.Workbooks.Add();

            Excel._Worksheet worksheet = (Excel.Worksheet)excelApp.ActiveSheet;

            worksheet.Cells[1, "A"] = "ID";
            worksheet.Cells[1, "B"] = "RoomNumber";
            worksheet.Cells[1, "C"] = "TypeID";
            worksheet.Cells[1, "D"] = "Price";
            worksheet.Cells[1, "E"] = "Availability";

            var row = 1;

            foreach (var room in rooms)
            {
                row++;

                worksheet.Cells[row, "A"] = room.ID;
                worksheet.Cells[row, "B"] = room.RoomNumber;
                worksheet.Cells[row, "C"] = room.TypeID;
                worksheet.Cells[row, "D"] = room.Price;
                worksheet.Cells[row, "E"] = room.Availability;

            }
            worksheet.Columns[1].AutoFit();
            worksheet.Columns[2].AutoFit();
        }

        private void BackButton_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

        private void CheckOutButton_Click(object sender, RoutedEventArgs e)
        {
            if (ReservationsDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите бронирование для выселения",
                               "Предупреждение",
                               MessageBoxButton.OK,
                               MessageBoxImage.Warning);
                return;
            }

            var selectedReservation = (ReservationModel)ReservationsDataGrid.SelectedItem;

            try
            {
                var helper = new Helper();
                var context = helper.GetContext();

                var reservation = context.Reservations.FirstOrDefault(r => r.ID == selectedReservation.ID);
                if (reservation == null) return;

   
                var room = context.Rooms.FirstOrDefault(r => r.ID == reservation.RoomID);
                if (room == null) return;

                var completedStatus = context.Statuses.FirstOrDefault(s => s.Name == "Completed");
                if (completedStatus != null)
                {
                    reservation.StatusID = completedStatus.ID;
                }

                room.Availability = true;

                if (reservation.CheckOutDate > DateTime.Today)
                {
                    reservation.CheckOutDate = DateTime.Today;
                }

                context.SaveChanges();

                MessageBox.Show("Гость успешно выселен!", "Успех",
                               MessageBoxButton.OK,
                               MessageBoxImage.Information);
                LoadReservations();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выселении: {ex.Message}",
                               "Ошибка",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
            }
        }
    }


    public class ReservationModel
    {
        public int ID { get; set; }
        public int GuestID { get; set; }
        public string GuestName { get; set; }
        public int RoomID { get; set; }
        public int RoomNumber { get; set; }
        public string RoomType { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public string Status { get; set; }
        public decimal Price { get; set; }
    }
}