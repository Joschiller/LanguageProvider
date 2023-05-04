namespace LanguageProvider
{
    /// <summary>
    /// A component that needs access to string resources.
    /// <br/>
    /// Should be used for all user controls or windows, that are updated prorammatically so the <see cref="LanguageProvider"/> does not need to perform the update itself (e.g. dialogs).
    /// </summary>
    public interface LanguageUser
    {
        /// <summary>
        /// Set up the GUI with the strings of <see cref="LanguageProvider.CurrentLanguage"/>. Therefore load the strings using <see cref="LanguageProvider.getString(string)"/> or <see cref="LanguageProvider.getString(string, string)"/>.
        /// <br/>
        /// <br/>
        /// <b>Requires a configured <see cref="LanguageProvider"/> using <see cref="LanguageProvider.ConfigureLanguages(System.Collections.Generic.Dictionary{string, byte[]}, string)"/>.</b>
        /// </summary>
        /// <param name="language">Language to load the strings from</param>
        void LoadTexts(string language);
    }
}
