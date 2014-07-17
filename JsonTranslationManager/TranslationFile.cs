using System.Collections.ObjectModel;

namespace JsonTranslationManager
{
	class TranslationFile
	{
		public string Name { get; set; }
		public string Path { get; set; }
		public ObservableCollection<TranslationPair> TranslationPairs { get; set; }
	}
}
