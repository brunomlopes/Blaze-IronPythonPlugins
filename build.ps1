Task Default -depends Build

Task Build {
    Exec { msbuild source\IronPythonPlugins.sln }
}

Task CopyLocal -depends Build {
    Remove-Item "D:\documents\My Dropbox\blaze\Plugins\IronPythonPlugins" -recurse
    Copy-Item "source\bin\debug\*" -destination "D:\documents\My Dropbox\blaze\Plugins" -recurse
    Copy-Item "source\IronPythonPlugins\*" -destination "D:\documents\My Dropbox\blaze\Plugins\IronPythonPlugins"
}
    