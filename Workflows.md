# Release

1. Update [Documentation](./Documentation.md) and perform a check of the content for any erros (e.g. using MS Word)
2. Update [Changelog](./Changelog.md)
   - document new features
   - document open issues that have a direct impact regarding the functionality
     > Add a "> Solved with Version x.x.x" mark at any open issue when it is solved in a later version.
   - document solved bugs
3. Simplify code
   - remove unnecessary imports
   - check autocompletion hints
4. Build and release library (https://learn.microsoft.com/de-de/nuget/quickstart/create-and-publish-a-package-using-visual-studio?tabs=netcore-cli)

   1. configure project > Tab Package:
      - Package version
      - Authors
      - Company
      - Description
      - Licensing > File `..\..\LICENSE.md`
      - Repository URL
      - Tags
      - Assembly neutral language
      - Assembly version
      - Assembly file version
   2. set `Build > Configuration Manager` to Release
   3. right-click project in explorer and click "Pack" OR configure `Package > Generate NuGet package on build`
      > Packed file will be shown in console output (usually `\bin\Release\...`)
   4. if expried, create a new API key: https://www.nuget.org/account/apikeys
      - Glob Pattern: `*`
   5. publish package:

      `dotnet nuget push someFile.nupkg --api-key someKey --source https://api.nuget.org/v3/index.json`

   6. Add ReadMe
      1. Open NuGet > Profile > Manage Packages
      2. Click "Edit" for package in latest version
      3. Got to ReadMe > File
      4. Select `Documentation.md`

5. Create Release in Repository
   1. `git checkout -b X.X.X-rc`
   2. push new branch
   3. Github > Tags > Create a new Release
      - Target branch: "X.X.X-rc"
      - Title: "vX.X.X"
      - Description: Copy from Changelog

# Update against template

> Run once: `git remote add template https://github.com/Joschiller/Library_Project_Template`

1. `git fetch --all`
2. `git merge template/main --allow-unrelated-histories`
