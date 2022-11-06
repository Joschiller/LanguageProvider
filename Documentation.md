# LanguageProvider

- [1 Introduction](#1-introduction)
- [2 API](#2-api)
  - [2.1 LanguageProvider (static class)](#21-languageprovider-static-class)
  - [2.2 LanguageUser (interface)](#22-languageuser-interface)
  - [2.3 UpdatedLanguageUser (interface)](#23-updatedlanguageuser-interface)

## 1 Introduction

The `LanguageProvider` offers the possibility to manage available json-language-files. It allows the centralized access to the configured languages and an automatic update of registered components whenever the language is changed.

## 2 API

### 2.1 LanguageProvider (static class)

> Offers string loading mechanisms for an automatic update of the GUI.
>
> For the configuration of the `LanguageProvider` call `ConfigureLanguages(Dictionary{string, byte[]}, string)`.
> Hand in the existing language files of the application to configure the available languages and select a default language.
>
> The language files must be a valid json-file containing a single root node. Cross-references of template strings can be assembled using the `${json.path.to.string}`-pattern within a value.
>
> `LanguageProvider.ConfigureLanguages(new Dictionary<string, byte[]> {{ "English", Properties.Resources.EnglishLanguageFile }}, "English");`
>
> To automatically update components that need strings in their GUI implement the `UpdatedLanguageUser` interface accordingly.
>
> The configured languages can be retrieved using `LanguageList` as well as `DefaultLanguage` and `CurrentLanguage`.

Accessible Interface:

```c#
public static void ConfigureLanguages(Dictionary<string, byte[]> languages, string defaultLanguage)
public static string CurrentLanguage
public static string DefaultLanguage
public static List<string> LanguageList

public static void Register(UpdatedLanguageUser languageUser)
public static void Unregister(UpdatedLanguageUser languageUser)
public static void RegisterUnique(UpdatedLanguageUser languageUser)
public static void UpdateAllSubcribers()

public static string getString(string path)
public static string getString(string language, string path)
```

### 2.2 LanguageUser (interface)

> A component that needs access to string resources and will automatically be updated by the `LanguageProvider`.

Accessible Interface:

```c#
void LoadTexts(string language)
```

### 2.3 UpdatedLanguageUser (interface)

> A component that needs access to string resources.

Accessible Interface:

```c#
void RegisterAtLanguageProvider()
```

> Extends `LanguageUser`
