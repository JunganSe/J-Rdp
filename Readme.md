# J-Rdp
J-Rdp is a tool to automate the editing and launching of rdp files.

It will watch one or more folders for new files, using name criteria filters.\
When a new file is found, several actions can be taken:
- Move the file to another location.
- Edit settings in the file.
- Launch the file.
- Delete the file.
<br/><br/>



# Launching
The application can be run directly as a console app, or silently in the background.\
No installation is needed.

To run it as a console app, simply run the .exe file.

To run it silently, run the .exe file with the `-HideConsole` argument.\
A pair of .bat files are provided to start it silently, and to stop it.

To run the app automatically on boot/login, use either of these methods:\
\- Create a shortcut and put it in your startup folder.\
\- Use the Windows task scheduler to run it on login or startup.
<br/><br/>



# Configuration

## Logging
To enable logging to file, use the `-LogToFile` argument when running the .exe file.\
To customize the logging, provide an NLog config file named "nlog.config" in the .exe directory, and it will be used instead of the default logging settings.

By default, one .log file will be generated per day in the "Logs" folder.\
Read them with a text editor such as notepad, or your favourite log reader.


## General configuration
The application is using a configuration file named "config.json" in the same directory as the .exe file.\
An example file is provided, edit it as needed.

Two general settings can be configured:
- `pollingInterval`: Decides how often, in milliseconds, the watched folder(s) should be checked for new files.\
  Default if omitted: 1000. Must be between 100 and 30000.
- `deleteDelay`: Decides how long to wait before deleting a file after launching it.\
  Default if omitted: 3000. Must be between 100 and 30000.


## Profiles
To configure which folders to watch and which actions should be taken, provide one or more profiles in the config file.

The settings are:
- `enabled`: Set to false to disable the profile without having to remove it from the config file.\
  Optional. Default if omitted: true
- `name`: The name of the profile. Optional. Only used to differentiate the profiles in the config file, and in log messages.\
- `watchFolder`: Path to the folder that should be watched. Must be absolute, e.g. "C:/MyFolder".\
  Accepts forward slash `/` or double (escaped) backslash `\\` as directory separators.
- `filter`: The file name to watch for. The file type ".rdp" will be assumed if not provided.\
  Accepts wildcards. `?` is exactly one character, `*` is zero or more characters.
- `moveToFolder`: A discovered file will be moved here before operations are made.\
  Optional. Path can be absolute or relative (to the watched folder).\
- `launch`: Set to true to launch the file after other operations are made.
  Optional. Default if omitted: false
- `delete`: Set to true to delete the file after all other operations are made.
  Optional: Default if omitted: false
- `settings`: An array of strings, each representing a setting to be made in the .rdp file.\
  Optional. See the chapter about rdp settings below.
<br/><br/>



# Rdp settings
Provide the settings to be applied as an array of strings in the profile.\
If a setting is not already present in the file, it will be added at the end.\
If a setting is already present in the file, the original line will be deleted and a new line will be added at the end.

See the [Microsoft documentation](https://learn.microsoft.com/en-us/windows-server/remote/remote-desktop-services/clients/rdp-files).

Examples of some useful settings:
- `screen mode id:i:*value*`: 1 for windowed, 2 for full screen.
- `desktopwidth:i:*value*`: Resolution width.
- `desktopheight:i:*value*`: Resolution height.
- `winposstr:s:0,1,x1,y1,x2,y2`: Set the size and position of the window by defining a rectangle. See [this post](https://superuser.com/a/665413) for an explanation.
- `smart sizing:i:1`: Scale the contens of the window when it is resized.
- `use multimon:i:1`: Enable multi monitor support. Will use all monitors unless `selectedmonitors:s` is used.
- `selectedmonitors:s:*value*`: Select which monitors to use when using multiple monitors. Zero-based, comma-separated list.\
  Example: A value of "1,2" will use monitor 2 and 3, but not monitor 1.
<br/><br/>



# Change log
## 0.2.0
2024-05-21

Changes and improvements:
- Improved logging.
- Logging can now be enabled or disabled.
- The console window title is now the program name instead of the full path.
- Moved some command line arguments to config file.
- Users can now provide their own nlog.config to customize logging.
- Delete delay is now configurable.
- "watchFolder" path must now be absolute. A relative path would previously be relative to the exe, which makes no sense.
- If "moveToFolder" path is relative, it is now relative to "watchFolder".
- Profiles can now be disabled.
- Existing settings in rdp files will be replaced, instead of duplicates being added at the end.
- ".rdp" file type is assumed in filter, if not explicitly stated. So the filter "\*.txt" will be interpreted as "\*.txt.rdp", while "\*.rdp" will be used as-is.
- Missing settings in config and profiles will use their default values.
- Improved config validation.

Bug fixes:
- Fixed a bug where changes to the config file was not detected after some time.

## 0.1.0
2024-03-27

Initial release.
