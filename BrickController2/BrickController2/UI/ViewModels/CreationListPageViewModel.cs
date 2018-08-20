﻿using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using BrickController2.CreationManagement;
using BrickController2.DeviceManagement;
using BrickController2.UI.Navigation;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Forms;

namespace BrickController2.UI.ViewModels
{
    public class CreationListPageViewModel : PageViewModelBase
    {
        private readonly ICreationManager _creationManager;
        private readonly IDeviceManager _deviceManager;

        public CreationListPageViewModel(
            INavigationService navigationService,
            ICreationManager creationManager,
            IDeviceManager deviceManager)
            : base(navigationService)
        {
            _creationManager = creationManager;
            _deviceManager = deviceManager;

            AddCreationCommand = new Command(async () =>
            {
                await DisplayAlertAsync("...", "Add creation", "Cancel");
            });

            MenuCommand = new Command(async () =>
            {
                var result = await DisplayActionSheetAsync("Select option", "Cancel", null, "Devices", "About");
                switch (result)
                {
                    case "Devices":
                        await NavigationService.NavigateToAsync<DeviceListPageViewModel>();
                        break;

                    case "About":
                        await DisplayAlertAsync(null, "About selected", "Ok");
                        break;
                }
            });
        }

        public ObservableCollection<Creation> Creations => _creationManager.Creations;

        public ICommand AddCreationCommand { get; }
        public ICommand MenuCommand { get; }

        public override async void OnAppearing()
        {
            base.OnAppearing();

            await RequestPermissions();

            // TODO: show progress
            await _creationManager.LoadCreationsAsync();
            await _deviceManager.LoadDevicesAsync();
        }

        private async Task RequestPermissions()
        {
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
            if (status != PermissionStatus.Granted)
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                {
                    await DisplayAlertAsync("Permission request", "Location permission is needed for accessing bluetooth", "Ok");
                }

                var result = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                if (result.ContainsKey(Permission.Location))
                {
                    status = result[Permission.Location];
                }
            }

            if (status != PermissionStatus.Granted)
            {
                await DisplayAlertAsync("Warning", "Bluetooth devices will NOT be available.", "Ok");
            }
        }
    }
}