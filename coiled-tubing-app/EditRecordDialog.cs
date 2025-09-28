using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;

namespace coiled_tubing_app
{
    public sealed class EditRecordDialog : ContentDialog
    {
        private readonly TextBox _recordNameTextBox;
        private readonly TextBlock _errorTextBlock;

        public string RecordName { get; private set; } = "";

        public EditRecordDialog(string currentName)
        {
            // Setup dialog properties
            this.Title = "Edit Record Name";
            this.PrimaryButtonText = "Save";
            this.SecondaryButtonText = "Cancel";

            // Create UI elements
            var mainPanel = new StackPanel
            {
                Width = 400,
                Spacing = 15,
                Margin = new Thickness(20)
            };

            // Record name input section
            var nameLabel = new TextBlock
            {
                Text = "Record Name:",
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                FontSize = 14,
                Margin = new Thickness(0, 0, 0, 5)
            };

            _recordNameTextBox = new TextBox
            {
                Text = currentName, // Pre-populate with current name
                PlaceholderText = "Enter record name...",
                Height = 40,
                FontSize = 14
            };

            // Error text block (initially hidden)
            _errorTextBlock = new TextBlock
            {
                Text = "Record name cannot be empty.",
                Foreground = new SolidColorBrush(Microsoft.UI.Colors.Red),
                FontSize = 12,
                Margin = new Thickness(0, 5, 0, 0),
                Visibility = Visibility.Collapsed
            };

            // Add elements to main panel
            mainPanel.Children.Add(nameLabel);
            mainPanel.Children.Add(_recordNameTextBox);
            mainPanel.Children.Add(_errorTextBlock);

            // Set content
            this.Content = mainPanel;

            // Event handlers
            this.PrimaryButtonClick += OnSaveButtonClick;
            _recordNameTextBox.TextChanged += OnTextChanged;

            // Focus on text box and select all text when dialog opens
            this.Loaded += (s, e) =>
            {
                _recordNameTextBox.Focus(FocusState.Programmatic);
                _recordNameTextBox.SelectAll();
            };
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            // Hide error when user starts typing
            if (_errorTextBlock.Visibility == Visibility.Visible)
            {
                _errorTextBlock.Visibility = Visibility.Collapsed;
            }
        }

        private void OnSaveButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            try
            {
                // Validate record name
                string recordName = _recordNameTextBox.Text?.Trim() ?? "";

                if (string.IsNullOrEmpty(recordName))
                {
                    // Show error and prevent dialog from closing
                    _errorTextBlock.Visibility = Visibility.Visible;
                    args.Cancel = true;
                    return;
                }

                // If validation passes, set the record name
                RecordName = recordName;
                
                // Allow dialog to close
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"EditRecordDialog OnSaveButtonClick error: {ex.Message}");
                args.Cancel = true;
            }
        }
    }
}