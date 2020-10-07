Try {
  $curPath = $MyInvocation.MyCommand.Path
  $curDir = Split-Path $curPath

  $srcPath = join-path $curDir "src\"
  $srcPath
  $csprojFiles = Get-ChildItem $srcPath -Filter *.csproj -Depth 1

  If ($csprojFiles.Count -le 0) {
    "No projects found, ABORTING ..."
    Return
  }

  $csprojIndices = @()


  # query configuration to build
  $config = "Release"
  $input = Read-Host "Which configuration to build? [$($config)]"
  $config = ($config, $input)[[bool]$input]

  # query publicRelease to build
  $publicRelease = "true"
  $input = Read-Host "Public release? [$($publicRelease)]"
  $publicRelease = ($publicRelease, $input)[[bool]$input]

  ""


  Foreach ($csprojFile in $csprojFiles) {
    $solution = $csprojFile
    "Building `"$($solution.BaseName)`" ($config|$publicRelease) ..."
    # $args = @(
    #   "build",
    #   $solution.FullName,
    #   "--configuration $config",
    #   "/p:publicrelease=$($publicRelease)"
    # )
    # & "dotnet" $args
    dotnet build $solution.FullName  -c $config /p:publicrelease=$publicRelease
    "===== BUILD FINISHED ====="
    
    "Packing `"$($solution.BaseName)`" ($config|$publicRelease) ..."

    # $args = @(
    #   "pack",
    #   $solution.FullName,
    #   "--output ..\..\nupkgs\$($solution.BaseName)",
    #   "--configuration $config",
    #   "--include-symbols",
    #   "/p:publicrelease=$($publicRelease)"
    # )
      dotnet pack $solution.FullName  -c $config /p:publicrelease=$publicRelease --output ".\nupkgs\$($solution.BaseName)" --include-symbols
    # & "dotnet" $args

    "===== PACK FINISHED ====="
  }
} Finally {
  ""

  Write-Host -NoNewline "Press any key to continue ... "

  [void][System.Console]::ReadKey($true)
}
