@echo off
echo Automator Configurator 0.5.6

:: set user configuration file
set file=User\user.ini
:: clear user configuration file
cd.> %file%

:: write interaction settings
echo [interaction] >> %file%
echo hotkeyMainkey=32 >> %file%
echo hotkeyModifierAlt=true >> %file%
echo hotkeyModifierCtrl=true >> %file%
echo hotkeyModifierShift=false >> %file%
echo hotkeyModifierWin=false >> %file%
:: insert a blank line
::echo. >> %file%

:: write directory settings
ver | find "XP" > nul
if %ERRORLEVEL% == 0 goto WindowsXP

ver | find "6.0" > nul
if %ERRORLEVEL% == 0 goto WindowsVista

echo Your operative system is not yet supported.
goto Exit

:WindowsXP
echo Configuring Automator for Microsoft Windows XP...
echo [indexer] >> %file%
echo 1\name=%ALLUSERSPROFILE%\Start Menu\ >> %file%
echo 1\extensions=.lnk >> %file%
echo 1\indexSubfolders=true >> %file%
echo 1\plugins= >> %file%
echo 2\name=%USERPROFILE%\Start Menu\ >> %file%
echo 2\extensions=.lnk >> %file%
echo 2\indexSubfolders=true >> %file%
echo 2\plugins= >> %file%
echo 3\name=%USERPROFILE%\Recent\ >> %file%
echo 3\extensions=.lnk >> %file%
echo 3\indexSubfolders=true >> %file%
echo 3\plugins= >> %file%
echo 4\name=%APPDATA%\Microsoft\Internet Explorer\Quick Launch\ >> %file%
echo 4\extensions=.lnk >> %file%
echo 4\indexSubfolders=true >> %file%
echo 4\plugins= >> %file%
echo 5\name=C:\Windows\System32\ >> %file%
echo 5\extensions=.msc >> %file%
echo 5\indexSubfolders=false >> %file%
echo 5\plugins= >> %file%
echo 6\name=Utilities\ >> %file%
echo 6\extensions=.lnk >> %file%
echo 6\indexSubfolders=true >> %file%
echo 6\plugins= >> %file%
echo 7\name=%USERPROFILE%\Favorites\ >> %file%
echo 7\extensions=.url >> %file%
echo 7\indexSubfolders=true >> %file%
echo 7\plugins= >> %file%
goto Exit

:WindowsVista
echo Configuring Automator for Microsoft Windows Vista...
echo [indexer] >> %file%
echo 1\name=%SYSTEMDRIVE%\ProgramData\Microsoft\Windows\Start Menu\ >> %file%
echo 1\extensions=.lnk >> %file%
echo 1\indexSubfolders=true >> %file%
echo 1\plugins= >> %file%
echo 2\name=%USERPROFILE%\AppData\Roaming\Microsoft\Windows\Start Menu\ >> %file%
echo 2\extensions=.lnk >> %file%
echo 2\indexSubfolders=true >> %file%
echo 2\plugins= >> %file%
echo 3\name=%USERPROFILE%\AppData\Roaming\Microsoft\Windows\Recent\ >> %file%
echo 3\extensions=.lnk >> %file%
echo 3\indexSubfolders=true >> %file%
echo 3\plugins= >> %file%
echo 4\name=%APPDATA%\Microsoft\Internet Explorer\Quick Launch\ >> %file%
echo 4\extensions=.lnk >> %file%
echo 4\indexSubfolders=true >> %file%
echo 4\plugins= >> %file%
echo 5\name=C:\Windows\System32\ >> %file%
echo 5\extensions=.msc >> %file%
echo 5\indexSubfolders=false >> %file%
echo 5\plugins= >> %file%
echo 6\name=Utilities\ >> %file%
echo 6\extensions=.lnk >> %file%
echo 6\indexSubfolders=true >> %file%
echo 6\plugins= >> %file%
echo 7\name=%USERPROFILE%\Favorites\ >> %file%
echo 7\extensions=.url >> %file%
echo 7\indexSubfolders=true >> %file%
echo 7\plugins= >> %file%
goto Exit

:Exit
set file=
echo Done!
