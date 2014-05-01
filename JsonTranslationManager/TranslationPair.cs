using System.ComponentModel;
using JsonTranslationManager.Annotations;

namespace JsonTranslationManager
{
	public class TranslationPair : INotifyPropertyChanged
	{
		private string _key;
		private string _value;
		private double _score = 0.5;
		private bool _generated;

		public string Key
		{
			get { return _key; }
			set
			{
				if (value == _key) return;
				_key = value;
				OnPropertyChanged("Key");
			}
		}

		public string Value
		{
			get { return _value; }
			set
			{
				if (value == _value) return;
				_value = value;
				OnPropertyChanged("Value");
			}
		}

		public double Score
		{
			get { return _score; }
			set
			{
				if (value.Equals(_score)) return;
				_score = value;
				OnPropertyChanged("Score");
			}
		}

		public bool Generated
		{
			get { return _generated; }
			set
			{
				if (value.Equals(_generated)) return;
				_generated = value;
				OnPropertyChanged("Generated");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
