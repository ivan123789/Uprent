﻿using SimpleAppEntityLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace SimpleApp
{
    public partial class UserRolesWindow : Window
    {
        private readonly HttpService _httpService;
        private int? _userId;
        private List<int>? _userRoleIds = new List<int>();
        private List<RoleDto> roles;
        private bool _isSaveAndExitClicked = false;
        public ObservableCollection<Group> Groups { get; set; } = new ObservableCollection<Group>();

        // Model for Group
        public class Group
        {
            public string GroupName { get; set; }
            public bool IsAdded { get; set; }
        }

        // Constructor for create
        public UserRolesWindow(HttpService httpService, string username)
        {
            InitializeComponent();
            _httpService = httpService;
            txtUsername.Text = username;
            DataContext = this;

            LoadGroups(); // Load group data
        }

        // Constructor for update (with user ID)
        public UserRolesWindow(HttpService httpService, int userId, string username, List<int> userRoleIds) : this(httpService, username)
        {
            _userId = userId;
            _userRoleIds = userRoleIds;
            LoadUserRoles(); // Load user-specific data
        }

        // Load the groups into the DataGrid
        private async void LoadGroups()
        {
            try
            {
                roles = await _httpService.GetAsync<List<RoleDto>>("roles");

                Groups.Clear();
                foreach (var role in roles)
                {
                    Groups.Add(new Group
                    {
                        GroupName = role.RoleName,
                        IsAdded = _userRoleIds.Contains(role.RoleId)
                    });
                }

                dataGridGroups.ItemsSource = Groups;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška prilikom dohvata grupa: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Load user-specific roles (if needed)
        private async void LoadUserRoles()
        {
            // Logic to load and display user-specific data
            // This method can be used if you need to show user-specific data or modify it
        }

        // Exit button logic
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Save button logic
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveUserData();
            MessageBox.Show("Podaci spremljeni.");
        }

        // Save and Exit button logic
        private void btnSaveExit_Click(object sender, RoutedEventArgs e)
        {
            SaveUserData();
            MessageBox.Show("Podaci spremljeni.");
            _isSaveAndExitClicked = true; // Set flag
            this.Close();
        }

        private async void SaveUserData()
        {
            try
            {
                var username = txtUsername.Text;
                var userId = _userId ?? 0; // If _userId is null, default to 0
                var userRolesToSave = new List<UserRoleSaveRequestDto>(); // List to store DTOs

                // Loop through all groups (both selected and deselected)
                foreach (var group in Groups)
                {
                    var role = roles.FirstOrDefault(r => r.RoleName == group.GroupName);
                    if (role != null)
                    {
                        // Create a DTO for each group
                        var userRoleSaveRequestDto = new UserRoleSaveRequestDto
                        {
                            RoleId = role.RoleId,
                            UserId = userId,
                            Username = username,
                            CreatedByUserId = 1, // Hardcoded value
                            ModifiedByUserId = 1, // Hardcoded value
                            Visible = group.IsAdded // true if selected, false if deselected
                        };

                        // Add to the list of roles to save
                        userRolesToSave.Add(userRoleSaveRequestDto);
                    }
                }

                // Make a single POST request with the list of roles
                var response = await _httpService.PostAsync("userRoles", userRolesToSave);

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Neuspjeh u spremanju podataka o korisniku: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Neuspjeh u spremanju podataka: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_isSaveAndExitClicked)
            {
                // Show a confirmation message box when the user tries to close the window
                var result = MessageBox.Show("Jeste li sigurni da želite izaći? Sve eventualne izmjene će biti izgubljene.",
                                             "Potvrda zatvaranja prozora",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Warning);

                // If the user selects "No", cancel the closing event
                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }

            // Reset the flag
            _isSaveAndExitClicked = false;
        }


    }
}