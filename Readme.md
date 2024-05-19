# J-Rdp
---



## Summary
J-Rdp is a tool to automate the editing and launching of rdp files.

It will watch one or more folders for new files, using name criteria filters.\
When a new file is found, several actions can be taken:
- Move the file to another location.
- Edit settings in the file.
- Launch the file.
- Delete the file.
<br/><br/>



## Launching
The application can be run directly as a console app, or silently in the background.\
No installation is needed.

To run it as a console app, simply run the .exe file.

To run it silently, run the .exe file with the argument `-HideConsole`\
A pair of .bat files are provided to start it silently, and to stop it.

To run the app automatically on boot/login, use either of these methods:\
\- Create a shortcut and put it in your startup folder.\
\- Use the Windows task scheduler to run it on login or startup.
<br/><br/>



## Configuration

### Logging
To enable logging to file, use the `-LogToFile` argument when running the .exe file.\
To customize the logging, provide an NLog config file named "nlog.config" in the .exe directory, and it will be used instead of the default logging settings.

By default, one .log file will be generated per day in the "Logs" folder.\
Read them with a text editor such as notepad, or your favourite log reader.


### General configuration
The application is using a configuration file named "config.json" in the same directory as the .exe file.\
An example file is provided, edit it as needed.

Two general settings can be configured:
- `pollingInterval` decides how often, in milliseconds, the watched folder(s) should be checked for new files.\
  Default if omitted: 1000. Must be between 100 and 30000.
- `deleteDelay` decides how long to wait before deleting a file after launching it.\
  Default if omitted: 3000. Must be between 100 and 30000.


### Profiles
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



## Rdp settings
(todo)
<br/><br/>



## Change log
### 0.2.0
(todo)

### 0.1.0
(todo)
