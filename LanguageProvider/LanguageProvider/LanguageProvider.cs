using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LanguageProvider
{
    /// <summary>
    /// Offers string loading mechanisms for an automatic update of the GUI.
    /// <br/>
    /// <br/>
    /// <example>
    /// For the configuration of the <see cref="LanguageProvider"/> call <see cref="ConfigureLanguages(Dictionary{string, byte[]}, string)"/>.
    /// Hand in the existing language files of the application to configure the available languages and select a default language.
    /// <br/>
    /// The language files must be a valid json-file containing a single root node. Cross-references of template strings can be assembled using the <c>${json.path.to.string}</c>-pattern within a value.
    /// <code>LanguageProvider.ConfigureLanguages(new Dictionary&lt;string, byte[]&gt; {{ "English", Properties.Resources.EnglishLanguageFile }}, "English");</code>
    /// </example>
    /// <br/>
    /// <br/>
    /// To automatically update components that need strings in their GUI implement the <see cref="UpdatedLanguageUser"/> interface accordingly.
    /// <br/>
    /// <br/>
    /// The configured languages can be retrieved using <see cref="LanguageList"/> as well as <see cref="DefaultLanguage"/> and <see cref="CurrentLanguage"/>.
    /// </summary>
    public static class LanguageProvider
    {
        #region Languages
        /// <summary>
        /// Configure the available languages. The languages are references to json-file-resources of the project. Each entry in the dictionary should have the language name as the key and the file content as the value. The file content for the project resource can be retrieved using <c>Properties.Resources.YourLanguageFile</c>.
        /// <br/>
        /// The language files must be a valid json-file containing a single root node. Cross-references of template strings can be assembled using the <c>${json.path.to.string}</c>-pattern within a value.
        /// <br/>
        /// If the <see cref="CurrentLanguage"/> is not contained within the new configured languages, the <see cref="CurrentLanguage"/> is reset to the new configured <see cref="DefaultLanguage"/>.
        /// </summary>
        /// <param name="languages">
        /// Dictionary of the available languages.
        /// <br/>
        /// <br/>
        /// The key refers to the language name.
        /// <br/>
        /// The value is the content of the json-file. For project resources this content can be retrieved using <c>Properties.Resources.YourLanguageFile</c>.
        /// </param>
        /// <param name="defaultLanguage">
        /// The name of the default language to use as a fallback.
        /// <br/>
        /// <br/>
        /// The default language must be an element of the configured languages. Otherwise an <see cref="ArgumentException"/> will be thrown.
        /// </param>
        public static void ConfigureLanguages(Dictionary<string, byte[]> languages, string defaultLanguage)
        {
            Languages = new Dictionary<string, JObject>();
            foreach (var language in languages) Languages.Add(language.Key, JObject.Parse(System.Text.Encoding.Default.GetString(language.Value)));
            if (!languages.Keys.Contains(defaultLanguage)) throw new ArgumentException("The default language must be an available configured language");
            if (!languages.Keys.Contains(CurrentLanguage)) _CurrentLanguage = null;
            DefaultLanguage = defaultLanguage;
        }
        private static string _CurrentLanguage;
        /// <summary>
        /// The currently configured language. This will be the <see cref="DefaultLanguage"/> if no <see cref="CurrentLanguage"/> was set yet.
        /// <br/>
        /// Changing the <see cref="CurrentLanguage"/> will trigger <see cref="UpdateAllSubcribers"/>.
        /// <br/>
        /// <br/>
        /// The <see cref="CurrentLanguage"/> must be an element of the configured languages. Otherwise an <see cref="ArgumentException"/> will be thrown.
        /// </summary>
        public static string CurrentLanguage
        {
            get
            {
                return _CurrentLanguage ?? DefaultLanguage;
            }
            set
            {
                if (!LanguageList.Contains(value)) throw new ArgumentException("The current language must be an available configured language");
                ResetCache();
                _CurrentLanguage = value;
                UpdateAllSubcribers();
            }
        }
        /// <summary>
        /// The default language to use as a fallback language.
        /// <br/>
        /// <br/>
        /// The <see cref="DefaultLanguage"/> can be configured using <see cref="ConfigureLanguages(Dictionary{string, byte[]}, string)"/>.
        /// </summary>
        public static string DefaultLanguage
        {
            get;
            private set;
        }

        private static Dictionary<string, JObject> Languages = new Dictionary<string, JObject>();
        /// <summary>
        /// List of available languages.
        /// <br/>
        /// <br/>
        /// The available languages can be configured using <see cref="ConfigureLanguages(Dictionary{string, byte[]}, string)"/>.
        /// </summary>
        public static List<string> LanguageList { get => Languages.Keys.ToList(); }
        #endregion
        #region Cache
        /// <summary>
        /// Maximum number of strings to keep in the <see cref="LanguageCache"/>.
        /// </summary>
        private const int CACHELIMIT = 20;
        /// <summary>
        /// Stores the last loaded strings for the current language to ensure faster access.
        /// </summary>
        private static Queue<Tuple<string, string>> LanguageCache = new Queue<Tuple<string, string>>();
        /// <summary>
        /// Reset the cache as the current language is changed.
        /// </summary>
        private static void ResetCache() => LanguageCache.Clear();
        #endregion
        #region Registations
        /// <summary>
        /// List of currently registered <see cref="UpdatedLanguageUser"/>s.
        /// </summary>
        private static List<UpdatedLanguageUser> LanguageUsers = new List<UpdatedLanguageUser>();
        /// <summary>
        /// Register an <see cref="UpdatedLanguageUser"/> to be updated whenever the <see cref="CurrentLanguage"/> is changed.
        /// <br/>
        /// <b>When disposing the instance the <see cref="UpdatedLanguageUser"/> must be unregistered manually with <see cref="Unregister(UpdatedLanguageUser)"/>.</b>
        /// <br/>
        /// <br/>
        /// To automatically register and unregsiter unique instances use <see cref="RegisterUnique(UpdatedLanguageUser)"/>.
        /// </summary>
        /// <param name="languageUser"><see cref="UpdatedLanguageUser"/> to register</param>
        public static void Register(UpdatedLanguageUser languageUser)
        {
            LanguageUsers.Add(languageUser);
            languageUser.LoadTexts(CurrentLanguage);
        }
        /// <summary>
        /// Remove a registered <see cref="UpdatedLanguageUser"/> from language updates.
        /// <br/>
        /// <br/>
        /// To automatically register and unregsiter unique instances use <see cref="RegisterUnique(UpdatedLanguageUser)"/>.
        /// </summary>
        /// <param name="languageUser"><see cref="UpdatedLanguageUser"/> to unregister</param>
        public static void Unregister(UpdatedLanguageUser languageUser)
        {
            LanguageUsers.Remove(languageUser);
        }
        /// <summary>
        /// Register an <see cref="UpdatedLanguageUser"/> to be updated whenever the <see cref="CurrentLanguage"/> is changed.
        /// <br/>
        /// Using this method one instance of a class can be registered at once. Registering another instance will automatically unregister the old one. That way only one instance of a specific window can be registered to be updated.
        /// <br/>
        /// <br/>
        /// To manage several instances of the same class use <see cref="Register(UpdatedLanguageUser)"/> and <see cref="Unregister(UpdatedLanguageUser)"/>.
        /// </summary>
        /// <param name="languageUser"><see cref="UpdatedLanguageUser"/> to register</param>
        public static void RegisterUnique(UpdatedLanguageUser languageUser)
        {
            LanguageUsers.RemoveAll(item => item.GetType().Equals(languageUser.GetType()));
            LanguageUsers.Add(languageUser);
            languageUser.LoadTexts(CurrentLanguage);
        }
        /// <summary>
        /// Update all <see cref="UpdatedLanguageUser"/>s that are subscribed to the update using <see cref="Register(UpdatedLanguageUser)"/>.
        /// </summary>
        public static void UpdateAllSubcribers() => LanguageUsers.ForEach(s => s.LoadTexts(CurrentLanguage));
        #endregion
        #region String Reading
        /// <summary>
        /// Regular expression for string references within a json value.
        /// </summary>
        private const string TemplateRegex = @"\${[^\${}]*}";
        /// <summary>
        /// Get a string using the <see cref="CurrentLanguage"/>. If no value is found for the <see cref="CurrentLanguage"/>, the <see cref="DefaultLanguage"/> is used as a fallback.
        /// <br/>
        /// <br/>
        /// The available languages and <see cref="DefaultLanguage"/> can be configured using <see cref="ConfigureLanguages(Dictionary{string, byte[]}, string)"/>.
        /// </summary>
        /// <param name="path">Path within the json file</param>
        /// <returns>String value</returns>
        public static string getString(string path) => getString(CurrentLanguage, path);
        /// <summary>
        /// Get a string using the given language. If no value is found for the given languge, the <see cref="DefaultLanguage"/> is used as a fallback.
        /// <br/>
        /// <br/>
        /// The available languages and <see cref="DefaultLanguage"/> can be configured using <see cref="ConfigureLanguages(Dictionary{string, byte[]}, string)"/>.
        /// </summary>
        /// <param name="language">Language to search within</param>
        /// <param name="path">Path within the json file</param>
        /// <returns>String value</returns>
        public static string getString(string language, string path)
        {
            // check cache
            if (CurrentLanguage.Equals(language))
            {
                var cachedString = LanguageCache.FirstOrDefault(c => c.Item1 == path)?.Item2;
                if (cachedString != null) return cachedString;
            }
            // load value
            JToken loadedValue = null;
            foreach (var layer in path.Split('.'))
            {
                loadedValue = loadedValue == null ? Languages[language]?[layer] : loadedValue[layer];
                if (loadedValue == null) break; // the path does not exist
            }
            // check if null
            if (loadedValue == null) return DefaultLanguage.Equals(language) ? "" : getString(DefaultLanguage, path);
            try
            {
                var loadedString = (string)loadedValue;
                // replace template strings
                var regex = new Regex(TemplateRegex);
                var match = regex.Match(loadedString);
                while (match.Success)
                {
                    var insertPath = match.Value.Substring(2, match.Value.Length - 3);
                    var insertion = getString(language, insertPath);
                    loadedString = loadedString.Substring(0, match.Index) + insertion + loadedString.Substring(match.Index + match.Value.Length);
                    match = regex.Match(loadedString);
                }

                // cache value
                if (CurrentLanguage.Equals(language))
                {
                    if (LanguageCache.Count > CACHELIMIT) LanguageCache.Dequeue();
                    LanguageCache.Enqueue(new Tuple<string, string>(path, loadedString));
                }
                // return
                return loadedString;
            }
            catch (Exception) { return ""; }
        }
        #endregion
    }
}
