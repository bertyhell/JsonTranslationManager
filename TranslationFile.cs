using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JsonTranslationManager
{
	class TranslationFile
	{
		public string Name { get; set; }
		public string Path { get; set; }
		public List<TranslationPair> TranslationPairs { get; set; }
	}
}
