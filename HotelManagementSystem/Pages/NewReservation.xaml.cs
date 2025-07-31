using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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

namespace HotelManagementSystem.Pages
{
    /// <summary>
    /// Логика взаимодействия для NewReservation.xaml
    /// </summary>
    public partial class NewReservation : Page
    {
        private readonly Helper _helper;
        public event EventHandler ReservationCreated;
        public NewReservation(Helper helper)
        {
            InitializeComponent();
            _helper = helper;
            LoadData();
        }
        private void LoadData()
        {
            //Загрузка данных из базы даннных для комбо боксов со списком гостей и списком свободных номеров
            try
            {
                var context = _helper.GetContext();

                CheckInDatePicker.SelectedDate = DateTime.Today;
                //Создаем список из клиентов для бронирования
                var guests = context.Guests.ToList();
                GuestComboBox.ItemsSource = guests.Select(g => new
                {
                    g.ID,
                    FullName = $"{g.FirstName} {g.LastName}"
                });
                GuestComboBox.DisplayMemberPath = "FullName";
                GuestComboBox.SelectedValuePath = "ID";
                //Проверяем, что комната для снятия обяхательно путсая, остльное не меняется 
                var rooms = context.Rooms
                    .Where(r => r.Availability == true)
                    .Join(context.RoomTypes,
                        r => r.TypeID,
                        rt => rt.ID,
                        (r, rt) => new RoomModel
                        {
                            ID = r.ID,
                            RoomNumber = (int)r.RoomNumber,
                            Type = rt.Name,
                            Price = (decimal)r.Price,
                            IsAvailable = (bool)r.Availability
                        }).ToList();

                RoomComboBox.ItemsSource = rooms.Select(r => new
                {
                    r.ID,
                    RoomInfo = $"Номер {r.RoomNumber} - {r.Type} ({r.Price} руб/ночь)"
                });
                RoomComboBox.DisplayMemberPath = "RoomInfo";
                RoomComboBox.SelectedValuePath = "ID";
                UpdateCheckOutDate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DaysTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateCheckOutDate();
        }

        private void UpdateCheckOutDate()
        {
            //В зависимости от количества выбранных дней обновление даты выселения, исходя из текущей даты
            if (CheckInDatePicker.SelectedDate.HasValue && int.TryParse(DaysTextBox.Text, out int days) && days > 0)
            {
                CheckOutDatePicker.SelectedDate = CheckInDatePicker.SelectedDate.Value.AddDays(days);
            }
        }

        private void RoomComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RoomComboBox.SelectedItem != null && RoomComboBox.SelectedValue is int roomId)
            {
                var rooms = (RoomComboBox.ItemsSource as IEnumerable<dynamic>)
                    .Select(item => new { ID = (int)item.ID, RoomInfo = (string)item.RoomInfo })
                    .ToList();

                var selectedRoomInfo = rooms.FirstOrDefault(r => r.ID == roomId)?.RoomInfo;

                if (selectedRoomInfo != null)
                {
                    RoomInfoText.Text = selectedRoomInfo;
                }
            }
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (GuestComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите гостя.", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (RoomComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите номер.", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(DaysTextBox.Text, out int days) || days <= 0)
            {
                MessageBox.Show("Пожалуйста, введите корректное количество дней.", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var context = _helper.GetContext();

                int guestId = (int)GuestComboBox.SelectedValue;
                int roomId = (int)RoomComboBox.SelectedValue;
                DateTime checkInDate = CheckInDatePicker.SelectedDate.Value;
                DateTime checkOutDate = CheckOutDatePicker.SelectedDate.Value;
                int bookedStatusId = context.Statuses.First(s => s.Name == "Booked").ID;

             
                var newReservation = new Reservations
                {
                    GuestID = guestId,
                    RoomID = roomId,
                    CheckInDate = checkInDate,
                    CheckOutDate = checkOutDate,
                    StatusID = bookedStatusId
                };

                var room = context.Rooms.First(r => r.ID == roomId);
                room.Availability = false;

                context.Reservations.Add(newReservation);
                context.SaveChanges();

                MessageBox.Show("Бронирование успешно создано!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                ReservationCreated?.Invoke(this, EventArgs.Empty);
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании бронирования: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }

    public class RoomModel
    {
        public int ID { get; set; }
        public int RoomNumber { get; set; }
        public string Type { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
    }
}
