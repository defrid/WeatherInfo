﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Globalization;
using System.Diagnostics;
using System.Windows;
using System.ComponentModel;
using Tomers.WPF.Localization;

namespace WeatherInfo.Classes
{
	public class XmlLanguageDictionary : LanguageDictionary
	{
		private Dictionary<string, Dictionary<string, string>> _data =
			new Dictionary<string, Dictionary<string, string>>();

		private string _path;
		private string _cultureName;
		private string _englishName;

		public string Path
		{
			get { return _path;  }
			set { _path = value; }
		}

		public override string CultureName
		{
			get { return _cultureName; }
		}

		public override string EnglishName
		{
			get { return _englishName; }
		}

		public XmlLanguageDictionary(string path)
		{
			if (!File.Exists(path))
			{
				throw new ArgumentException(string.Format(LanguageDictionary.Current.Translate<string>("checkFilePathFailed_XmlLangDict", "Content"), path));
			}
			this._path = path;
		}

		protected override void OnLoad()
		{
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(_path);
                if (xmlDocument.DocumentElement.Name != "Dictionary")
                {
                    throw new XmlException(LanguageDictionary.Current.Translate<string>("invalidRoot_XmlLangDict", "Content"));
                }
                XmlAttribute englishNameAttribute = xmlDocument.DocumentElement.Attributes["EnglishName"];
                if (englishNameAttribute != null)
                {
                    _englishName = englishNameAttribute.Value;
                }
                XmlAttribute cultureNameAttribute = xmlDocument.DocumentElement.Attributes["CultureName"];
                if (cultureNameAttribute != null)
                {
                    _cultureName = cultureNameAttribute.Value;
                }
                foreach (XmlNode node in xmlDocument.DocumentElement.ChildNodes)
                {
                    if (node.Name == "Value")
                    {
                        Dictionary<string, string> innerData = new Dictionary<string, string>();
                        foreach (XmlAttribute attribute in node.Attributes)
                        {
                            if (attribute.Name == "Id")
                            {
                                if (!_data.ContainsKey(attribute.Value))
                                {
                                    _data[attribute.Value] = innerData;
                                }
                            }
                            else
                            {
                                innerData[attribute.Name] = attribute.Value;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var message = "Произошла ошибка при загрузке файла локализации или смене локализации. Возможно файл локализации повреждён или словарь загружен некорректно.";
                MessageBox.Show(message);
                throw new Exception(message);
            }
		}

		protected override void OnUnload()
		{
			_data.Clear();
		}

		protected override object OnTranslate(string uid, string vid, object defaultValue, Type type)
		{
            try
            {
                if (string.IsNullOrEmpty(uid))
                {
                    #region Trace
                    Debug.WriteLine(string.Format(LanguageDictionary.Current.Translate<string>("uidEmpty_XmlLangDict", "Content")));
                    #endregion
                    return defaultValue;
                }
                if (string.IsNullOrEmpty(vid))
                {
                    #region Trace
                    Debug.WriteLine(string.Format(LanguageDictionary.Current.Translate<string>("uidEmpty_XmlLangDict", "Content")));
                    #endregion
                    return defaultValue;
                }
                if (!_data.ContainsKey(uid))
                {
                    #region Trace
                    Debug.WriteLine(string.Format(LanguageDictionary.Current.Translate<string>("uidNotFoundForDict_XmlLangDict", "Content"), uid, EnglishName));
                    #endregion
                    return defaultValue;
                }
                Dictionary<string, string> innerData = _data[uid];

                if (!innerData.ContainsKey(vid))
                {
                    #region Trace
                    Debug.WriteLine(string.Format(LanguageDictionary.Current.Translate<string>("vidNotFoundForUidForDict_XmlLangDict", "Content"), vid, uid, EnglishName));
                    #endregion
                    return defaultValue;
                }
                string textValue = innerData[vid];
                try
                {
                    if (type == typeof(object))
                    {
                        return textValue;
                    }
                    else
                    {
                        TypeConverter typeConverter = TypeDescriptor.GetConverter(type);
                        object translation = typeConverter.ConvertFromString(textValue);
                        return translation;
                    }
                }
                catch (Exception ex)
                {
                    #region Trace
                    Debug.WriteLine(string.Format(LanguageDictionary.Current.Translate<string>("failedTransTextInDict_XmlLangDict", "Content"), textValue, EnglishName, ex.Message));
                    #endregion
                    return null;
                }
            }
            catch (Exception ex)
            {
                var message = "Произошла ошибка при загрузке файла локализации или смене локализации. Возможно файл локализации повреждён или словарь загружен некорректно.";
                MessageBox.Show(message);
                //throw new Exception(message);
                return null;
            }
		}		
	}
}
