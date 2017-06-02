// include Fake lib
#r @"packages/FAKE/tools/FakeLib.dll"
open Fake

// Properties
let solutionFile = FindFirstMatchingFile "*.sln" currentDirectory
let csprojFiles = filesInDirMatchingRecursive "*.csproj"  (directoryInfo "./src/")
let nuspecFiles = filesInDirMatching "*.nuspec" (directoryInfo "./build/nuspec/")
let packageFiles =
  subDirectories (directoryInfo "./src")
  |> Seq.map (filesInDirMatching "package.json")
  |> Seq.concat

let version = environVarOrFail "release"
let informationalVersion =
  if hasBuildParam "buildVersion" then sprintf "%s-%s" version (environVar "buildVersion")
  else version

let nugetPath = FullName "./.nuget/nuget.exe"
let assemblyInfos = !! "./src/**/AssemblyInfo.cs"

let artifactsDir = FullName "./artifacts/"
let appPluginsDir = artifactsDir @@ "App_Plugins" @@ (fileNameWithoutExt solutionFile)

let Exec command args workingDir =
  let result = Shell.Exec(command, args, workingDir)
  if result <> 0 then failwithf "%s exited with error %d" command result

let yarnOrNpm =
  match tryFindFileOnPath "yarnpkg.cmd" with
  | Some path -> path
  | None ->
     match tryFindFileOnPath "npm.cmd" with
     | Some path -> path
     | None -> failwith "yarn or npm could not be found"

// Targets
Target "Clean" (fun _ ->
  CleanDirs [artifactsDir]
)

Target "RestorePackages" (fun _ ->
  solutionFile
  |> RestoreMSSolutionPackages(fun p ->
      { p with
          Retries = 4 })
)

Target "RestoreUiPackages" (fun _ ->
  packageFiles
  |> Seq.iter(fun f -> Exec yarnOrNpm "install" f.DirectoryName)
)

Target "AssemblyInfo" (fun _ ->
  ReplaceAssemblyInfoVersionsBulk assemblyInfos (fun f ->
      { f with
          AssemblyVersion = sprintf "%s.*" version
          AssemblyInformationalVersion = informationalVersion })
)

Target "Build" (fun _ ->
  csprojFiles
  |> Seq.iter (fun path ->
    let buildDir = artifactsDir @@ path.Directory.Name
    !! path.FullName
      |> MSBuildRelease buildDir "Build"
      |> ignore
  )
)

Target "BuildUI" (fun _ ->
  packageFiles
  |> Seq.iter(fun f -> Exec yarnOrNpm (sprintf "run build -- --output-path %s" appPluginsDir) f.DirectoryName)
)

Target "Package" (fun _ ->
  nuspecFiles
  |> Seq.iter (fun path ->
    path.FullName
      |> NuGetPackDirectly (fun p ->
          { p with
              ToolPath = nugetPath
              WorkingDir = currentDirectory
              OutputPath = artifactsDir
              Version = informationalVersion }))
)

Target "Default" DoNothing

// Dependencies
"Clean"
  ==> "RestorePackages" <=> "RestoreUiPackages" <=> "AssemblyInfo"
  ==> "Build" <=> "BuildUI"
  ==> "Default"

"Build" <=> "BuildUi"
  ==> "Package"

// start build
RunTargetOrDefault "Default"
