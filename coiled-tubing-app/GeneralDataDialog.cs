using coiled_tubing_app.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace coiled_tubing_app
{
    public sealed class GeneralDataDialog : ContentDialog
    {
        private readonly TextBox _customerTextBox;
        private readonly TextBox _leaseTextBox;
        private readonly TextBox _wellNumberTextBox;
        private readonly TextBox _apiNumberTextBox;
        private readonly TextBox _zoneTextBox;
        private readonly TextBox _locationTextBox;
        private readonly TextBox _countryTextBox;
        private readonly TextBox _stateTextBox;
        private readonly TextBox _customerRepresentativeTextBox;
        private readonly TextBox _dcsgLegalDescriptionTextBox;
        private readonly TextBox _serviceDateTextBox;
        private readonly TextBox _serviceTypeTextBox;
        private readonly TextBox _serviceDistrictBoatTextBox;
        private readonly TextBox _bjRepresentativeTextBox;
        private readonly TextBox _jobNumberTextBox;

        public GeneralData GeneralData { get; private set; } = new GeneralData();

        public GeneralDataDialog()
        {
            this.Title = "General Data";
            this.PrimaryButtonText = "Save";
            this.SecondaryButtonText = "Cancel";

            // ScrollViewer for the dialog content
            var scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                Margin = new Thickness(10),
                Padding = new Thickness(10),
                Width = 450,
                MaxHeight = 400
            };

            var mainPanel = new StackPanel
            {
                Spacing = 5
            };

            // Initialize all text boxes
            _customerTextBox = CreateTextBox("Chevron Pacific Indonesia");
            _leaseTextBox = CreateTextBox("");
            _wellNumberTextBox = CreateTextBox("3174C");
            _apiNumberTextBox = CreateTextBox("");
            _zoneTextBox = CreateTextBox("");
            _locationTextBox = CreateTextBox("DSF");
            _countryTextBox = CreateTextBox("Indonesia");
            _stateTextBox = CreateTextBox("");
            _customerRepresentativeTextBox = CreateTextBox("Mulyani / ISTACHORI");
            _dcsgLegalDescriptionTextBox = CreateTextBox("");
            _serviceDateTextBox = CreateTextBox("1 OCT 2017");
            _serviceTypeTextBox = CreateTextBox("SOLVENT");
            _serviceDistrictBoatTextBox = CreateTextBox("");
            _bjRepresentativeTextBox = CreateTextBox("Ade Suleman");
            _jobNumberTextBox = CreateTextBox("");
            
            // Add form fields
            mainPanel.Children.Add(CreateFormFieldGrid("Customer:", _customerTextBox));
            mainPanel.Children.Add(CreateFormFieldGrid("Lease:", _leaseTextBox));
            mainPanel.Children.Add(CreateFormFieldGrid("Well Number:", _wellNumberTextBox));
            mainPanel.Children.Add(CreateFormFieldGrid("API Number:", _apiNumberTextBox));
            mainPanel.Children.Add(CreateFormFieldGrid("Zone:", _zoneTextBox));
            mainPanel.Children.Add(CreateFormFieldGrid("Location:", _locationTextBox));
            mainPanel.Children.Add(CreateFormFieldGrid("Country:", _countryTextBox));
            mainPanel.Children.Add(CreateFormFieldGrid("State:", _stateTextBox));
            mainPanel.Children.Add(CreateFormFieldGrid("Customer Representative:", _customerRepresentativeTextBox));
            mainPanel.Children.Add(CreateFormFieldGrid("DCSG / Legal Description:", _dcsgLegalDescriptionTextBox));
            mainPanel.Children.Add(CreateFormFieldGrid("Service Date:", _serviceDateTextBox));
            mainPanel.Children.Add(CreateFormFieldGrid("Service Type:", _serviceTypeTextBox));
            mainPanel.Children.Add(CreateFormFieldGrid("Service District / Boat:", _serviceDistrictBoatTextBox));
            mainPanel.Children.Add(CreateFormFieldGrid("BJ Representative:", _bjRepresentativeTextBox));
            mainPanel.Children.Add(CreateFormFieldGrid("Job Number:", _jobNumberTextBox));

            scrollViewer.Content = mainPanel;
            
            // Set dialog content and size
            this.Content = scrollViewer;
            this.Width = 500;
            this.Height = 500;
            this.MaxWidth = 600;
            this.MaxHeight = 600;
            
            // Event handler for save button
            this.PrimaryButtonClick += OnSaveButtonClick;
        }

        public GeneralDataDialog(GeneralData existingData) : this()
        {
            LoadData(existingData);
        }

        private void LoadData(GeneralData data)
        {
            _customerTextBox.Text = data.Customer;
            _leaseTextBox.Text = data.Lease;
            _wellNumberTextBox.Text = data.WellNumber;
            _apiNumberTextBox.Text = data.ApiNumber;
            _zoneTextBox.Text = data.Zone;
            _locationTextBox.Text = data.Location;
            _countryTextBox.Text = data.Country;
            _stateTextBox.Text = data.State;
            _customerRepresentativeTextBox.Text = data.CustomerRepresentative;
            _dcsgLegalDescriptionTextBox.Text = data.DcsgLegalDescription;
            _serviceDateTextBox.Text = data.ServiceDate;
            _serviceTypeTextBox.Text = data.ServiceType;
            _serviceDistrictBoatTextBox.Text = data.ServiceDistrictBoat;
            _bjRepresentativeTextBox.Text = data.BjRepresentative;
            _jobNumberTextBox.Text = data.JobNumber;
        }

        private TextBox CreateTextBox(string defaultText = "")
        {
            return new TextBox
            {
                Text = defaultText,
                Height = 32,
                MinWidth = 100,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(4, 2, 4, 2),
                FontSize = 13,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
        }

        private Grid CreateFormFieldGrid(string label, TextBox textBox)
        {
            var grid = new Grid
            {
                Height = 38,
                Margin = new Thickness(0, 2, 0, 2)
            };

            // Mengatur width kolom - sesuaikan dengan kebutuhan
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(180) }); // Label width
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(245) }); // TextBox width

            var labelBlock = new TextBlock
            {
                Text = label,
                FontWeight = Microsoft.UI.Text.FontWeights.Normal,
                FontSize = 13,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 2, 0)
            };

            Grid.SetColumn(labelBlock, 0);
            Grid.SetColumn(textBox, 1);

            grid.Children.Add(labelBlock);
            grid.Children.Add(textBox);

            return grid;
        }

        private void OnSaveButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            try
            {
                // Collect data from form
                GeneralData = new GeneralData
                {
                    Customer = _customerTextBox.Text?.Trim() ?? "",
                    Lease = _leaseTextBox.Text?.Trim() ?? "",
                    WellNumber = _wellNumberTextBox.Text?.Trim() ?? "",
                    ApiNumber = _apiNumberTextBox.Text?.Trim() ?? "",
                    Zone = _zoneTextBox.Text?.Trim() ?? "",
                    Location = _locationTextBox.Text?.Trim() ?? "",
                    Country = _countryTextBox.Text?.Trim() ?? "",
                    State = _stateTextBox.Text?.Trim() ?? "",
                    CustomerRepresentative = _customerRepresentativeTextBox.Text?.Trim() ?? "",
                    DcsgLegalDescription = _dcsgLegalDescriptionTextBox.Text?.Trim() ?? "",
                    ServiceDate = _serviceDateTextBox.Text?.Trim() ?? "",
                    ServiceType = _serviceTypeTextBox.Text?.Trim() ?? "",
                    ServiceDistrictBoat = _serviceDistrictBoatTextBox.Text?.Trim() ?? "",
                    BjRepresentative = _bjRepresentativeTextBox.Text?.Trim() ?? "",
                    JobNumber = _jobNumberTextBox.Text?.Trim() ?? ""
                };

                System.Diagnostics.Debug.WriteLine("GeneralDataDialog: Data saved successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GeneralDataDialog: Error saving data: {ex.Message}");
                args.Cancel = true;
            }
        }
    }
}