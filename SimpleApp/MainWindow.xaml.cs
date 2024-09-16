using SimpleAppEntityLibrary.DTOs;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;

namespace SimpleApp
{
    public partial class MainWindow : Window
    {
        private readonly HttpService _httpService;
        private int _selectedUserId = -1; // To store the selected UserId
        public ObservableCollection<UserDto> Users { get; set; } = new ObservableCollection<UserDto>();

        public MainWindow(HttpService httpService)
        {
            InitializeComponent();
            _httpService = httpService;

            // Set the DataContext for binding
            DataContext = this;

            // Load data from API when MainWindow is initialized
            LoadData();

        }

        // Load data from API
        private async void LoadData()
        {
            try
            {
                var users = await _httpService.GetAsync<List<UserDto>>("users");
                Users.Clear(); // Clear existing items
                foreach (var user in users)
                {
                    Users.Add(user); // Add users to the ObservableCollection
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška prilikom dohvata podataka: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Handle Create button click
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            UserRolesWindow userRolesWindow = new UserRolesWindow(_httpService, "Novi korisnik");
            userRolesWindow.ShowDialog();
            LoadData(); // Refresh user data after closing the window
        }

        // Handle Update button click
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedUserId != -1)
            {
                var selectedUser = dataGridUsers.SelectedItem as UserDto;
                if (selectedUser != null)
                {
                    UserRolesWindow userRolesWindow = new UserRolesWindow(_httpService, selectedUser.UserId, selectedUser.Username, selectedUser.UserRoleIds);
                    userRolesWindow.ShowDialog();
                    LoadData(); // Refresh user data after closing the window
                }
            }
            else
            {
                MessageBox.Show("Molimo odaberite korisnika za kojeg želite izmjeniti podatke.");
            }
        }

        // Handle Delete button click
        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedUserId != -1)
            {
                var result = MessageBox.Show($"Jeste li sigurni da želite obrisati korisnika s korisničkim id-em {_selectedUserId}?",
                    "Potvrda brisanja", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    bool isDeleted = await DeleteUserAsync(_selectedUserId);
                    if (isDeleted)
                    {
                        MessageBox.Show($"Korisnika s ID-em {_selectedUserId} obrisan.");
                        // Refresh the data grid
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Neuspješno brisanje korisnika.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Molimo odaberite korisnika za brisanje.");
            }
        }

        // Method to delete user
        private async Task<bool> DeleteUserAsync(int userId)
        {
            try
            {
                var request = new
                {
                    UserId = userId,
                    ModifiedBy = 1 // Hardcoded value
                };

                // Send DELETE request with payload
                //var response = await _httpService.DeleteAsync($"users", request);
                var response = await SoftDeleteUserAsync(request.UserId, request.ModifiedBy);

                // Check response (assuming successful if status code is 200)
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška prilikom brisanja korisnika: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public async Task<HttpResponseMessage> SoftDeleteUserAsync(int userId, int modifiedBy)
        {
            var requestUri = $"soft-delete/{userId}?modifiedByUserId={modifiedBy}";
            return await _httpService.PutAsync(requestUri, modifiedBy); // Assuming no body content for soft delete
        }

        // Handle DataGrid row selection
        private void dataGridUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridUsers.SelectedItem is UserDto selectedUser)
            {
                // Store the selected UserId for future operations (like Update or Delete)
                _selectedUserId = selectedUser.UserId;
            }
        }
    }
}