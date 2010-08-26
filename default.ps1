Properties {
    $root_dir = Split-Path $psake.build_script_file	
    $build_artifacts_dir = "$root_dir\build\"
    $package_dir = "$root_dir\package"
    $code_dir = "source"
    $solution = "$code_dir\IronPythonPlugins.sln"
    $api_key = Get-Content "api.key"
    $version = "1.1"
    $configuration = "Debug"
    $blaze_portable = "lib\blaze"
    $blaze_version = "0.5.6.10"
}

Task Default -depends Build

Task Build {
    Exec { msbuild $solution }
}

Task Clean {
    Exec { msbuild "$solution" /t:Clean /p:Configuration=$configuration /v:quiet "/p:OutDir=$build_artifacts_dir\Plugins\\" }
    
    if (Test-Path $build_artifacts_dir){
        Remove-Item $build_artifacts_dir -recurse
    }
    if (Test-Path $package_dir){
        Remove-Item $package_dir -recurse
    }
}

# yeah, this duplication here sucks
# but I was at the end of the timebox and needed it to work.
# FIXME: create correct tasks to package both the plugin and blaze+plugin
Task BuildPackage -depends Clean {
    if (-not (Test-Path $build_artifacts_dir)){
        mkdir $build_artifacts_dir
    }
        
    if (-not (Test-Path $package_dir)){
        mkdir $package_dir
    }
    
    Write-Host "Building" -ForegroundColor Green
    Exec { msbuild "$solution" /t:Build /p:Configuration=$configuration /v:quiet "/p:OutDir=$build_artifacts_dir\Plugins\\" }
    Copy-Item "source\IronPythonPlugins\*" -destination "$build_artifacts_dir\Plugins\IronPythonPlugins"
 
    
    Write-Host "Creating packages" -ForegroundColor Green
    Get-ChildItem $build_artifacts_dir\Plugins -recurse | Write-Zip -IncludeEmptyDirectories -EntryPathRoot "build\Plugins" -OutputPath $package_dir\ironpythonplugins-$version.zip
}


Task BuildFullPackage -depends Clean {
     if (-not (Test-Path $build_artifacts_dir)){
        mkdir $build_artifacts_dir
    }
        
    if (-not (Test-Path $package_dir)){
        mkdir $package_dir
    }
    
    Copy-Item $blaze_portable\* $build_artifacts_dir -recurse -force 
    
    Write-Host "Building" -ForegroundColor Green
    Exec { msbuild "$solution" /t:Build /p:Configuration=$configuration /v:quiet "/p:OutDir=$build_artifacts_dir\Plugins\\" }
    Copy-Item "source\IronPythonPlugins\*" -destination "$build_artifacts_dir\Plugins\IronPythonPlugins"
    
    Write-Host "Creating packages" -ForegroundColor Green
    
    Get-ChildItem $build_artifacts_dir\ -recurse | Write-Zip -IncludeEmptyDirectories -EntryPathRoot "build" -OutputPath $package_dir\blaze-portable-$blaze_version-with-ironpythonplugins-$version.zip
}

# yes, the order between BuildPackage and BuildFullPackage is important
Task BuildPackages -depends BuildPackage,BuildFullPackage {}

Task CopyLocal -depends Build {
    get-process | where{$_.ProcessName.StartsWith("Blaze")} | stop-process
    Remove-Item "D:\documents\My Dropbox\blaze\Plugins\IronPythonPlugins" -recurse
    Copy-Item "source\bin\$configuration\*" -destination "D:\documents\My Dropbox\blaze\Plugins" -recurse
    Copy-Item "source\IronPythonPlugins\*" -destination "D:\documents\My Dropbox\blaze\Plugins\IronPythonPlugins"
    Start-Process "D:\documents\My Dropbox\blaze\Blaze.exe" -WorkingDirectory "D:\documents\My Dropbox\blaze\"
}
