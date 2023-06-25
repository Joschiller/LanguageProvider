# Release

> <span style="color:red"><b>The followint steps only apply for the following two types of Visual Studio projects: `Class Library (.NET Framework)`, `WPF User Control Library (.NET Framework)`</b></span>

1. Update [Documentation](./Documentation.md) and perform a check of the content for any erros (e.g. using MS Word)
2. Update [Changelog](./Changelog.md)
   - document new features
   - document open issues that have a direct impact regarding the functionality
     > Add a "> Solved with Version x.x.x" mark at any open issue when it is solved in a later version.
   - document solved bugs
3. Simplify code
   - remove unnecessary imports
   - check autocompletion hints
4. Build and release library
   1. configure project and nuget
      - first setup:
        1. configure project > Tab Application:
           - Assembly name
           - Assembly information:
             - Title
             - Description
             - Assembly version
             - File version
             - Neutral language
        2. set `Build > Configuration Manager` to Debug
        3. run `Build > Build Solution`
        4. for first setup:
           1. open `Tools > Command Line > Developer Command Prompt`
           2. **open folder in which the `.csproj` is located**
           3. run `nuget spec NameOfTheProject`
           4. edit generated `NameOfTheProject.nuspec`
              ```xml
               <?xml version="1.0" encoding="utf-8"?>
               <package >
                  <metadata>
                     <id>$id$</id>
                     <version>$version$</version>
                     <title>$title$</title>
                     <authors>...</authors>
                     <requireLicenseAcceptance>true</requireLicenseAcceptance>
                     <license type="file">LICENSE.md</license>
                     <readme>docs\Documentation.md</readme>
                     <projectUrl>https://.../</projectUrl>
                     <description>$description$</description>
                     <copyright>$copyright$</copyright>
                     <tags>... ...</tags>
                  </metadata>
                  <files>
                     <file src="..\..\LICENSE.md" target="" />
                     <file src="..\..\Documentation.md" target="docs\" />
                  </files>
               </package>
              ```
              1. insert `author`
              2. set `<requireLicenseAcceptance>true</requireLicenseAcceptance>`
              3. set `<license type="file">LICENSE.md</license>`
              4. add `<readme>docs\Documentation.md</readme>`
              5. remove empty `icon` line
              6. set `projectUrl`
              7. remove empty `releaseNotes` line
              8. set `tags` separated by space
              9. add `files` section from license and readme
      - subsequent updates:
        1. configure project > Tab Application:
           - Assembly information: increase Assembly version and File version
        2. set `Build > Configuration Manager` to Debug
        3. run `Build > Build Solution`
   2. run `nuget pack`
      > Packed .nupkg-file will be shown in console output
   3. test package locally
      1. open `Manage NuGet Packages`
      2. open NuGet Settings (top right corner)
      3. add package source that points to the folder of the generated package
      4. install the package
      5. update the code and run the application
   4. if expired, create a new API key: https://www.nuget.org/account/apikeys
      - Glob Pattern: `*`
   5. publish package:
      <!-- - Test: `dotnet nuget push someFile.nupkg --api-key someKey --source https://int.nugettest.org/v3/index.json` -->
      - Prod: `dotnet nuget push someFile.nupkg --api-key someKey --source https://api.nuget.org/v3/index.json`
5. Create Release in Repository
   1. `git checkout -b X.X.X-rc`
   2. push new branch
   3. Github > Tags > Create a new Release
      - Tag: "vX.X.X" -> set to create "on publish"
      - Target branch: "X.X.X-rc"
      - Title: "vX.X.X"
      - Description: Copy from Changelog
      - Files: add nuget package

# Update against template

> Run once: `git remote add template https://github.com/Joschiller/Library_Project_Template`

1. `git fetch --all`
2. `git merge template/main --allow-unrelated-histories`
