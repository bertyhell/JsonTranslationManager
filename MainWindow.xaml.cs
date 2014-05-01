using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using Ookii.Dialogs.Wpf;

namespace JsonTranslationManager
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		private readonly MainController _controller;

		public MainWindow()
		{
			InitializeComponent();

			DataContext = _controller = new MainController(this);

			_controller.RefreshTranslationFiles();
		}

		private void BrowseButtonClick(object sender, RoutedEventArgs e)
		{
			var dialog = new VistaFolderBrowserDialog { Description = "Please select the translations folder", UseDescriptionForTitle = true, SelectedPath = _controller.SelectedFolder };
			bool? showDialog = dialog.ShowDialog(this);
			if (showDialog != null && (bool)showDialog)
			{
				_controller.SelectedFolder = dialog.SelectedPath;
			}
		}

		public void UpdateTabs()
		{
			TabControl.Items.Clear();
			foreach (TranslationFile translationFile in _controller.TranslationFiles)
			{
				TabItem tab = AddTab(translationFile.Name);

				Style rowStyle = new Style();
				rowStyle.Setters.Add(new Setter(BackgroundProperty, new Binding("Score") { Converter = new Int2ColorConverter() }));
				DataGrid dataGrid = new DataGrid
									{
										ItemsSource = translationFile.TranslationPairs,
										AutoGenerateColumns = false,
										RowStyle = rowStyle
									};

				dataGrid.CurrentCellChanged += DataGridOnCurrentCellChanged;

				StackPanel tabHeader = new StackPanel { Orientation = Orientation.Horizontal };
				tabHeader.Children.Add(
					new Image { Source = new BitmapImage(new Uri(string.Format("/JsonTranslationManager;component/img/flags/{0}.png", translationFile.Name.Split('-')[0].ToLower()), UriKind.Relative)) });
				tabHeader.Children.Add(new Label { Content = translationFile.Name, VerticalAlignment = VerticalAlignment.Center });
				tabHeader.Children.Add(new Ellipse
					{
						Width = 10,
						Height = 10,
						VerticalAlignment = VerticalAlignment.Center,
						Fill = translationFile.TranslationPairs.Min(tp => tp.Score) > 0 ? Int2ColorConverter.ConvertDouble2Brush(1) : Int2ColorConverter.ConvertDouble2Brush(0)
					});
				tab.Header = tabHeader;

				//Style tabHeaderStyle = new Style();
				//tabHeaderStyle.Setters.Add(new Setter(BackgroundProperty, Brushes.DarkGray));

				Style style = new Style { TargetType = typeof(TabItem) };
				DataTrigger trigger = new DataTrigger
									  {
										  Value = true,
										  Binding =
											  new Binding
											  {
												  Path = new PropertyPath("IsSelected"),
												  RelativeSource = RelativeSource.Self
											  }
									  };
				//set binding
				Setter setter = new Setter { Property = BackgroundProperty, Value = Brushes.DarkGray };
				trigger.Setters.Add(setter);
				//clear the triggers
				style.Triggers.Clear();
				style.Triggers.Add(trigger);
				tab.Style = style;


				DataGridTextColumn textColumn = new DataGridTextColumn
														{
															Header = "Name",
															Width = new DataGridLength(1, DataGridLengthUnitType.Star),
															Binding = new Binding("Key")
														};
				dataGrid.Columns.Add(textColumn);
				//todo add country flags

				dataGrid.Columns.Add(new DataGridTextColumn
					{
						Header = "Value",
						Width = new DataGridLength(1, DataGridLengthUnitType.Star),
						Binding = new Binding("Value")
					});

				foreach (DataGridColumn column in dataGrid.Columns)
				{
					column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
				}
				tab.Content = dataGrid;
			}
		}

		private void DataGridOnCurrentCellChanged(object sender, EventArgs eventArgs)
		{
			_controller.ChangesPending = true;
			_controller.RefreshScores();
		}

		private TabItem AddTab(string name)
		{
			TabItem tabItem = new TabItem { Header = name };
			TabControl.Items.Insert(0, tabItem);
			return tabItem;
		}

		private void RefreshButtonClick(object sender, RoutedEventArgs e)
		{
			if (_controller.ChangesPending)
			{
				MessageBoxResult result = MessageBox.Show("Changes are pending, want to save changes to files before refresh?", "Pending changes",
					MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes);
				if (result == MessageBoxResult.Yes)
				{
					SaveButtonClick(null, null);
				}
				else if (result == MessageBoxResult.No)
				{
					_controller.RefreshTranslationFiles();
					MessageBox.Show("Refreshed " + _controller.TranslationFiles.Count + " files.");
				}
			}
		}

		private void SaveButtonClick(object sender, RoutedEventArgs e)
		{
			_controller.SaveChangesToFiles();
			MessageBox.Show("Saved " + _controller.TranslationFiles.Count + " files.");
		}
	}
}
