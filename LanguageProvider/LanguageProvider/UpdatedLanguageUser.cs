namespace LanguageProvider
{
    /// <summary>
    /// A component that needs access to string resources and will automatically be updated by the <see cref="LanguageProvider"/>.
    /// </summary>
    public interface UpdatedLanguageUser : LanguageUser
    {
        /// <summary>
        /// Register for language updates using <see cref="LanguageProvider.Register(UpdatedLanguageUser)"/> or <see cref="LanguageProvider.RegisterUnique(UpdatedLanguageUser)"/>.
        /// </summary>
        void RegisterAtLanguageProvider();
    }
}
