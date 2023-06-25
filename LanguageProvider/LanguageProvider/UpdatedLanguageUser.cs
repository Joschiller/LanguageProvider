namespace LanguageProvider
{
    /// <summary>
    /// A component that needs access to string resources and will automatically be updated by the <see cref="LanguageProvider"/>.
    /// <br/>
    /// Should be used for all user controls or windows, that must be updated automatically whenever the language changes.
    /// </summary>
    public interface UpdatedLanguageUser : LanguageUser
    {
        /// <summary>
        /// Register for language updates using <see cref="LanguageProvider.Register(UpdatedLanguageUser)"/> or <see cref="LanguageProvider.RegisterUnique(UpdatedLanguageUser)"/>.
        /// </summary>
        void RegisterAtLanguageProvider();
    }
}