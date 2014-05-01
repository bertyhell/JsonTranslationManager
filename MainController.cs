using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using JsonTranslationManager.Annotations;
using JsonTranslationManager.LanguageService;
using Newtonsoft.Json;

namespace JsonTranslationManager
{
	class MainController : INotifyPropertyChanged
	{
		private readonly MainWindow _mainWindow;
		private string _selectedFolder;
		private List<TranslationFile> _translationFiles;
		private readonly LanguageServiceClient _languageServiceClient;

		private const string APP_ID = "91O/KFDsveEly6nm8FJhVmiHpv7qEzQnEaJ0YnerfOE=";

		public MainController(MainWindow mainWindow)
		{
			_mainWindow = mainWindow;
			TranslationFiles = new List<TranslationFile>();
			_selectedFolder = @"C:\Dropbox\personal\JsonTranslationManager\JsonTranslationMananger\JsonTranslationManager\lang";
			_languageServiceClient = new LanguageServiceClient();
		}

		public string SelectedFolder
		{
			get { return _selectedFolder; }
			set
			{
				if (value == _selectedFolder) return;
				_selectedFolder = value;
				RefreshTranslationFiles();
				OnPropertyChanged();
			}
		}

		public List<TranslationFile> TranslationFiles
		{
			get { return _translationFiles; }
			set
			{
				if (Equals(value, _translationFiles)) return;
				_translationFiles = value;
				OnPropertyChanged();
			}
		}

		public bool ChangesPending { get; set; }

		public void RefreshTranslationFiles()
		{
			TranslationFiles.Clear();
			if (Directory.Exists(SelectedFolder))
			{
				DirectoryInfo folder = new DirectoryInfo(SelectedFolder);

				FileInfo[] fileInfos = folder.GetFiles("*.json", SearchOption.TopDirectoryOnly);

				foreach (FileInfo translationFile in fileInfos)
				{
					StreamReader reader = new StreamReader(new FileStream(translationFile.FullName, FileMode.Open));
					string jsonString = reader.ReadToEnd();
					Dictionary<string, string> jsonPairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);
					List<TranslationPair> translationPairs = jsonPairs.Select(pair => new TranslationPair { Key = pair.Key, Value = pair.Value }).ToList();
					string fileName = translationFile.Name.Substring(0, translationFile.Name.Length - translationFile.Extension.Length);

					TranslationFiles.Add(new TranslationFile { Name = fileName, Path = translationFile.FullName, TranslationPairs = translationPairs });
				}

				RefreshScores();

				_mainWindow.UpdateTabs();
			}
			else
			{
				MessageBox.Show("The selected folder wasn't found.");
			}
		}

		public void RefreshScores()
		{
			//get unique translation keys of all files
			Dictionary<string, Tuple<string, string>> uniqueTranslationKeys = new Dictionary<string, Tuple<string, string>>();
			foreach (TranslationFile translationFile in TranslationFiles)
			{
				foreach (TranslationPair translationPair in translationFile.TranslationPairs)
				{
					if (!uniqueTranslationKeys.ContainsKey(translationPair.Key))
					{
						uniqueTranslationKeys.Add(translationPair.Key, new Tuple<string, string>(translationFile.Name,translationPair.Value));
					}
				}
			}

			//determine score for every key and add missing keys to respective files
			foreach (KeyValuePair<string ,Tuple<string, string>> uniqueTranslationPair in uniqueTranslationKeys)
			{
				double score = (double)TranslationFiles.Count(tf => tf.TranslationPairs.Any(tp => tp.Key == uniqueTranslationPair.Key && !string.IsNullOrWhiteSpace(tp.Value))) / TranslationFiles.Count();
				foreach (TranslationFile translationFile in TranslationFiles)
				{
					TranslationPair translationPair = translationFile.TranslationPairs.FirstOrDefault(tp => tp.Key == uniqueTranslationPair.Key);
					if (translationPair == null)
					{
						//_languageServiceClient.GetAppIdToken(APP_ID, );
						//string autoTranslateValue = _languageServiceClient.Translate(
						//	"Bearer" + " " + APP_ID,
						//	uniqueTranslationPair.Value.Item2,
						//	uniqueTranslationPair.Value.Item1,
						//	translationFile.Name,
						//	"text/plain",
						//	"general"
						//	);
						translationFile.TranslationPairs.Add(new TranslationPair { Key = uniqueTranslationPair.Key, Value = "", Score = 0 });
					}
					else if (string.IsNullOrWhiteSpace(translationPair.Value))
					{
						translationPair.Score = 0;
						//translationPair.Value = _languageServiceClient.Translate(
						//	"Bearer" + " " + APP_ID,
						//	uniqueTranslationPair.Value.Item2,
						//	uniqueTranslationPair.Value.Item1,
						//	translationFile.Name,
						//	"text/plain",
						//	"general"
						//	);
					}
					else
					{
						translationPair.Score = score;
					}
				}
			}

			foreach (TranslationFile translationFile in TranslationFiles)
			{
				translationFile.TranslationPairs = translationFile.TranslationPairs.OrderBy(tp => tp.Score).ThenBy(tp => tp.Key).ToList();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged(string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}

		public void SaveChangesToFiles()
		{
			foreach (TranslationFile translationFile in TranslationFiles)
			{
				StreamWriter streamWriter = new StreamWriter(new FileStream(translationFile.Path, FileMode.Truncate));
				streamWriter.Write("{");
				foreach (TranslationPair translationPair in translationFile.TranslationPairs)
				{
					streamWriter.Write("\"" + translationPair.Key + "\":\"" + translationPair.Value + "\",");
				}
				streamWriter.Write("}");
				streamWriter.Flush();
				streamWriter.Close();
			}
		}
	}
}
